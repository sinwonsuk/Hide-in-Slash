using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject player;
    public List<GameObject> mapObjects;

    private Dictionary<string, GameObject> mapDic = new();
    private string currentMap;

    void Start()
    {
        for (int i = 0; i < mapObjects.Count; i++)
        {
            Instantiate(mapObjects[i]);
            mapDic.Add(mapObjects[i].name, mapObjects[i]);
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

    //public void MovePlayer(Vector2 newPos)  
    //{  
    //    player.transform.position = newPos;  
    //}  
}

