using Photon.Pun;
using UnityEngine;
using System.Collections;
using Photon.Realtime;

public class BoSceneManager : MonoBehaviourPunCallbacks
{
    public Transform[] playerSpawnPoints;  // �÷��̾� ���� ��ġ

    public ProfileSlotManager profileSlotManager;

    [SerializeField]
    Canvas playerCanvas;

    [SerializeField]
    Canvas bossCanvas;

    float time = 0;

    void test()
    {
        while (true)
        {
            if (time > 1)
            {
                break;
            }

            time += Time.deltaTime;
        }
    }

    //public override void OnJoinedRoom()
    //{
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        SetRole("Boss");
    //    }
    //    else
    //    {
    //        SetRole("Human");
    //    }

    //    StartCoroutine(SetupProfileSlotsAndSpawn());
    //}

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

        string name;

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Role", out object selfRoleObj))
        {
            name = selfRoleObj.ToString();

            if(name == "Boss")
            {
                bossCanvas.gameObject.SetActive(true);
                playerCanvas.gameObject.SetActive(false);
            }
            else
            {
                bossCanvas.gameObject.SetActive(false);
                playerCanvas.gameObject.SetActive(true);
            }
        }

        profileSlotManager.CreateProfileSlot();



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
            case 1: return "PeanutGhost";
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