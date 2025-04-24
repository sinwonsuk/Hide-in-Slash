using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviourPunCallbacks
{
    public List<GameObject> mapObjects;
    public GameObject miniGamePrefab;
    public GameObject generatorPrefab;
    public GameObject playerPrefab;

    private Dictionary<string, GameObject> mapDic = new();
    private List<Transform> eventSpawnPoints = new();
    private List<Transform> playerSpawnPoints = new();
    private List<int> espIndexs = new();
    private List<int> pspIndexs = new();
    private string[] pRoles = { "Player1", "Player2", "Player3", "Player4", "Player5", "Player6", "Player7" };
    private string[] monRoles = { "Mon1", "Mon2", "Mon3", "Mon4", "Mon5" };
    private string currentMap;
    private int minigameCount = 10; //미니게임 개수
    private int generatorCount = 5; //발전기 개수





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

    private void AssignRole()
    {
        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;

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
                if (child.CompareTag("EventSpawnPoint"))
                {
                    eventSpawnPoints.Add(child);
                }
                else if (child.CompareTag("PlayerSpawnPoint"))
                {
                    playerSpawnPoints.Add(child);
                }
            }
        }
    }

    private List<int> MakeRandomValues(int count, int maxValue)
    {
        HashSet<int> randomValues = new HashSet<int>();
        while (randomValues.Count < count)
        {
            int randomValue = Random.Range(0, maxValue);
            randomValues.Add(randomValue);
        }
        return new List<int>(randomValues);
    }

    private void InitializeMinigames() //이벤트스폰포인트를 앞에서부터 미니게임을 개수만큼 스폰
    {
        for (int i = 0; i < minigameCount; i++)
        {
            int index = espIndexs[i];
            Transform spawnPoint = eventSpawnPoints[index];
            PhotonNetwork.Instantiate("MiniGame", spawnPoint.position, Quaternion.identity);
        }
    }

    private void InitializeGenerators() //이벤트스폰포인트를 뒤에서부터 발전기를 개수만큼 스폰
    {
        for (int i = 0; i < generatorCount; i++)
        {
            int index = espIndexs[espIndexs.Count - i - 1];
            Transform spawnPoint = eventSpawnPoints[index];
            PhotonNetwork.Instantiate("Generator", spawnPoint.position, Quaternion.identity);
        }
    }

    private void InitializePlayers()
    {
        
    }


}

