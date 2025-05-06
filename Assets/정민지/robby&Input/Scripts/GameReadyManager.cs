using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using RealtimePlayer = Photon.Realtime.Player;
using PHashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;
using UnityEngine.Windows;
using System;
using System.Linq;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.Events;

public class GameReadyManager : MonoBehaviourPunCallbacks
{
    public UnityAction<Photon.Realtime.Player, Hashtable> PropertiesAction;

    [Header("닉네임")]
    // public GameObject loginchang;
    public TMP_InputField NickNameInput; // 클래스 멤버 변수로 선언

    [Header("로비판넬")]
    public GameObject LobbyPanel;
    public TMP_InputField WelcomeText;
    public Text LobbyInfoText;
    public Button[] CellBtn;
    public Button PreviousBtn;
    public Button NextBtn;
    [SerializeField] private roomClick roomClickScript;
    public GameObject IncorrectPassword;

    [Header("대기실")]
    public Text ListText;
    public Text RoomInfoText; //방 인원수
    public Text[] ChatText;
    public TMP_InputField ChatInput;
    public GameObject roomButtonPrefab; //생성할 방
    private Transform content; //스크롤 콘텐트
    public Text totalPlayersText;  // 인원수를 표시할 텍스트 UI (전체 인원수)

    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;

    [Header("ETC")]
    public Text StatusText;
    public PhotonView PV;

    [Header("UI Panels")]
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject hands;
    [SerializeField] private GameObject waitingPanel;
    [SerializeField] private GameObject BossUI;
    [SerializeField] private GameObject BossPanel;
    [SerializeField] private GameObject RunnerPanel;

    [Header("Slot UI")]
    [SerializeField] private RectTransform[] slotPoints; // length=5
    [SerializeField] private GameObject playerSlotPrefab;


    private Dictionary<Photon.Realtime.Player, PlayerSlot> slotMap = new Dictionary<Photon.Realtime.Player, PlayerSlot>();
    private bool[] occupied = new bool[5];

    private Dictionary<int, int> _profileMap = new();
    private List<int> profileOrder;
    private int nextProfilePointer;

    public static GameReadyManager Instance { get; private set; }

    private void Awake()
    {
        Screen.SetResolution(960, 540, false);

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            PropertiesAction += HandleReadyChanged;
            PropertiesAction += HandleProfileIndexChanged;
            PropertiesAction += HandleRoleChanged;
            PropertiesAction += HandleRoleConfirmed;
        }
        else
        {
            Destroy(this.gameObject);
        }

