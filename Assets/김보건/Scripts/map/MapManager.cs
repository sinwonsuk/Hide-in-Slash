using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviourPunCallbacks
{
    public List<GameObject> mapObjects;
    public GameObject miniGamePrefab;
    public GameObject playerPrefab;

    private Dictionary<string, GameObject> mapDic = new();
    private List<Transform> eventSpawnPoints = new();
    private List<Transform> playerSpawnPoints = new();
    private List<int> espIndexs = new();
    private List<int> pspIndexs = new();
    private string currentMap;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InitializeMap();
            AddSpawnPoints();
            espIndexs = MakeRandomValues(10, eventSpawnPoints.Count);
            pspIndexs = MakeRandomValues(5, playerSpawnPoints.Count);
            GenerateMiniGames();
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
            Instantiate(mapObjects[i]);
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

    private void GenerateMiniGames()
    {
        for(int i = 0; i < espIndexs.Count; i++)
        {
            int index = espIndexs[i];
            Transform spawnPoint = eventSpawnPoints[index];
            GameObject miniGame = Instantiate(miniGamePrefab, spawnPoint.position, Quaternion.identity);
        }
    }

    private void PlayerSpawn()
    {
        for (int i = 0; i < pspIndexs.Count; i++)
        {
            int index = pspIndexs[i];
            Transform spawnPoint = playerSpawnPoints[index];
            GameObject player = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        }
    }

}

