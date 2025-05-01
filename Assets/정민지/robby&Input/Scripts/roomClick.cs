using TMPro;
using UnityEngine;

public class roomClick : MonoBehaviour
{
    public GameObject passwordPanelPrefab;  // 프리팹 (Hierarchy에 없어야 함)

    private string selectedRoomName;

    public void OnRoomButtonClicked(string roomName)
    {
        selectedRoomName = roomName;

        // 인스턴스 생성
        GameObject panelInstance = Instantiate(passwordPanelPrefab);

        // 선택적으로, 패널 스크립트에 방 이름 전달
        var enterRoom = panelInstance.GetComponent<EnterRoom>();
        if (enterRoom != null)
        {
            enterRoom.SetRoomName(selectedRoomName);
        }
    }
}