        test = Test;
        Gc = GetContent;

    }
    private void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;

    }

    public void CleanHandler()
    {
        PropertiesAction -= HandleReadyChanged;
        PropertiesAction -= HandleProfileIndexChanged;
        PropertiesAction -= HandleRoleChanged;
        PropertiesAction -= HandleRoleConfirmed;
    }

    void Start()
    {
        // 시작 시 로비만 보이도록
        //lobbyPanel.SetActive(true);
        //hands.SetActive(true);
        //waitingPanel.SetActive(false);

        // 대기실 나가기
        //leaveButton.onClick.AddListener(() =>
        //{
        //    PhotonNetwork.LeaveRoom();
        //    ClearSlots();
        //    waitingPanel.SetActive(false);
        //    lobbyPanel.SetActive(true);
        //});
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "RobbyScene") return;

        // RobbyScene 에 있는 UI를 찾아서 바인딩
        lobbyPanel = GameObject.Find("robby");
        waitingPanel = GameObject.Find("WaitingUI");
        hands = GameObject.Find("RobbyHands");
        BossUI = GameObject.Find("BossSelectionUI");
        BossPanel = BossUI.transform.Find("BossPanel")?.gameObject;
        RunnerPanel = BossUI.transform.Find("RunnerPanel")?.gameObject;

        slotPoints = new RectTransform[5];
        var waitingUI = GameObject.Find("WaitingUI");
        var roomUI = waitingUI.transform.Find("RoomUI");
        var slotContainer = roomUI.Find("SlotContainer");

        for (int i = 0; i < slotPoints.Length; i++)
        {
            var p = slotContainer.Find($"SlotPoint{i}");
            slotPoints[i] = p.GetComponent<RectTransform>();
        }

        // 초기 UI 상태
        lobbyPanel.SetActive(true);
        hands.SetActive(true);
        waitingPanel.SetActive(false);
        BossPanel.SetActive(false);
        RunnerPanel.SetActive(false);

        // leaveButton 클릭 리스너
        //leaveButton.onClick.RemoveAllListeners();
        //leaveButton.onClick.AddListener(OnLeaveClicked);
    }


    #region 방리스트 갱신



    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (roomList.Count == 0)
        {
            return;
        }

        Debug.Log(roomList.Count);
        if (content == null)
        {
            Debug.LogWarning("Content가 아직 설정되지 않았습니다. GetContent 먼저 호출하세요!");
            return;  // 여기서 바로 리턴
        }

        // 갱신된 방 리스트를 기반으로 UI 업데이트
        foreach (RoomInfo room in roomList)
        {

            if (!room.RemovedFromList)  // 제거된 방은 제외
            {
                if (roomDic.ContainsKey(room.Name))
                {
                    int currentPlayerCount = room.PlayerCount;  // 현재 인원수
                    int maxPlayers = room.MaxPlayers;           // 최대 인원수



                    if (currentPlayerCount == 0)
                    {
                        GameObject roomUI = roomDic[room.Name];
                        roomDic.Remove(room.Name);
                        Destroy(roomUI);  // UI 오브젝트 파괴
                        return;
                    }

                    roomDic[room.Name].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{currentPlayerCount}/{maxPlayers}";  // 인원수 텍스트 설정
                    roomDic[room.Name].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = room.Name;

                }
                else
                {

                    GameObject newButton = Instantiate(roomButtonPrefab, content);
                    newButton.transform.SetParent(content, false); // false를 꼭 넣자!

                    int currentPlayerCount = room.PlayerCount;  // 현재 인원수
                    int maxPlayers = room.MaxPlayers;           // 최대 인원수

                    newButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{currentPlayerCount}/{maxPlayers}";  // 인원수 텍스트 설정
                    newButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = room.Name;

                    string capturedRoomName = room.Name;
                    newButton.GetComponent<Button>().onClick.AddListener(() => {
                        roomClickScript.OnRoomButtonClicked(capturedRoomName); // capturedRoomName 사용
                    });



                    // roomDic.Add(room.Name, newButton);
                    //roomInfoDict.Add(room, newButton);

                    roomDic[room.Name] = newButton;
                    roomInfoDict[room.Name] = room;
                }

            }
            else if (room.RemovedFromList || room.PlayerCount == 0)
            {
                if (roomDic.TryGetValue(room.Name, out GameObject go))
                {
                    Destroy(go);
                    roomDic.Remove(room.Name);
                }
                continue;
            }

        }
    }
    #endregion


    #region 서버연결


    public void Connect() => PhotonNetwork.ConnectUsingSettings(); //접속

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby(); //대기실에 조인

    public override void OnJoinedLobby() //로비로 가기
    {

        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;


        // PhotonNetwork.LocalPlayer.NickName = "adadadad";
        WelcomeText.text = PhotonNetwork.LocalPlayer.NickName + "님 환영합니다";
        PhotonNetwork.LoadLevel("RobbyScene");

    }

    public void Disconnect() => PhotonNetwork.Disconnect(); //서버 접속 끊음

    #endregion


    #region 방

    //비번과 함께 방 생성
    public void CreateRoomWithPassword(string roomName, string password)
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InLobby)
        {

            RoomOptions roomOptions = new RoomOptions(); //새로운 룸 옵션

            roomOptions.MaxPlayers = 5; //옵션에서 최대인원
            roomOptions.EmptyRoomTtl = 0; // 유저가 모두 나가면 즉시 삭제

            // 비밀번호 설정
            Hashtable customProps = new Hashtable();
            customProps.Add("pw", password);
            roomOptions.CustomRoomProperties = customProps;
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "pw" };

            PhotonNetwork.CreateRoom(roomName, roomOptions); //서버에서 룸 생성
            Debug.Log($"{PhotonNetwork.LocalPlayer.NickName}님이 {roomName}이라는 방을 생성하셨습니다!");
        }
        else
        {
            Debug.Log("아직 접속되지 않음");
        }

    }

    //public void OnSubmitPassword(string inputPassword)
    //{
    //   // RoomInfo[] roomList = myList;
    //    RoomInfo roomInfo = myList.FirstOrDefault(r => r.Name == ClickRoom.selectedRoomName);

    //    if (roomInfo != null)
    //    {
    //        TryJoinRoom(roomInfo, inputPassword);
    //    }
    //    else
    //    {
    //        Debug.LogError("방 정보를 찾을 수 없습니다.");
    //    }
    //}

    public void TryJoinRoom(RoomInfo roomInfo, string inputPassword)
    {
        // Check if the room has a password set
        if (roomInfo.CustomProperties.ContainsKey("pw"))
        {
            string roomPassword = (string)roomInfo.CustomProperties["pw"];

            if (roomPassword == inputPassword)
            {
                PhotonNetwork.JoinRoom(roomInfo.Name);
            }
            else
            {
                GameObject currentWarning = Instantiate(IncorrectPassword);

                if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) && currentWarning != null)
                {
                    Destroy(currentWarning);
                }

            }
        }
        else
        {
            Debug.Log("이 방은 비밀번호가 없습니다!"); // "This room does not have a password."
            PhotonNetwork.JoinRoom(roomInfo.Name);
        }
    }
    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();
    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    public override void OnJoinedRoom()
    {
        AssignManager.instance.gameObject.AddComponent<PhotonView>();


        PhotonNetwork.AutomaticallySyncScene = true;

        occupied = new bool[slotPoints.Length];
        Debug.Log("방 입장 완료: " + PhotonNetwork.CurrentRoom.Name);
        lobbyPanel.SetActive(false);
        hands.SetActive(false);
        waitingPanel.SetActive(true);

        if (PhotonNetwork.IsMasterClient && profileOrder == null)
        {
            // 0,1,2,3,4를 중복 없이 랜덤 섞어서 profileOrder에 저장
            profileOrder = MakeRandomValues(5, 5);
            nextProfilePointer = 0;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            AssignProfileIndex(PhotonNetwork.LocalPlayer);
        }


        SpawnSlot(PhotonNetwork.LocalPlayer, 0, animate: true);

        var others = PhotonNetwork.PlayerList.Where(p => p != PhotonNetwork.LocalPlayer).ToArray();
        int count = 1;
        foreach (var p in others)
        {
            int idx = GetNextFreeIndex(count++);
            SpawnSlot(p, idx, animate: false);
        }


        //ChatInput.text = "";
        //for (int i = 0; i < ChatText.Length; i++) ChatText[i].text = "";
    }

    //public override void OnCreateRoomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); } //방 생성

    // public override void OnJoinRandomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); } //랜덤 참가

    public override void OnPlayerEnteredRoom(RealtimePlayer newPlayer) //방 참가 (포톤에서 자동으로 처리)
    {

        int idx = GetNextFreeIndex(0);
        SpawnSlot(newPlayer, idx, animate: true);

        if (PhotonNetwork.IsMasterClient)
        {
            AssignProfileIndex(newPlayer);
        }
        RoomRenewal();  //채팅 메시지
    }

    public override void OnPlayerLeftRoom(RealtimePlayer otherPlayer)
    {
        //RoomRenewal();
        //ChatRPC("<color=yellow>" + otherPlayer.NickName + "님이 퇴장하셨습니다</color>");
        //RemoveSlot(otherPlayer);
    }
    public override void OnPlayerPropertiesUpdate(RealtimePlayer target, PHashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(target, changedProps);
        PropertiesAction?.Invoke(target, changedProps);
    }

    private void HandleReadyChanged(RealtimePlayer target, PHashtable changedProps)
    {
        if (changedProps.ContainsKey("Ready") && slotMap.TryGetValue(target, out var slot))
        {
            slot.SetReadyState((bool)changedProps["Ready"]);
            TryAssignRoles();
        }
    }

    private void HandleProfileIndexChanged(RealtimePlayer target, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("ProfileIndex") && slotMap.TryGetValue(target, out var slot))
        {
            Debug.Log($"[{target.NickName}] 프로필 동기화");
            int idx = (int)changedProps["ProfileIndex"];
            _profileMap[target.ActorNumber] = idx;
            slot.SetProfileImage(idx);
        }
    }

    private void HandleRoleChanged(RealtimePlayer target, Hashtable changedProps)
    {
        if (target == PhotonNetwork.LocalPlayer && changedProps.ContainsKey("Role"))
        {
            ShowRolePanel();
        }
    }
    private void HandleRoleConfirmed(RealtimePlayer target, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("RoleConfirmed") && (bool)changedProps["RoleConfirmed"])
        {
            TryStartGame(); // 모든 플레이어가 RoleConfirmed면 MergeScene으로
        }
    }

    private void TryStartGame()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        bool allConfirmed = PhotonNetwork.PlayerList.All(p =>
            p.CustomProperties.TryGetValue("RoleConfirmed", out var c) && (bool)c
        );

        if (allConfirmed)
        {
            PhotonNetwork.LoadLevel("MergeScene");
            CleanHandler();
        }
    }


    //public override void OnLeftRoom()
    //{
    //    // 방 나가기 시 로비로 돌아가기
    //    ClearSlots();
    //    waitingPanel.SetActive(false);
    //    lobbyPanel.SetActive(true);
    //}

    private void SpawnSlot(RealtimePlayer p, int index, bool animate)
    {
        if (slotMap.ContainsKey(p)) return;
        if (occupied[index]) return;

        var go = Instantiate(playerSlotPrefab, slotPoints[index], false);
        var slot = go.GetComponent<PlayerSlot>();

        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;

        slot.Initialize(p, index);

        slotMap[p] = slot;
        occupied[index] = true;

        if (animate)
            slot.PlayDropAnimation();

        if (p == PhotonNetwork.LocalPlayer)
            slot.BindReadyButton();
    }

    //private void RemoveSlot(RealtimePlayer p)
    //{
    //    if (!slotMap.TryGetValue(p, out var slot)) return;
    //    occupied[slot.SlotIndex] = false;
    //    Destroy(slot.gameObject);
    //    slotMap.Remove(p);
    //}

    private void ClearSlots()
    {
        foreach (var slot in slotMap.Values)
            Destroy(slot.gameObject);
        slotMap.Clear();
        for (int i = 0; i < occupied.Length; i++) occupied[i] = false;
    }

    private int GetNextFreeIndex(int start)
    {
        for (int off = 0; off < occupied.Length; off++)
        {
            int i = (start + off) % occupied.Length;
            if (!occupied[i]) return i;
        }
        return 0;
    }
    private void TryAssignRoles()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        bool allReady = PhotonNetwork.PlayerList.All(p =>
            p.CustomProperties.TryGetValue("Ready", out var v) && (bool)v);

        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2 && allReady)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            if (PhotonNetwork.CurrentRoom.IsOpen == false)
                Debug.Log("이제 방에 못들어감");

            if (PhotonNetwork.CurrentRoom.IsVisible == false)
                Debug.Log("이제 방이 안보임");

            AssignManager.instance.StartGame();
        }
    }

    void RoomRenewal()
    {
        //ListText.text = "";
        //for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        //    ListText.text += PhotonNetwork.PlayerList[i].NickName + ((i + 1 == PhotonNetwork.PlayerList.Length) ? "" : ", ");
        //RoomInfoText.text = PhotonNetwork.CurrentRoom.Name + " / " + PhotonNetwork.CurrentRoom.PlayerCount + "명 / " + PhotonNetwork.CurrentRoom.MaxPlayers + "최대";
    }

    private void AssignProfileIndex(RealtimePlayer p)
    {
        if (profileOrder == null || nextProfilePointer >= profileOrder.Count)
            return;

        int idx = profileOrder[nextProfilePointer++];
        var cp = new ExitGames.Client.Photon.Hashtable { { "ProfileIndex", idx } };
        p.SetCustomProperties(cp);
        HandleProfileIndexChanged(p, cp);
    }

    private List<int> MakeRandomValues(int count, int maxValue) // count: 생성할 랜덤 값의 개수, maxValue: 랜덤 값의 최대값+1
    {
        HashSet<int> randomValues = new HashSet<int>();
        while (randomValues.Count < count)
        {
            int randomValue = UnityEngine.Random.Range(0, maxValue);
            randomValues.Add(randomValue);
        }
        return new List<int>(randomValues);
    }

    public void ShowRolePanel()
    {
        waitingPanel.SetActive(false);
        if (!PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Role", out var roleObj))
        {
            Debug.LogWarning("❗ Role 값이 아직 설정되지 않았습니다.");
            return;
        }

        if (NetworkProperties.instance == null)
        {
            Debug.LogError("❌ NetworkProperties.instance 가 null입니다.");
            return;
        }

        bool isBoss = NetworkProperties.instance.GetMonsterStates((string)roleObj);

        int profileIndex = PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("ProfileIndex", out var pIndex)
                       ? (int)pIndex : 0;

        if (isBoss)
        {
            if (BossPanel == null)
            {
                Debug.LogError("❌ BossPanel 이 null입니다.");
                return;
            }

            BossPanel.SetActive(true);
            var mgr = BossPanel.GetComponent<BossSelectionManager>();
            if (mgr == null)
            {
                Debug.LogError("❌ BossSelectionManager가 BossPanel에 없습니다.");
                return;
            }
            mgr.SetProfileImage(profileIndex);
        }
        else
        {
            if (RunnerPanel == null)
            {
                Debug.LogError("❌ RunnerPanel 이 null입니다.");
                return;
            }

            RunnerPanel.SetActive(true);
            string characterType = (string)roleObj;
            int profileIndexs = profileIndex;

            var mgr = RunnerPanel.GetComponent<RoleSelectionManager>();
            if (mgr == null)
            {
                Debug.LogError("❌ RoleSelectionManager가 RunnerPanel에 없습니다.");
                return;
            }
            mgr.SetProfileInfo(profileIndexs, characterType);
        }
    }


    public void WaitUntilAllInitProperties(Action onReady)
    {
        StartCoroutine(WaitUntilAllPropertiesAssignedCoroutine(onReady));
    }

    private IEnumerator WaitUntilAllPropertiesAssignedCoroutine(Action onReady)
    {
        float timeout = 5f;
        float elapsed = 0f;

        while (elapsed < timeout)
        {
            var props = PhotonNetwork.LocalPlayer.CustomProperties;

            bool hasRole = props.TryGetValue("Role", out _);
            bool hasProfile = props.TryGetValue("ProfileIndex", out _);

            if (hasRole && hasProfile)
            {
                Debug.Log("✅ 모든 초기 속성 수신 완료");
                onReady?.Invoke();  // 모든 조건 만족 시 콜백 실행
                yield break;
            }

            if (!hasRole) Debug.Log("⏳ Role 대기 중...");
            if (!hasProfile) Debug.Log("⏳ ProfileIndex 대기 중...");

            elapsed += Time.deltaTime;
            yield return null;
        }

        Debug.LogWarning("⛔ 초기 속성이 제한 시간 내에 도착하지 않음");
        yield break;
    }


    #endregion


    #region 채팅
    public void Send()
    {
        PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text);
        ChatInput.text = "";
    }

    [PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다
    void ChatRPC(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++)
            if (ChatText[i].text == "")
            {
                isInput = true;
                ChatText[i].text = msg;
                break;
            }
        if (!isInput) // 꽉차면 한칸씩 위로 올림
        {
            for (int i = 1; i < ChatText.Length; i++) ChatText[i - 1].text = ChatText[i].text;
            ChatText[ChatText.Length - 1].text = msg;
        }
    }
    #endregion

    public void Test(string _InputField)
    {
        NickNameInput.text = _InputField;

        WelcomeText = NickNameInput;
    }

    public void GetContent(Transform _content)
    {
        content = _content;
    }

    public RoomInfo GetRoomByName(string roomName)
    {
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogError("Room name is null or empty");
            return null;  // 또는 적절한 처리
        }
        if (roomDic.TryGetValue(roomName, out GameObject roomObject))
        {
            return roomInfoDict[roomName];
        }
        else
        {
            Debug.LogWarning($"Room {roomName} not found");
            return null;
        }
    }

    int keyCheck = 0;

    public Action<string> test;
    public Action<Transform> Gc;


    public List<int> keyManager = new List<int>();

    public Dictionary<String, GameObject> roomDic = new Dictionary<String, GameObject>();
    private Dictionary<string, RoomInfo> roomInfoDict = new Dictionary<string, RoomInfo>();

}