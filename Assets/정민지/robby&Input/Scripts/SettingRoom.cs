using TMPro;
using UnityEngine;

public class SettingRoom : MonoBehaviour
{
    public TMP_InputField roomNameInput; //���̸� �Է�â
    public TMP_InputField passwordInput; //���� �Է�â
    public Transform roomListContent; //��ũ�� ����Ʈ
    public GameObject roomPanelPrefab; //������ ��
    public GameObject popupPanel; // �� ����â
    private int numberOfPlayers; //�÷��̾� �ο�

    [Header("���â")]
    [SerializeField] private GameObject nicknameIsNull;
    [SerializeField] private GameObject lengthExceeded;

    private int warning = 0;

    private GameObject currentWarning;  // ���â �ν��Ͻ��� ������ ����

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Return))
            AddRoom();

        if (Input.GetKeyDown(KeyCode.Escape) && warning == 0)
        {
            Time.timeScale = 1;
            popupPanel.SetActive(false); // �˾� �ݱ�
        }


        if ((Input.GetKeyUp(KeyCode.Escape)) && (warning == 1 || warning == 2))
        {
            if (currentWarning != null)
            {
                Destroy(currentWarning);
                Time.timeScale = 0;
            }

            warning = 0;
        }
    }


    public void OnClickConfirm()
    {
        AddRoom();
    }

    public void OnClickCancel()
    {
        popupPanel.SetActive(false);
    }

    public void AddRoom()
    {
        string roomName = roomNameInput.text;
        string password = passwordInput.text;

        if (!string.IsNullOrEmpty(roomName) && !string.IsNullOrEmpty(password))
        {
            if (roomName.Length < 14 && password.Length < 6)
            {
                GameObject room = Instantiate(roomPanelPrefab, roomListContent);

                Transform roomNameObj = room.transform.Find("RoomNameText");
                Transform roomplayerObj = room.transform.Find("player");

                numberOfPlayers = 1;

                if (roomNameObj != null)
                {
                    roomNameObj.GetComponent<TMP_Text>().text = roomName; //���̸�
                    roomplayerObj.GetComponent<TMP_Text>().text = ($"{numberOfPlayers}/5");
                }
                else
                {
                    Debug.LogError("RoomNameText �Ǵ� PasswordText ������Ʈ�� �����տ��� ã�� �� �����ϴ�.");
                }

                popupPanel.SetActive(false); // �˾� �ݱ�
                roomNameInput.text = "";
                passwordInput.text = "";
            }
            else
            {
                if (currentWarning == null)
                {
                    currentWarning = Instantiate(lengthExceeded); // ���� ��� = ���� �ʰ�
                }
                warning = 2;
            }

        }
        else
        {
            warning = 1;
            if (currentWarning == null)
                currentWarning = Instantiate(nicknameIsNull); // ���� ��� = �ؽ�Ʈ ��
        }
    }

}
