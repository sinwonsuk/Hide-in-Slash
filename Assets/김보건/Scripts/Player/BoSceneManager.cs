using Photon.Pun;
using UnityEngine;
using System.Collections;

public class BoSceneManager : MonoBehaviourPunCallbacks
{
    public Transform[] playerSpawnPoints;  // �÷��̾� ���� ��ġ

    public ProfileSlotManager profileSlotManager;

    float time = 0;

    void test()
    {
        while(true)
        {
            if (time > 1)
            {
                break;
            }

            time += Time.deltaTime;
        }
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            ExitGames.Client.Photon.Hashtable prop = player.CustomProperties;

            //string name = prop["Role"].ToString();

            if (player != PhotonNetwork.LocalPlayer)
            {
                profileSlotManager.CreateProfileSlot(player);
            }
        }
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable monProp = new();

            monProp.Add("Role", "Monster");
            PhotonNetwork.LocalPlayer.SetCustomProperties(monProp);
        }
        else
        {
            ExitGames.Client.Photon.Hashtable humanProp = new();
            humanProp.Add("Role", "Human");
            PhotonNetwork.LocalPlayer.SetCustomProperties(humanProp);
        }
        //test();

        //foreach (var player in PhotonNetwork.PlayerList)
        //{         
        //    ExitGames.Client.Photon.Hashtable prop = player.CustomProperties;

        //    string name = prop["Role"].ToString();

        //    if (player != PhotonNetwork.LocalPlayer && name != "Monster")
        //    {
        //        profileSlotManager.CreateProfileSlot(player);
        //    }         
        //}

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
            PhotonNetwork.Instantiate("ProteinGhost", spawnPos, playerRotation);
        }
        else if (playerCount == 2)
        {
            PhotonNetwork.Instantiate("ProteinGhost", spawnPos, playerRotation);
        }
        else if (playerCount == 3)
        {
            PhotonNetwork.Instantiate("ProteinGhost", spawnPos, playerRotation);
        }
        else if (playerCount == 4)
        {
            PhotonNetwork.Instantiate("Player2", spawnPos, playerRotation);
        }
        else if (playerCount == 5)
        {
            PhotonNetwork.Instantiate("Player2", spawnPos, playerRotation);
        }
        else if (playerCount == 6)
        {
            PhotonNetwork.Instantiate("Player2", spawnPos, playerRotation);
        }
        else if (playerCount == 7)
        {
            PhotonNetwork.Instantiate("Player2", spawnPos, playerRotation);
        }
        else if (playerCount == 8)
        {
            PhotonNetwork.Instantiate("Player2", spawnPos, playerRotation);
        }
        else if (playerCount == 9)
        {
            PhotonNetwork.Instantiate("Player2", spawnPos, playerRotation);
        }
    }
}
