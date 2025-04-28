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

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SetRole("Monster");
        }
        else
        {
            SetRole("Human");
        }

        StartCoroutine(SetupProfileSlotsAndSpawn());
    }

    private void SetRole(string role)
    {
        ExitGames.Client.Photon.Hashtable prop = new();
        prop.Add("Role", role);
        PhotonNetwork.LocalPlayer.SetCustomProperties(prop);
    }

    private IEnumerator SetupProfileSlotsAndSpawn()
    {
        // 기다려줘야 할 수도 있음
        yield return new WaitForSeconds(0.5f);

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("Role", out object roleObj))
            {
                string role = roleObj.ToString();
                if (player != PhotonNetwork.LocalPlayer && role != "Monster")
                {
                    profileSlotManager.CreateProfileSlot(player);
                }
            }
        }

        if (playerSpawnPoints == null || playerSpawnPoints.Length == 0)
        {
            Debug.LogError("Spawn points missing!");
            yield break;
        }

        int playerIndex = PhotonNetwork.CurrentRoom.PlayerCount - 1;
        if (playerIndex >= playerSpawnPoints.Length)
        {
            playerIndex = 0;
        }

        Vector3 spawnPos = playerSpawnPoints[playerIndex].position;
        Quaternion playerRotation = Quaternion.identity;

        string prefabName = GetPrefabName(PhotonNetwork.CurrentRoom.PlayerCount);


        if (!string.IsNullOrEmpty(prefabName))
        {
            PhotonNetwork.Instantiate(prefabName, spawnPos, playerRotation);
        }
       
    }

    private string GetPrefabName(int playerCount)
    {
        switch (playerCount)
        {
            case 1: return "ProteinGhost";
            case 2: return "Player";
            case 3: return "Player2";
            case 4: return "Player3";
            case 5: return "Player3";
            case 6: return "Player3";
            case 7: return "Player3";
            case 8:
            case 9: return "Player2";
            default: return null;
        }
    }

}
