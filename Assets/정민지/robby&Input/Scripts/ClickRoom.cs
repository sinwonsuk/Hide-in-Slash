using UnityEngine;

public class ClickRoom : MonoBehaviour
{
    public static string selectedRoomName;
    public GameObject passwordPanel;

    public void OnRoomButtonClicked(string roomName)
    {
        selectedRoomName = roomName;
        passwordPanel.SetActive(true); // ��й�ȣ �Է�â ����
    }
}
