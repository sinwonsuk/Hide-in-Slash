using TMPro;
using UnityEngine;

public class SettingRoom : MonoBehaviour
{
    public TMP_InputField roomNameInput;
    public TMP_InputField passwordInput;
    public Transform roomListContent;
    public GameObject roomPanelPrefab;
    public GameObject popupPanel; // �˾� ��Ʈ ������Ʈ

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
                Debug.LogError("RoomNameText �Ǵ� PasswordText ������Ʈ�� �����տ��� ã�� �� �����ϴ�.");
            }

            popupPanel.SetActive(false); // �˾� �ݱ�
            roomNameInput.text = "";
            passwordInput.text = "";
        }
        else
        {
            Debug.LogWarning("�� �̸��� ��й�ȣ�� �Է����ּ���.");
        }
    }

    public void OnClickCancel()
    {
        popupPanel.SetActive(false);
    }

}
