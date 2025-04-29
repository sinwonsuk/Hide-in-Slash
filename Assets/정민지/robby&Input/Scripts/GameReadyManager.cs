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
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using System.Linq;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;

public class GameReadyManager : MonoBehaviourPunCallbacks
{

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

    [Header("Slot UI")]
    [SerializeField] private RectTransform[] slotPoints; // length=5
    [SerializeField] private GameObject playerSlotPrefab;

    [Header("Role Assignment")]
    public GameObject assignManager;

    [Header("Other UI")]
    [SerializeField] private Button leaveButton;

    private Dictionary<Photon.Realtime.Player, PlayerSlot> slotMap = new Dictionary<Photon.Realtime.Player, PlayerSlot>();
    private bool[] occupied = new bool[5];

    int count = 1;

    public static GameReadyManager Instance { get; private set; }

    private void Awake()
    {
        Screen.SetResolution(960, 540, false);

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;

            // Corrected the assignment to avoid modifying the read-only 'gameObject' property  
            AssignManager.instance = Instantiate(assignManager).GetComponent<AssignManager>();
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
        leaveButton = GameObject.Find("Exit").GetComponent<Button>();
        playerSlotPrefab = Resources.Load<GameObject>("PlayerSlot");

        slotPoints = new RectTransform[5];
        var waitingUI = GameObject.Find("WaitingUI");
        if (waitingUI == null)
            Debug.LogError("❌ WaitingUI 오브젝트를 찾을 수 없습니다!");
        else
        {
            var roomUI = waitingUI.transform.Find("RoomUI");
            if (roomUI == null)
                Debug.LogError("❌ WaitingUI/RoomUI 경로를 찾을 수 없습니다!");
            else
            {
                var slotContainer = roomUI.Find("SlotContainer");
                if (slotContainer == null)
                    Debug.LogError("❌ WaitingUI/RoomUI/SlotContainer 를 찾을 수 없습니다!");
                else
                {
                    // 3) SlotPoint0~4 채우기
                    slotPoints = new RectTransform[5];
                    for (int i = 0; i < slotPoints.Length; i++)
                    {
                        var p = slotContainer.Find($"SlotPoint{i}");
                        if (p == null)
                            Debug.LogError($"❌ SlotPoint{i} 를 찾을 수 없습니다!");
                        else
                            slotPoints[i] = p.GetComponent<RectTransform>();
                    }
                }
            }
        }

        // 초기 UI 상태
        lobbyPanel.SetActive(true);
        hands.SetActive(true);
        waitingPanel.SetActive(false);

        // leaveButton 클릭 리스너
        //leaveButton.onClick.RemoveAllListeners();
        //leaveButton.onClick.AddListener(OnLeaveClicked);
    }


