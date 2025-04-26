using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviourPunCallbacks
{
    public List<GameObject> mapObjects;

    private Dictionary<string, GameObject> mapDic = new();
    private List<Transform> eventSpawnPoints = new();
    private List<Transform> playerSpawnPoints = new();
    private List<int> espIndexs = new();
    private List<int> pspIndexs = new();
    private List<int> roleIndexs = new();
    private string[] monTypes = { "Mon1", "Mon2", "Mon3" }; //몬스터 프리팹 이름
    private string[] pTypes = { "P1", "P2", "P3", "P4" }; // 플레이어 프리팹 이름
    private string currentMap;

    void Start()
    {

        PhotonNetwork.ConnectUsingSettings();




    }

    public override void OnConnectedToMaster()
    {
        //PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 6 }, null);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InitializeMap();
        }
        WriteDic();
        AddSpawnPoints();
        if (PhotonNetwork.IsMasterClient)
        {
            espIndexs = MakeRandomValues(10, eventSpawnPoints.Count);

            InitializeMiniGames();
            InitializeGenerators();
            Debug.Log("방에서 마스터할일 완");
        }
    }


    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("총" + PhotonNetwork.PlayerList.Length);
                roleIndexs = MakeRandomValues(PhotonNetwork.PlayerList.Length, PhotonNetwork.PlayerList.Length);
                for (int i = 0; i < roleIndexs.Count; i++)
                {
                    Debug.Log("roleIndexs = " + string.Join(", ", roleIndexs[i]));
                }
                AssignRole();
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {

            InitializePlayers();
        }
    }



    public GameObject GetCurrentMap()
    {
        if (mapDic.ContainsKey(currentMap))
            return mapDic[currentMap];
        return null;
    }

    public GameObject MoveMap(GameObject mapPrefab)
    {
        string mapName = mapPrefab.name;

        currentMap = mapName;
        return mapDic[mapName];
    }


    private void InitializeMap()
    {
        for (int i = 0; i < mapObjects.Count; i++)
        {
            PhotonNetwork.Instantiate(mapObjects[i].name, mapObjects[i].transform.position, Quaternion.identity);
            mapDic.Add(mapObjects[i].name, mapObjects[i]);
        }
    }


    private void WriteDic()
    {
        for (int i = 0; i < mapObjects.Count; i++)
        {
            mapDic.Add(mapObjects[i].name, mapObjects[i]);
        }
    }
    private void AddSpawnPoints()
    {
        for (int i = 0; i < mapObjects.Count; i++)
        {
            foreach (Transform child in mapObjects[i].transform)
            {
                if (child.CompareTag("esp"))
                {
                    eventSpawnPoints.Add(child);
                }
                else if (child.CompareTag("psp"))
                {
                    playerSpawnPoints.Add(child);
                }
            }
        }
    }

    private List<int> MakeRandomValues(int count, int maxValue) // count: 생성할 랜덤 값의 개수, maxValue: 랜덤 값의 최대값+1
    {
        HashSet<int> randomValues = new HashSet<int>();
        while (randomValues.Count < count)
        {
            int randomValue = UnityEngine.Random.Range(0, maxValue);
            randomValues.Add(randomValue);
        }
        return new List<int>(randomValues);
    }

    private void InitializeMiniGames()
    {
        for (int i = 0; i < 5; i++)
        {
            int index = espIndexs[i];
            Transform spawnPoint = eventSpawnPoints[index];
            PhotonNetwork.Instantiate("MiniGame", spawnPoint.position, Quaternion.identity);
        }
    }

    private void InitializeGenerators()
    {
        for (int i = 0; i < 5; i++)
        {
            int index = espIndexs[9 - i];
            Transform spawnPoint = eventSpawnPoints[index];
            PhotonNetwork.Instantiate("Generator", spawnPoint.position, Quaternion.identity);
        }
    }

    private void AssignRole()
    {
        pspIndexs = MakeRandomValues(PhotonNetwork.PlayerList.Length, playerSpawnPoints.Count);
        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
        string monsterName = monTypes[UnityEngine.Random.Range(0, monTypes.Length)];
        ExitGames.Client.Photon.Hashtable monProp = new();
        monProp.Add("Role", monsterName);
        monProp.Add("SpawnIndex", pspIndexs[0]);
        players[roleIndexs[0]].SetCustomProperties(monProp);


        for (int i = 1; i < players.Length; i++)
        {

            string playerName = pTypes[UnityEngine.Random.Range(0, pTypes.Length)];
            ExitGames.Client.Photon.Hashtable playerProp = new ExitGames.Client.Photon.Hashtable();
            playerProp.Add("Role", playerName);
            playerProp.Add("SpawnIndex", pspIndexs[i]);
            players[roleIndexs[i]].SetCustomProperties(playerProp);
        }
        Debug.Log("역할배정완료");
        for (int i = 0; i < players.Length; i++)
        {
            Debug.Log(i + PhotonNetwork.PlayerList[i].CustomProperties["Role"].ToString());
        }
    }

    private void InitializePlayers()
    {
        ExitGames.Client.Photon.Hashtable prop = PhotonNetwork.LocalPlayer.CustomProperties;
        string playerName = prop["Role"].ToString();
        int spawnIndex = (int)prop["SpawnIndex"];
        PhotonNetwork.Instantiate(playerName, playerSpawnPoints[spawnIndex].position, Quaternion.identity);
    }


}

