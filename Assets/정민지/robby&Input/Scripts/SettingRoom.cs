using TMPro;
using UnityEngine;

public class SettingRoom : MonoBehaviour
{
    public TMP_InputField roomNameInput;
    public TMP_InputField passwordInput;
    public Transform roomListContent;
    public GameObject roomPanelPrefab;
    public GameObject popupPanel; // 팝업 루트 오브젝트

    public void OnClickConfirm()
    {
        string roomName = roomNameInput.text;
        string password = passwordInput.text;

        if (!string.IsNullOrEmpty(roomName) && !string.IsNullOrEmpty(password))
        {
            GameObject room = Instantiate(roomPanelPrefab, roomListContent);

            Transform roomNameObj = room.transform.Find("RoomNameText");

            if (roomNameObj != null)
            {
                roomNameObj.GetComponent<TMP_Text>().text = roomName;
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
            Debug.LogWarning("방 이름과 비밀번호를 입력해주세요.");
        }
    }

    public void OnClickCancel()
    {
        popupPanel.SetActive(false);
    }

}
