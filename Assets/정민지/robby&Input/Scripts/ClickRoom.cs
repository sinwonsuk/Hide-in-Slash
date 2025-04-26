using UnityEngine;

public class ClickRoom : MonoBehaviour
{
    public static string selectedRoomName;
    public GameObject passwordPanel;

    public void OnRoomButtonClicked(string roomName)
    {
        selectedRoomName = roomName;
        passwordPanel.SetActive(true); // 비밀번호 입력창 열기
    }
}
