using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using RealtimePlayer = Photon.Realtime.Player;
using TMPro;
using UnityEngine.Windows;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using System.Linq;


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

    [Header("ETC")]
    public Text StatusText;
    public PhotonView PV;


    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;

    public static GameReadyManager Instance { get; private set; }

    private void Awake()
    {


        Screen.SetResolution(960, 540, false);

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        test = Test;
        Gc = GetContent;
    }

    //void Start()
    //{
    //    NickNameInput = loginchang.GetComponentInChildren<TMP_InputField>();
    //}


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
        //RoomRenewal();
        //ChatInput.text = "";
        //for (int i = 0; i < ChatText.Length; i++) ChatText[i].text = "";
        PhotonNetwork.LoadLevel("WaitingRoom");
    }

    //public override void OnCreateRoomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); } //방 생성

   // public override void OnJoinRandomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); } //랜덤 참가

    public override void OnPlayerEnteredRoom(RealtimePlayer newPlayer) //방 참가 (포톤에서 자동으로 처리)
    {
        RoomRenewal(); //방 정보 리뉴얼
        ChatRPC("<color=yellow>" + newPlayer.NickName + "님이 참가하셨습니다</color>");
    }

    public override void OnPlayerLeftRoom(RealtimePlayer otherPlayer)
    {
        RoomRenewal();
        ChatRPC("<color=yellow>" + otherPlayer.NickName + "님이 퇴장하셨습니다</color>");
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
