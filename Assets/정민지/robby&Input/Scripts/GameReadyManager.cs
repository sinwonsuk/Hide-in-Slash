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


public class GameReadyManager : MonoBehaviourPunCallbacks
{

    [Header("�г���")]
   // public GameObject loginchang;
    public TMP_InputField NickNameInput; // Ŭ���� ��� ������ ����

    [Header("�κ��ǳ�")]
    public GameObject LobbyPanel;
    public Text WelcomeText;
    public Text LobbyInfoText;
    public Button[] CellBtn;
    public Button PreviousBtn;
    public Button NextBtn;

    [Header("����")]
    public Text ListText;
    public Text RoomInfoText; //�� �ο���
    public Text[] ChatText;
    public TMP_InputField ChatInput;
    public GameObject roomButtonPrefab; //������ ��
    private Transform content; //��ũ�� ����Ʈ
    public Text totalPlayersText;  // �ο����� ǥ���� �ؽ�Ʈ UI (��ü �ο���)

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


    #region �渮��Ʈ ����



    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // ���� �� ����� �ʱ�ȭ
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);  // ������ ������ ��ư���� ����
        }

        // ���ŵ� �� ����Ʈ�� ������� UI ������Ʈ
        foreach (RoomInfo room in roomList)
        {
            if (!room.RemovedFromList)  // ���ŵ� ���� ����
            {
                myList.Add(room);  // �� ��Ͽ� �߰�

                // ��ư�� �����ϰ� �ؽ�Ʈ�� �ش� �� �̸����� ����
                GameObject newButton = Instantiate(roomButtonPrefab, content);
                newButton.transform.SetParent(content, false); // false�� �� ����!
                newButton.GetComponentInChildren<Text>().text = room.Name;  // �� �̸� ����

                // �ο��� �ؽ�Ʈ �߰�
                int currentPlayerCount = room.PlayerCount;  // ���� �ο���
                int maxPlayers = room.MaxPlayers;           // �ִ� �ο���
                newButton.transform.GetChild(0).GetComponent<Text>().text = $"{currentPlayerCount}/{maxPlayers}";  // �ο��� �ؽ�Ʈ ����

            }
        }
    }
    #endregion


        #region ��������


    public void Connect() => PhotonNetwork.ConnectUsingSettings(); //����

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby(); //���ǿ� ����

    public override void OnJoinedLobby() //�κ�� ����
    {
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + "�� ȯ���մϴ�");
        OnRoomListUpdate(myList);
    }

    public void Disconnect() => PhotonNetwork.Disconnect(); //���� ���� ����

    #endregion


    #region ��

    //����� �Բ� �� ����
    public void CreateRoomWithPassword(string roomName, string password)
    {
        if(PhotonNetwork.IsConnected&&PhotonNetwork.InLobby)
        {
            ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
            roomProperties.Add("pw", password); // "pw"�� Ű, password�� ��

            RoomOptions roomOptions = new RoomOptions(); //���ο� �� �ɼ�

            roomOptions.MaxPlayers = 5; //�ɼǿ��� �ִ��ο�
            roomOptions.CustomRoomProperties = roomProperties; //�������
            PhotonNetwork.CreateRoom(roomName, roomOptions); //�������� �� ����
            Debug.Log($"{PhotonNetwork.LocalPlayer.NickName}���� {roomName}�̶�� ���� �����ϼ̽��ϴ�! pw : {password}");
        }
        else
        {
            Debug.Log("���� ���ӵ��� ����");
        }

    }

    public void TryJoinRoom(RoomInfo roomInfo, string inputPassword)
    {
        string roomPassword = (string)roomInfo.CustomProperties["pw"];

        if (roomPassword == inputPassword)
        {
            PhotonNetwork.JoinRoom(roomInfo.Name);
        }
        else
        {
            Debug.Log("��й�ȣ�� Ʋ�Ƚ��ϴ�!");
        }
    }

    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    public override void OnJoinedRoom()
    {
        //RoomRenewal();
        //ChatInput.text = "";
        //for (int i = 0; i < ChatText.Length; i++) ChatText[i].text = "";
    }

    //public override void OnCreateRoomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); } //�� ����

   // public override void OnJoinRandomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); } //���� ����

    public override void OnPlayerEnteredRoom(RealtimePlayer newPlayer) //�� ���� (���濡�� �ڵ����� ó��)
    {
        RoomRenewal(); //�� ���� ������
        ChatRPC("<color=yellow>" + newPlayer.NickName + "���� �����ϼ̽��ϴ�</color>");
    }

    public override void OnPlayerLeftRoom(RealtimePlayer otherPlayer)
    {
        RoomRenewal();
        ChatRPC("<color=yellow>" + otherPlayer.NickName + "���� �����ϼ̽��ϴ�</color>");
    }

    void RoomRenewal()
    {
        //ListText.text = "";
        //for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        //    ListText.text += PhotonNetwork.PlayerList[i].NickName + ((i + 1 == PhotonNetwork.PlayerList.Length) ? "" : ", ");
        //RoomInfoText.text = PhotonNetwork.CurrentRoom.Name + " / " + PhotonNetwork.CurrentRoom.PlayerCount + "�� / " + PhotonNetwork.CurrentRoom.MaxPlayers + "�ִ�";
    }
    #endregion


    #region ä��
    public void Send()
    {
        PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text);
        ChatInput.text = "";
    }

    [PunRPC] // RPC�� �÷��̾ �����ִ� �� ��� �ο����� �����Ѵ�
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
        if (!isInput) // ������ ��ĭ�� ���� �ø�
        {
            for (int i = 1; i < ChatText.Length; i++) ChatText[i - 1].text = ChatText[i].text;
            ChatText[ChatText.Length - 1].text = msg;
        }
    }
    #endregion

    public void Test(string _InputField)
    {
        NickNameInput.text = _InputField;
    }

    public void GetContent(Transform _content)
    {
        content = _content;
    }


    public Action<string> test;
    public Action<Transform> Gc;

}
