using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using UnityEngine;
using Photon.Pun.Demo.PunBasics;
using Unity.VisualScripting;
using ExitGames.Client.Photon;
using UnityEngine.UIElements;

public class AssignManager : MonoBehaviourPunCallbacks
{
    private List<GameObject> mapObjects = new();
    private string[] maps = { "HouseMain", "hunthouse", "Main", "miro", "Room", "UnderGround", "WC" };
    private Dictionary<string, GameObject> mapDic = new();
    private List<Transform> eventSpawnPoints = new();
    private List<Transform> playerSpawnPoints = new();
    private List<Transform> generatorSpawnPoints = new();
    private List<Transform> maingeneratorSpawnPoints = new();
    private List<int> espIndexs = new();
    private List<int> pspIndexs = new();
    private List<int> gspIndexs = new();
    private List<int> maingspIndexs = new();
    private List<int> roleIndexs = new();
    private List<string> pTypes = new List<string> { "Player", "Player2", "Player3", "Player4", "Player5", "Player6", "Player7" }; // 플레이어 프리팹 이름

    private List<GameObject> generators = new();
    private List<GameObject> minigames = new();
    private List<GameObject> Maps = new();

    GameObject ship;

    private string currentMap;
    private Transform shipTf;
    string playerName;
    int spawnIndex;
    private bool initialized = false;
    const byte EVENT_MAP_READY = 1;

    public static AssignManager instance;

    GameObject player;

    public Photon.Realtime.Player Bossplayer;

    public void ListClear()
    {
        mapObjects.Clear();
        mapDic.Clear();
        eventSpawnPoints.Clear();
        playerSpawnPoints.Clear();
        generatorSpawnPoints.Clear();
        maingeneratorSpawnPoints.Clear();
        espIndexs.Clear();
        pspIndexs.Clear();
        gspIndexs.Clear();
        maingspIndexs.Clear();
        roleIndexs.Clear();
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        //PhotonNetwork.Destroy(player);
        instance = null;
        initialized = false;
        pTypes.Clear();
        pTypes = new List<string> { "Player", "Player2", "Player3", "Player4", "Player5", "Player6", "Player7" }; // 플레이어 프리팹 이름

        Destroy(gameObject);

        //if(PhotonNetwork.IsMasterClient)
        //{
        //    for (int i = 0; i < minigames.Count; i++)
        //    {
        //        PhotonNetwork.Destroy(minigames[i]);
        //    }

        //    minigames.Clear();

        //    for (int i = 0; i < generators.Count; i++)
        //    {
        //        PhotonNetwork.Destroy(generators[i]);
        //    }

        //    generators.Clear();

        //    for (int i = 0; i < Maps.Count; i++)
        //    {
        //        PhotonNetwork.Destroy(Maps[i]);
        //    }

        //    Maps.Clear();
        //}

        //PhotonNetwork.Destroy(ship);


    }


    private new void OnEnable()
    {
        
    }

