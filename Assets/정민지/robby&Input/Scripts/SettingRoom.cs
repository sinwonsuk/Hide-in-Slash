using TMPro;
using UnityEngine;

public class SettingRoom : MonoBehaviour
{
    public TMP_InputField roomNameInput; //방이름 입력창
    public TMP_InputField passwordInput; //방비번 입력창
    public Transform roomListContent; //스크롤 콘텐트
    public GameObject roomPanelPrefab; //생성할 방
    public GameObject popupPanel; // 방 생성창
    private int numberOfPlayers; //플레이어 인원

    [Header("경고창")]
    [SerializeField] private GameObject nicknameIsNull;
    [SerializeField] private GameObject lengthExceeded;

    private int warning = 0;

    private GameObject currentWarning;  // 경고창 인스턴스를 저장할 변수

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Return))
            AddRoom();

        if (Input.GetKeyDown(KeyCode.Escape) && warning == 0)
        {
            Time.timeScale = 1;
            popupPanel.SetActive(false); // 팝업 닫기
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
                    roomNameObj.GetComponent<TMP_Text>().text = roomName; //방이름
                    roomplayerObj.GetComponent<TMP_Text>().text = ($"{numberOfPlayers}/5");
                }
                else
                {
                    Debug.LogError("RoomNameText 또는 PasswordText 오브젝트를 프리팹에서 찾을 수 없습니다.");
                }

                popupPanel.SetActive(false); // 팝업 닫기
                roomNameInput.text = "";
                passwordInput.text = "";
            }
            else
            {
                if (currentWarning == null)
                {
                    currentWarning = Instantiate(lengthExceeded); // 현재 경고 = 길이 초과
                }
                warning = 2;
            }

        }
        else
        {
            warning = 1;
            if (currentWarning == null)
                currentWarning = Instantiate(nicknameIsNull); // 현재 경고 = 텍스트 빔
        }
    }

}