    #region 방리스트 갱신



    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if(roomList.Count == 0)
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
                if (roomDic.ContainsKey(room))
                {
                    int currentPlayerCount = room.PlayerCount;  // 현재 인원수
                    int maxPlayers = room.MaxPlayers;           // 최대 인원수

                    roomDic[room].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{currentPlayerCount}/{maxPlayers}";  // 인원수 텍스트 설정
                    roomDic[room].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = room.Name;

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
                        PhotonNetwork.JoinRoom(capturedRoomName);
                    });

                    roomDic.Add(room, newButton);
                }
                                      
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
    public void CreateRoomWithPassword(string roomName)
    {
         if(PhotonNetwork.IsConnected&&PhotonNetwork.InLobby)
        {

            RoomOptions roomOptions = new RoomOptions(); //새로운 룸 옵션

            roomOptions.MaxPlayers = 5; //옵션에서 최대인원
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

    //public void TryJoinRoom(RoomInfo roomInfo, string inputPassword)
    //{
    //    string roomPassword = (string)roomInfo.CustomProperties["pw"];

    //    if (roomPassword == inputPassword)
    //    {
    //        PhotonNetwork.JoinRoom(roomInfo.Name);
    //    }
    //    else
    //    {
    //        Debug.Log("비밀번호가 틀렸습니다!");
    //    }
    //}

    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    public override void OnJoinedRoom()
    {
        occupied = new bool[slotPoints.Length];
        Debug.Log("방 입장 완료: " + PhotonNetwork.CurrentRoom.Name);
        lobbyPanel.SetActive(false);
        hands.SetActive(false);
        waitingPanel.SetActive(true);
        RoomRenewal();
        SpawnAllSlots();
        //ChatInput.text = "";
        //for (int i = 0; i < ChatText.Length; i++) ChatText[i].text = "";
    }

    //public override void OnCreateRoomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); } //방 생성

   // public override void OnJoinRandomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); } //랜덤 참가

    public override void OnPlayerEnteredRoom(RealtimePlayer newPlayer) //방 참가 (포톤에서 자동으로 처리)
    {

        int idx = GetNextFreeIndex(count++);
        SpawnSlot(newPlayer, idx, animate: true);
    }

    public override void OnPlayerLeftRoom(RealtimePlayer otherPlayer)
    {
        //RoomRenewal();
        //ChatRPC("<color=yellow>" + otherPlayer.NickName + "님이 퇴장하셨습니다</color>");

        RemoveSlot(otherPlayer);
    }
    public override void OnPlayerPropertiesUpdate(RealtimePlayer target, PHashtable changedProps)
    {
        if (changedProps.ContainsKey("Ready") &&
            slotMap.TryGetValue(target, out var slot))
        {
            slot.SetReadyState((bool)changedProps["Ready"]);
            TryAssignRoles();
        }
    }

    public override void OnLeftRoom()
    {
        // 방 나가기 시 로비로 돌아가기
        ClearSlots();
        waitingPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    private void SpawnAllSlots()
    {
        // 로컬 플레이어는 0번 슬롯
        SpawnSlot(PhotonNetwork.LocalPlayer, 0, false);

        // 나머지는 빈 자리 찾아서
        var others = PhotonNetwork.PlayerList.Where(p => p != PhotonNetwork.LocalPlayer);

        foreach (var p in others)
        {
            int idx = GetNextFreeIndex(count++);
            SpawnSlot(p, idx, false);
        }
    }

    private void SpawnSlot(RealtimePlayer p, int index, bool animate)
    {
        if (slotMap.ContainsKey(p)) return;

        if (occupied[index]) return;

        var go = Instantiate(playerSlotPrefab, slotPoints[index], false);
        go.transform.SetAsLastSibling();

        var slot = go.GetComponent<PlayerSlot>();
        slot.Initialize(p, index);
        slot.SetReadyState(false);

        // 로컬 슬롯이면 버튼 바인딩
        if (p.IsLocal)
            slot.BindReadyButton();

        slotMap[p] = slot;
        occupied[index] = true;

        if (animate) slot.PlayDropAnimation();
    }

    private void RemoveSlot(RealtimePlayer p)
    {
        if (!slotMap.TryGetValue(p, out var slot)) return;
        occupied[slot.SlotIndex] = false;
        Destroy(slot.gameObject);
        slotMap.Remove(p);
    }

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
        if (PhotonNetwork.CurrentRoom.PlayerCount == 5 && allReady)
        {
            assignManager.GetComponent<AssignManager>().StartGame();
        }
    }

    void RoomRenewal()
    {
        //ListText.text = "";
        //for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        //    ListText.text += PhotonNetwork.PlayerList[i].NickName + ((i + 1 == PhotonNetwork.PlayerList.Length) ? "" : ", ");
        //RoomInfoText.text = PhotonNetwork.CurrentRoom.Name + " / " + PhotonNetwork.CurrentRoom.PlayerCount + "명 / " + PhotonNetwork.CurrentRoom.MaxPlayers + "최대";
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

    int keyCheck = 0;

    public Action<string> test;
    public Action<Transform> Gc;

    public List<int> keyManager = new List<int>();

    public Dictionary<RoomInfo, GameObject> roomDic = new Dictionary<RoomInfo, GameObject>();

}
