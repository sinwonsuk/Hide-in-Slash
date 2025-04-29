using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class AssignManager : MonoBehaviourPunCallbacks
{
    private List<GameObject> mapObjects = new();
    private string[] maps = { "HouseMain", "hunthouse", "Main", "miro", "Room", "UnderGround", "WC" };
    private Dictionary<string, GameObject> mapDic = new();
    private List<Transform> eventSpawnPoints = new();
    private List<Transform> playerSpawnPoints = new();
    private List<int> espIndexs = new();
    private List<int> pspIndexs = new();
    private List<int> roleIndexs = new();
    private string[] monTypes = { "Mon1", "Mon2", "Mon3" }; //몬스터 프리팹 이름
    private string[] pTypes = { "Player", "Player2", "Player3", "Player4", "Player5", "Player6", "Player7" }; // 플레이어 프리팹 이름
    private string currentMap;
    private Transform shipTf;

    private new void OnEnable()
    {
        EventManager.RegisterEvent(EventType.AllGeneratorSuccess, SpawnExit);
        Debug.Log("구독완");
    }

    private void StartGame() //메모장
    {

        //사람이 다 모였다는 가정 하에
        //대기실

        if (PhotonNetwork.IsMasterClient)
        {
            roleIndexs = MakeRandomValues(PhotonNetwork.PlayerList.Length, PhotonNetwork.PlayerList.Length); // 인원수 맞춰 랜덤으로 순서섞기
            AssignRole(); // 섞인 순서에서 첫사람이 몬스터, 나머지가 플레이어
        }


        //게임으로 넘어가면
        //게임내부임

        if (PhotonNetwork.IsMasterClient) //마스터만
        {
            InitializeMap(); // 맵 만들기
        }
        //맵 만들고 생성된 맵에 대한 정보는 각 클라이언트에서 저장
        WriteDic();
        AddSpawnPoints();

        //다시 마스터만 진행
        if (PhotonNetwork.IsMasterClient)
        {
            pspIndexs = MakeRandomValues(PhotonNetwork.PlayerList.Length, playerSpawnPoints.Count); // 플레이어 스폰포인트 섞기
            espIndexs = MakeRandomValues(10, eventSpawnPoints.Count); // 여러 이벤트 스폰포인트 섞기
            InitializeMiniGames(); // 미니게임 뿌리기
            InitializeGenerators(); // 발전기 뿌리기
            AssignSpawnPoint(); // 각 플레이어 스폰포인트 할당
            Debug.Log("방에서 마스터할일 완");
        }

        //위에 할당 후 아래에서 서버 정보 활용하려면 좀 기다려줘야함
        //
        //여기쯤에 시간 한 0.5초정도 코루틴 넣고
        //
        //밑에 서버 정보 활용하는거 넣어야함

        InitializePlayers(); // 각 클라이언트에서 플레이어 생성
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
        for (int i = 0; i < maps.Length; i++)
        {
            GameObject go = PhotonNetwork.Instantiate(maps[i], Vector3.right*200*i , Quaternion.identity);
            mapObjects.Add(go);
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
                else if (child.name == "ShipSpawn")
                {
                    shipTf = child;
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

    public void AssignRole()
    {

        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
        string monsterName = monTypes[UnityEngine.Random.Range(0, monTypes.Length)];
        ExitGames.Client.Photon.Hashtable monProp = new();
        monProp.Add("Role", monsterName);
        players[roleIndexs[0]].SetCustomProperties(monProp);


        for (int i = 1; i < players.Length; i++)
        {
            string playerName = pTypes[UnityEngine.Random.Range(0, pTypes.Length)];
            ExitGames.Client.Photon.Hashtable playerProp = new ExitGames.Client.Photon.Hashtable();
            playerProp.Add("Role", playerName);
            players[roleIndexs[i]].SetCustomProperties(playerProp);
        }
        Debug.Log("역할배정완료");

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MergeScene");
        }
    }

    private void AssignSpawnPoint()
    {
        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
        ExitGames.Client.Photon.Hashtable monProp = new();
        monProp.Add("SpawnIndex", pspIndexs[0]);
        players[roleIndexs[0]].SetCustomProperties(monProp);


        for (int i = 1; i < players.Length; i++)
        {
            ExitGames.Client.Photon.Hashtable playerProp = new ExitGames.Client.Photon.Hashtable();
            playerProp.Add("SpawnIndex", pspIndexs[i]);
            players[roleIndexs[i]].SetCustomProperties(playerProp);
        }
    }

    private void InitializePlayers()
    {
        ExitGames.Client.Photon.Hashtable prop = PhotonNetwork.LocalPlayer.CustomProperties;
        string playerName = prop["Role"].ToString();
        int spawnIndex = (int)prop["SpawnIndex"];
        PhotonNetwork.Instantiate(playerName, playerSpawnPoints[spawnIndex].position, Quaternion.identity);
        CinemachineCamera cam = FindFirstObjectByType<CinemachineCamera>();
        CinemachineConfiner2D confiner = cam.GetComponent<CinemachineConfiner2D>();
        Collider2D col = playerSpawnPoints[spawnIndex].GetComponentInParent<Collider2D>();
        confiner.BoundingShape2D = col;
    }

    private void SpawnExit()
    {
        PhotonNetwork.Instantiate("Ship", shipTf.position, Quaternion.identity);
        EventManager.UnRegisterEvent(EventType.AllGeneratorSuccess, SpawnExit);
    }
}

