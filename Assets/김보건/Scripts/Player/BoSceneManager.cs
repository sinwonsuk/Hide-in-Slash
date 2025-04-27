using Photon.Pun;
using UnityEngine;
using System.Collections;

public class BoSceneManager : MonoBehaviourPunCallbacks
{
    public Transform[] playerSpawnPoints;  // �÷��̾� ���� ��ġ

    public override void OnJoinedRoom()
    {


        Debug.Log(" OnJoinedRoom ȣ���!");
        Debug.Log($" ���� �� �̸�: {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($" �� �г���: {PhotonNetwork.NickName}");
        Debug.Log($" �� ActorNumber (�÷��̾� ID): {PhotonNetwork.LocalPlayer.ActorNumber}");
        Debug.Log($" ���� �� ���� �ο� ��: {PhotonNetwork.CurrentRoom.PlayerCount}");
 
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

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("Player", spawnPos, playerRotation);
        }
        else if (playerCount == 2)
        {
            PhotonNetwork.Instantiate("Player2", spawnPos, playerRotation);
        }
        else if (playerCount == 3)
        {
            PhotonNetwork.Instantiate("Player3", spawnPos, playerRotation);
        }
        else if (playerCount == 4)
        {
            PhotonNetwork.Instantiate("ProteinGhost", spawnPos, playerRotation);
        }
        else if (playerCount == 5)
        {
            PhotonNetwork.Instantiate("ProteinGhost", spawnPos, playerRotation);
        }
        else if (playerCount == 6)
        {
            PhotonNetwork.Instantiate("ProteinGhost", spawnPos, playerRotation);
        }
        else if (playerCount == 7)
        {
            PhotonNetwork.Instantiate("ProteinGhost", spawnPos, playerRotation);
        }
        else if (playerCount == 8)
        {
            PhotonNetwork.Instantiate("ProteinGhost", spawnPos, playerRotation);
        }
        else if (playerCount == 9)
        {
            PhotonNetwork.Instantiate("ProteinGhost", spawnPos, playerRotation);
        }
    }
}
