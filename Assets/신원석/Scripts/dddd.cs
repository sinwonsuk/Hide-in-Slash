using Photon.Chat.UtilityScripts;
using Photon.Pun;
using UnityEngine;

public class BoScenesManager : MonoBehaviourPunCallbacks
{
    public Transform[] playerSpawnPoints;  // 플레이어 스폰 위치

    public override void OnJoinedRoom()
    {
        //foreach (var player in PhotonNetwork.PlayerList)
        //{
        //    if (player != PhotonNetwork.LocalPlayer)
        //    {
        //        profileSlotManager.CreateProfileSlot(player);
        //    }
        //}

        Debug.Log("OnJoinedRoom 호출됨!");
        if (playerSpawnPoints == null || playerSpawnPoints.Length == 0)
        {
            Debug.LogError("스폰 포인트가 할당되지 않았습니다!");
            return;
        }

        int playerIndex = PhotonNetwork.CurrentRoom.PlayerCount - 1;
        Quaternion playerRotation = Quaternion.identity;

        if (playerIndex >= playerSpawnPoints.Length)
        {
            Debug.LogWarning("스폰 포인트 부족, 기본 위치에 스폰.");
            playerIndex = 0; // 기본값
        }

        Vector3 spawnPos = playerSpawnPoints[playerIndex].position;

        // 
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("Player", spawnPos, playerRotation);

            PhotonNetwork.Instantiate("generator1", spawnPos, playerRotation);
        }
        else
        {
            PhotonNetwork.Instantiate("Player", spawnPos, playerRotation);
        }




        Debug.Log($" 현재 방 이름: {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($" 내 닉네임: {PhotonNetwork.NickName}");
        Debug.Log($" 내 ActorNumber (플레이어 ID): {PhotonNetwork.LocalPlayer.ActorNumber}");
        Debug.Log($" 현재 방 접속 인원 수: {PhotonNetwork.CurrentRoom.PlayerCount}");
    }

    [SerializeField]
    ProfileSlotManager profileSlotManager;
}