    private void Awake()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        EventManager.RegisterEvent(EventType.AllGeneratorSuccess, SpawnExit);
        Debug.Log("구독완");
    }

    private void OnDestroy()
    {
        EventManager.UnRegisterEvent(EventType.AllGeneratorSuccess);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MergeScene")
        {
            AssignManager.instance.asbvasdf();
        }
    }

    public void StartGame() //메모장
    {
        if (PhotonNetwork.IsMasterClient)
        {
            roleIndexs = MakeRandomValues(PhotonNetwork.PlayerList.Length, PhotonNetwork.PlayerList.Length); // 인원수 맞춰 랜덤으로 순서섞기
            AssignRole(); // 섞인 순서에서 첫사람이 몬스터, 나머지가 플레이어
            GameReadyManager.Instance.PropertiesAction += SetPlayerName;
        }

        if (PhotonNetwork.LocalPlayer.IsMasterClient || true) 
        {
            GameReadyManager.Instance.WaitUntilAllInitProperties(GameReadyManager.Instance.ShowRolePanel);
        }
    }

    //IEnumerator CallAssignDelayed()
    //{
    //    yield return null; // 한 프레임 기다림
    //    GameReadyManager.Instance.assignManager.GetComponent<AssignManager>().asbvasdf();
    //}
    public void asbvasdf()
    {

        //게임으로 넘어가면
        //게임내부임

        if (PhotonNetwork.IsMasterClient)
        {
            InitializeMap(); // 맵 생성 및 브로드캐스트
            Wait1();         // 마스터는 직접 실행
        }

        gameObject.SetActive(true);

        //맵 만들고 생성된 맵에 대한 정보는 각 클라이언트에서 저장


        //다시 마스터만 진행



        //다시 마스터만 진행


        //gameObject.SetActive(true);

        //Wait1();


        //위에 할당 후 아래에서 서버 정보 활용하려면 좀 기다려줘야함
        //
        //여기쯤에 시간 한 0.5초정도 코루틴 넣고
        //
        //밑에 서버 정보 활용하는거 넣어야함

        // 각 클라이언트에서 플레이어 생성
    }
    private void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == EVENT_MAP_READY)
        {
            Debug.Log("맵 생성 완료 신호 받음");
            Wait1(); // 이제 안전하게 실행 가능
        }
    }
    void Wait1()
    {
        AddMapObjects();
        Debug.Log("대기끝");
        WriteDic();
        AddSpawnPoints();

        if (PhotonNetwork.IsMasterClient)
        {
            pspIndexs = MakeRandomValues(PhotonNetwork.PlayerList.Length, playerSpawnPoints.Count); // 플레이어 스폰포인트 섞기
            espIndexs = MakeRandomValues(23, eventSpawnPoints.Count); // 여러 이벤트 스폰포인트 섞기
            gspIndexs = MakeRandomValues(3, generatorSpawnPoints.Count); // 발전기 스폰포인트 섞기
            maingspIndexs = MakeRandomValues(3, maingeneratorSpawnPoints.Count); // 발전기 스폰포인트 섞기
            InitializeMiniGames(); // 미니게임 뿌리기
            InitializeGenerators(); // 발전기 뿌리기
            AssignSpawnPoint(); // 각 플레이어 스폰포인트 할당
            Debug.Log("방에서 마스터할일 완");
        }

        Debug.Log("대기끝");
        GameReadyManager.Instance.StartCoroutine(tttt());
    }



    private void Update()
    {

    }

    public GameObject GetCurrentMap()
    {
        if (mapDic.ContainsKey(currentMap))
            return mapDic[currentMap];
        return null;
    }

    public GameObject MoveMap(GameObject mapPrefab)
    {
        string mapName = mapPrefab.name + "(Clone)";

        currentMap = mapName;
        return mapDic[mapName];
    }


    private void InitializeMap()
    {
        for (int i = 0; i < maps.Length; i++)
        {
            GameObject go = PhotonNetwork.InstantiateRoomObject(maps[i], Vector3.right * 200 * i, Quaternion.identity);



            Maps.Add(go);

        }

        // 맵 생성 완료 후 신호
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.RaiseEvent(EVENT_MAP_READY, null, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
        }
    }

    private void AddMapObjects()
    {
        Array a = GameObject.FindGameObjectsWithTag("Maps");
        foreach (GameObject obj in a)
        {
            mapObjects.Add(obj);
        }
    }
    [PunRPC]
    public void AAA(int viewID)
    {
        GameObject go = PhotonView.Find(viewID).gameObject;
        mapObjects.Add(go);
    }






    private void WriteDic()
    {
        for (int i = 0; i < mapObjects.Count; i++)
        {
            // 중복 등록 방지
            var key = mapObjects[i].name;
            if (!mapDic.ContainsKey(key))
            {
                mapDic.Add(key, mapObjects[i]);
            }
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
                else if (child.CompareTag("gsp"))
                {
                    generatorSpawnPoints.Add(child);
                }
                else if (child.CompareTag("maingsp"))
                {
                    maingeneratorSpawnPoints.Add(child);
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
        for (int i = 0; i < 20; i++)
        {
            int index = espIndexs[i];
            Transform spawnPoint = eventSpawnPoints[index];
            GameObject ins = PhotonNetwork.InstantiateRoomObject("MiniGame", spawnPoint.position, Quaternion.identity);

            minigames.Add(ins);
        }
    }

    private void InitializeGenerators()
    {
        for (int i = 0; i < 3; i++)
        {
            int index = gspIndexs[i];
            Transform spawnPoint = generatorSpawnPoints[index];
            GameObject ins = PhotonNetwork.InstantiateRoomObject("Generator", spawnPoint.position, Quaternion.identity);


            generators.Add(ins);
        }
        for (int i = 0; i < 3; i++)
        {
            int index = maingspIndexs[i];
            Transform spawnPoint = maingeneratorSpawnPoints[index];
            PhotonNetwork.InstantiateRoomObject("Generator", spawnPoint.position, Quaternion.identity);
        }
    }

    private bool AllPlayersHaveRoles()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (!player.CustomProperties.ContainsKey("Role"))
            {
                return false;
            }

            if (!player.CustomProperties.ContainsKey("SpawnIndex"))
            {
                return false;
            }


        }
        return true;
    }

    public IEnumerator tttt()
    {
        while (true)
        {
            if (AllPlayersHaveRoles())
            {
                if (InitializePlayers() == true)
                {
                    yield break;
                }
            }
            yield return null;
        }
    }


    public void SetPlayerName(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable hashtable)
    {

        //if (hashtable.ContainsKey("Role"))
        //{
        //    Debug.Log($"{targetPlayer.NickName} 역할 설정됨: {hashtable["Role"]}");

        //    // 모든 플레이어의 Role이 설정되었는지 확인
        //    if (AllPlayersHaveRoles())
        //    {
        //        Debug.Log("모든 플레이어 역할 설정 완료!");
        //        InitializePlayers();
        //    }
        //}
    }
    public void AssignRole()
    {
        pTypes = new List<string> { "Player", "Player2", "Player3", "Player4", "Player5", "Player6", "Player7" };

        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
        //string[] monsterTypes = { "PeanutGhost", "PeanutGhost", "PeanutGhost" };

        string[] monsterTypes = { "PeanutGhost", "ProteinGhost", "PukeGirlGhost" };
        //string bossType = monsterTypes[UnityEngine.Random.Range(0, monsterTypes.Length)];

        roleIndexs = MakeRandomValues(players.Length, players.Length);
        ExitGames.Client.Photon.Hashtable monProp = new();
        monProp.Add("Role", "Boss");
        //monProp.Add("BossType", bossType);
        players[roleIndexs[0]].SetCustomProperties(monProp);

        Bossplayer = players[roleIndexs[0]];
    
        for (int i = 1; i < players.Length; i++)
        {
            string playerName = pTypes[UnityEngine.Random.Range(0, pTypes.Count)];
            ExitGames.Client.Photon.Hashtable playerProp = new();
            playerProp.Add("Role", playerName);
            pTypes.Remove(playerName);
            players[roleIndexs[i]].SetCustomProperties(playerProp);
        }

        Debug.Log("역할배정완료");
    }


    [PunRPC]
    public void RPC_LoadMergeScene()
    {
        //PhotonNetwork.LoadLevel("MergeScene");
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

    private bool InitializePlayers()
    {
        if (initialized) return false; // 이미 초기화됨


          initialized = true;

        ExitGames.Client.Photon.Hashtable prop = PhotonNetwork.LocalPlayer.CustomProperties;

        string role = "";
        string bossType = "";

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Role", out object selfRoleObj))
            role = selfRoleObj.ToString();

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("SpawnIndex", out object index))
            spawnIndex = (int)index;

        //BossType도 함께 가져오기
        //if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("BossType", out object bossTypeObj))
        //    bossType = bossTypeObj.ToString();

        ////Instantiate할 때 Role 또는 BossType을 기준으로
        //if (role == "Boss" && !string.IsNullOrEmpty(bossType))
        //{
        //    PhotonNetwork.Instantiate(bossType, playerSpawnPoints[spawnIndex].position, Quaternion.identity);
        //}
        //else
        {
            player = PhotonNetwork.Instantiate(role, playerSpawnPoints[spawnIndex].position, Quaternion.identity);


            StartCoroutine(SendRPCWithDelay(player, playerSpawnPoints[spawnIndex].position));

        }

        //CinemachineCamera cam = FindFirstObjectByType<CinemachineCamera>();
        //CinemachineConfiner2D confiner = cam.GetComponent<CinemachineConfiner2D>();
        //Collider2D col = playerSpawnPoints[spawnIndex].GetComponentInParent<Collider2D>();
        //confiner.BoundingShape2D = col;
        //StartCoroutine(ResetCache());
        return true;
    }

    

    IEnumerator SendRPCWithDelay(GameObject player, Vector3 pos)
    {
        yield return new WaitForSeconds(1.0f); // 한 프레임 이상 대기
        if (player != null)
        {
            player.GetComponent<PhotonView>().RPC("SetStartPosition", RpcTarget.AllBuffered, pos);
        }
    }

    IEnumerator ResetCache()
    {
        yield return new WaitForSeconds(0.2f);
        CinemachineCamera cam = FindFirstObjectByType<CinemachineCamera>();
        CinemachineConfiner2D confiner = cam.GetComponent<CinemachineConfiner2D>();
        confiner.InvalidateBoundingShapeCache();
    }
    private void SpawnExit()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        GameObject ins = PhotonNetwork.InstantiateRoomObject("Ship", shipTf.position, Quaternion.identity);
      
        ship = ins;
    }
}

