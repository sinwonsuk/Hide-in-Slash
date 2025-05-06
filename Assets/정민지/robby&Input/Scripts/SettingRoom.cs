using TMPro;
using UnityEngine;

public class SettingRoom : MonoBehaviour
{
    public TMP_InputField roomNameInput; //방이름 입력창
    public TMP_InputField roomPasswordInput; //방이름 입력창
    public GameObject popupPanel; // 방 생성창
    public GameObject roomPanelPrefab; //생성할 방

    [Header("경고창")]
    [SerializeField] private GameObject nicknameIsNull;
    [SerializeField] private GameObject lengthExceeded;

    private int warning = 0;

    private GameObject currentWarning;  // 경고창 인스턴스를 저장할 변수

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Return)&& gameObject.activeSelf)
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

    public void OnClickCancel()
    {
        popupPanel.SetActive(false);
    }

    public void AddRoom()
    {
        Time.timeScale = 1;
        string roomName = roomNameInput.text;
        string password = roomPasswordInput.text;

        if (!string.IsNullOrEmpty(roomName))
        {
            if (roomName.Length < 14)
            {

                GameReadyManager.Instance.CreateRoomWithPassword(roomName,password);
                popupPanel.SetActive(false); // 팝업 닫기
                roomNameInput.text = "";
            }
            else
            {
                if (currentWarning == null)
                {
                    currentWarning = Instantiate(lengthExceeded); // 현재 경고 = 길이 초과
                }
                warning = 2;
            }


            if (!string.IsNullOrEmpty(password))
            {

                GameReadyManager.Instance.CreateRoomWithOutPassword(roomName);
                popupPanel.SetActive(false); // 팝업 닫기
                roomNameInput.text = "";
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
