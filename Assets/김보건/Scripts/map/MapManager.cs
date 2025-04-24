using Photon.Pun;
using Photon.Realtime;
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
    private string currentMap;

    void Start()
    {

        PhotonNetwork.ConnectUsingSettings();
        
        

        
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 6 }, null);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InitializeMap();
            AddSpawnPoints();
            espIndexs = MakeRandomValues(10, eventSpawnPoints.Count);
            pspIndexs = MakeRandomValues(5, playerSpawnPoints.Count);
            InitializeMiniGames();
            InitializeGenerators();
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
            int randomValue = Random.Range(0, maxValue);
            randomValues.Add(randomValue);
        }
        return new List<int>(randomValues);
    }

    private void InitializeMiniGames()
    {
        for(int i = 0; i < 5; i++)
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
            int index = espIndexs[9-i];
            Transform spawnPoint = eventSpawnPoints[index];
            PhotonNetwork.Instantiate("Generator", spawnPoint.position, Quaternion.identity);
        }
    }

    private void PlayerSpawn()
    {
        for (int i = 0; i < pspIndexs.Count; i++)
        {
            int index = pspIndexs[i];
            Transform spawnPoint = playerSpawnPoints[index];
        }
    }

}

