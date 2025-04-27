using Photon.Pun;
using UnityEngine;
using System.Collections;

public class BoSceneManager : MonoBehaviourPunCallbacks
{
    public Transform[] playerSpawnPoints;  // 플레이어 스폰 위치

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

        Debug.Log(" OnJoinedRoom 호출됨!");
        Debug.Log($" 현재 방 이름: {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($" 내 닉네임: {PhotonNetwork.NickName}");
        Debug.Log($" 내 ActorNumber (플레이어 ID): {PhotonNetwork.LocalPlayer.ActorNumber}");
        Debug.Log($" 현재 방 접속 인원 수: {PhotonNetwork.CurrentRoom.PlayerCount}");
 
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

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("ProteinGhost", spawnPos, playerRotation);
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
