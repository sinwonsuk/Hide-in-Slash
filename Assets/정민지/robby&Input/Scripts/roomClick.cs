using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class roomClick : MonoBehaviour
{
    public GameObject passwordPanelPrefab;  // 프리팹 (Hierarchy에 없어야 함)

    private string selectedRoomName;

    // 방 목록 저장
    public static List<RoomInfo> cachedRoomList = new List<RoomInfo>();


    public void OnRoomButtonClicked(string roomName)
    {
        selectedRoomName = roomName;

        // 캐시된 방 목록에서 찾기
        RoomInfo roomInfo = cachedRoomList.FirstOrDefault(r => r.Name == selectedRoomName);

        // 비밀번호가 설정되어 있으면 패널을 띄우고, 없으면 바로 입장
        if (roomInfo != null && roomInfo.CustomProperties.ContainsKey("pw"))
        {
            string roomPassword = (string)roomInfo.CustomProperties["pw"];
            if (!string.IsNullOrEmpty(roomPassword))
            {
                // 비밀번호 입력 패널 인스턴스 생성
                GameObject panelInstance = Instantiate(passwordPanelPrefab);

                // 선택적으로, 패널 스크립트에 방 이름 전달
                var enterRoom = panelInstance.GetComponent<EnterRoom>();
                if (enterRoom != null)
                {
                    enterRoom.SetRoomName(selectedRoomName);
                }
            }
            else
            {
                // 비밀번호가 없으면 바로 입장
                PhotonNetwork.JoinRoom(selectedRoomName);
            }
        }
        else
        {
            // 비밀번호가 없으면 바로 입장
            PhotonNetwork.JoinRoom(selectedRoomName);
        }
    }
}
