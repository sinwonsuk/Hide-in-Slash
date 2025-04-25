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
        Quaternion playerRotation = Quaternion.Euler(0, 0, 0);

        if (playerIndex >= playerSpawnPoints.Length)
        {
            Debug.LogWarning("���� ����Ʈ ����, �⺻ ��ġ�� ����.");
            PhotonNetwork.Instantiate("Player7", Vector3.zero, playerRotation);
        }
        else
        {
            PhotonNetwork.Instantiate("Player7", playerSpawnPoints[playerIndex].position, playerRotation);
        }
    }
}
