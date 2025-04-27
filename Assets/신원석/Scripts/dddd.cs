using Photon.Chat.UtilityScripts;
using Photon.Pun;
using UnityEngine;

public class BoScenesManager : MonoBehaviourPunCallbacks
{
    public Transform[] playerSpawnPoints;  // �÷��̾� ���� ��ġ

    public override void OnJoinedRoom()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player != PhotonNetwork.LocalPlayer)
            {
                profileSlotManager.CreateProfileSlot(player);
            }
        }

        Debug.Log("OnJoinedRoom ȣ���!");
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
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("Player", spawnPos, playerRotation);

            PhotonNetwork.Instantiate("generator1", spawnPos, playerRotation);
        }
        else
        {
            PhotonNetwork.Instantiate("Player", spawnPos, playerRotation);
        }




        Debug.Log($" ���� �� �̸�: {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($" �� �г���: {PhotonNetwork.NickName}");
        Debug.Log($" �� ActorNumber (�÷��̾� ID): {PhotonNetwork.LocalPlayer.ActorNumber}");
        Debug.Log($" ���� �� ���� �ο� ��: {PhotonNetwork.CurrentRoom.PlayerCount}");
    }

    [SerializeField]
    ProfileSlotManager profileSlotManager;
}

