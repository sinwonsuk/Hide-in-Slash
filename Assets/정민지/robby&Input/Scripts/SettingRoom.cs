using TMPro;
using UnityEngine;

public class SettingRoom : MonoBehaviour
{
    public TMP_InputField roomNameInput; //���̸� �Է�â
    public TMP_InputField passwordInput; //���� �Է�â
    public GameObject popupPanel; // �� ����â
    public GameObject roomPanelPrefab; //������ ��

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

    public void OnClickCancel()
    {
        popupPanel.SetActive(false);
    }

    public void AddRoom()
    {
        Time.timeScale = 1;
        string roomName = roomNameInput.text;
        string password = passwordInput.text; 

        if (!string.IsNullOrEmpty(roomName) && !string.IsNullOrEmpty(password))
        {
            if (roomName.Length < 14 && password.Length < 6)
            {

                    GameReadyManager.Instance.CreateRoomWithPassword(roomName, password);
                    //SceneManager.Instance.LoadSceneAsync(SceneName.WaitingRoom);
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
