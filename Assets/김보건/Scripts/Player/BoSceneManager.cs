using Photon.Pun;
using UnityEngine;
using System.Collections;

public class BoSceneManager : MonoBehaviourPunCallbacks
{
    public Transform[] playerSpawnPoints;  // �÷��̾� ���� ��ġ

    public override void OnJoinedRoom()
    {
        if (playerSpawnPoints == null || playerSpawnPoints.Length == 0)
        {
            Debug.LogError("���� ����Ʈ�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }

        int playerIndex = PhotonNetwork.CurrentRoom.PlayerCount - 1;
        Quaternion playerRotation = Quaternion.identity;

        if (playerIndex >= playerSpawnPoints.Length)
        {
            Debug.LogWarning("���� ����Ʈ ����, �⺻ ��ġ�� ����.");
            playerIndex = 0; // �⺻��
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
