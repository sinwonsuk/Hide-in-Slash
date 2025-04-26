using Photon.Pun;
using UnityEngine;
using System.Collections;

public class BoSceneManager : MonoBehaviourPunCallbacks
{
    public Transform[] playerSpawnPoints;  // 플레이어 스폰 위치

    public override void OnJoinedRoom()
    {
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
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.Instantiate("Player", spawnPos, playerRotation);
        }
        else
        {
            PhotonNetwork.Instantiate("PeanutGhost", spawnPos, playerRotation);
        }
    }
}
