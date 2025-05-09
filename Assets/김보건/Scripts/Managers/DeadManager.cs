using Photon.Pun;
using System.Collections;
using System.Linq;
using UnityEngine;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class DeadManager : MonoBehaviourPunCallbacks
{
    //public static DeadManager Instance;

    //[Header("UI")]
    //public GameObject allDeadUI;      // 전원 사망 연출용
    //public GameObject someDeadUI;       // 일부 사망
    //public GameObject allEscapeUI; // 탈출 연출용
    //public GameObject someEscapeUI; // 탈출 연출용
    //public GameObject prisonEndingUI; // 감옥 연출용

    //public float uiDuration = 5f;     // 연출 후 로비 전환까지 지연 시간

    //private readonly List<int> escapedActors = new();   // 탈출한 플레이어 ActorNumber 모음
    //private int runnersTotal;
    //private int runnersAlive;         // 살아있는 도망자 수 (마스터만 신뢰)
    //private int runnersEscaped;         // 탈출 성공인원
    //private bool alreadyEnded;        // 중복 호출 방지


    //private void Awake()
    //{
    //    // 중복 방지
    //    if (Instance != null)
    //    {
    //        Destroy(gameObject);
    //        return;
    //    }

    //    Instance = this;

    //    DontDestroyOnLoad(gameObject);
    //}

    //private void Start()
    //{
    //    if (!PhotonNetwork.IsMasterClient) return;
    //    StartCoroutine(InitRunnerCount());
    //}

    //private IEnumerator InitRunnerCount()
    //{
    //    // 모든 플레이어가 Role / SpawnIndex 를 받을 때까지 대기
    //    while (PhotonNetwork.PlayerList.Any(p =>
    //           !p.CustomProperties.ContainsKey("Role")))
    //    {
    //        yield return null;      // 한 프레임씩 쉬어 줌
    //    }

    //    runnersTotal = CountRunners();  // ← 이거 있어야 함
    //    runnersAlive = runnersTotal;
    //    runnersEscaped = 0;
    //    Debug.Log($"[DM] Runner 초기화 완료 : {runnersAlive}");
    //}

    ////타이머 종료
    //public void OnTimeOut()
    //{
    //    if (!PhotonNetwork.IsMasterClient) return;
    //    TryFinishGame("타이머 종료");
    //}

    ////생존자 사망
    //[PunRPC]
    //public void RunnerDied()
    //{
    //    if (!PhotonNetwork.IsMasterClient || alreadyEnded) return;

    //    runnersAlive = Mathf.Max(0, runnersAlive - 1);
    //    Debug.Log($"[DM] Runner 남음 : {runnersAlive}");

    //    CheckEndCondition();
    //}

    //// 생존자 탈출
    //[PunRPC]
    //public void RunnerEscaped(int actorNumber)
    //{
    //    if (!PhotonNetwork.IsMasterClient || alreadyEnded) return;

    //    runnersAlive = Mathf.Max(0, runnersAlive - 1);
    //    escapedActors.Add(actorNumber);       // 목록에 ActorNumber 삽입
    //    CheckEndCondition();
    //}

    ////엔드 게임판정
    //private void CheckEndCondition()
    //{
    //    int realAlive = 0;

    //    foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
    //    {
    //        bool isRunner = !NetworkProperties.instance.GetMonsterStates(p.CustomProperties["Role"].ToString());
    //        bool isEscaped = escapedActors.Contains(p.ActorNumber);
    //        bool isInPrison = p.CustomProperties.TryGetValue("IsInPrison", out object prison) && (bool)prison;
    //        bool isDead = false;
    //        if (p.CustomProperties.TryGetValue("IsDead", out object deadObj))
    //        {
    //            isDead = (bool)deadObj;
    //        }

    //        // 진짜 생존한 러너 = 탈출X, 감옥X, 죽음X
    //        if (isRunner && !isEscaped && !isInPrison && !isDead)
    //        {
    //            realAlive++;
    //        }
    //    }

    //    Debug.Log($"[DM] CheckEndCondition() - 생존자 수: {realAlive}");

    //    if (realAlive == 0)
    //        TryFinishGame("생존자 전멸");
    //}


    ////게임 종료
    //private void TryFinishGame(string reason)
    //{
    //    if (alreadyEnded) return;
    //    alreadyEnded = true;

    //    int[] escapedArray = escapedActors.ToArray();

    //    List<int> prisonedPlayers = new();  
    //    foreach (var p in PhotonNetwork.PlayerList)
    //    {
    //        if (p.CustomProperties.TryGetValue("IsInPrison", out object value) && (bool)value)
    //        {
    //            prisonedPlayers.Add(p.ActorNumber);
    //        }
    //    }

    //    photonView.RPC(nameof(ShowResultPerClient),
    //           RpcTarget.All,
    //           escapedArray,
    //           runnersTotal,
    //           prisonedPlayers.ToArray());
    //}

    //[PunRPC]
    //private void ShowResultPerClient(int[] escapedList, int runnersTotal, int[] prisonedList)
    //{
    //    // 술레 플레이어면 UI 생략
    //    if (NetworkProperties.instance.GetMonsterStates(
    //            PhotonNetwork.LocalPlayer.CustomProperties["Role"].ToString()))
    //        return;

    //    int escapedCnt = escapedList.Length;
    //    bool iEscaped = escapedList.Contains(PhotonNetwork.LocalPlayer.ActorNumber);
    //    bool iInPrison = prisonedList.Contains(PhotonNetwork.LocalPlayer.ActorNumber);

    //    //전원탈출, 전원사망
    //    if (escapedCnt == runnersTotal) PlayUI(allEscapeUI);
    //    else if (escapedCnt == 0) PlayUI(allDeadUI);
    //    //일부탈출
    //    else
    //    {
    //        if (iInPrison)
    //            PlayUI(prisonEndingUI);  // 새로 만든 감옥 엔딩 연출 UI
    //        else if (iEscaped)
    //            PlayUI(someEscapeUI);   // 탈출
    //        else
    //            PlayUI(someDeadUI);     // 죽음
    //    }

    //    // 5초 뒤 로비 이동
    //    StartCoroutine(ReturnToLobbyAfter(5f, null));
    //}

    //[PunRPC] private void ShowAllDead() => PlayUI(allDeadUI);
    //[PunRPC] private void ShowSomeDead() => PlayUI(someDeadUI);
    //[PunRPC] private void ShowAllEscape() => PlayUI(allEscapeUI);
    //[PunRPC] private void ShowSomeEscape() => PlayUI(someEscapeUI);

    //private void PlayUI(GameObject ui)
    //{
    //    if (ui == null) return;

    //    ui.transform.SetParent(null);                // 부모 해제 (캔버스 등)
    //    DontDestroyOnLoad(ui);                       // 씬 전환 후에도 유지
    //    ui.SetActive(true);                          // 보이기

    //    Transform black = ui.transform.Find("Black");
    //    if (black)
    //    {
    //        var fade = black.GetComponent<playerDeath>();
    //        if (fade) fade.TriggerFade();
    //    }

    //    StartCoroutine(ReturnToLobbyAfter(uiDuration, ui));
    //}

    //private IEnumerator ReturnToLobbyAfter(float sec, GameObject ui)
    //{
    //    yield return new WaitForSeconds(sec);
    //    if (ui != null)
    //        Destroy(ui);

    //    PhotonNetwork.LoadLevel("RobbyScene");

    //    yield return new WaitForSeconds(0.1f);
    //    if (!PhotonNetwork.InLobby)
    //        PhotonNetwork.JoinLobby();
    //}


    //private int CountRunners()
    //{
    //    int cnt = 0;
    //    foreach (var p in PhotonNetwork.PlayerList)
    //    {
    //        if (NetworkProperties.instance.GetMonsterStates(p.CustomProperties["Role"].ToString()) == false)
    //            cnt++;
    //    }
    //    return cnt;
    //}

    //[PunRPC]
    //public void ForceInitDeadManager()
    //{
    //    if (!PhotonNetwork.IsMasterClient) return;
    //    Debug.Log("[DM] 강제 초기화 호출됨");
    //    StartCoroutine(InitRunnerCount());
    //}

    //public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    //{
    //    Debug.Log($"[DM] 마스터 변경됨 → 새로운 마스터: {newMasterClient.NickName}");

    //    if (PhotonNetwork.IsMasterClient &&
    //    PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
    //    {
    //        Debug.Log("[DM] 새로운 Master가 DeadManager 초기화 재실행");

    //        if (DeadManager.Instance != null)
    //        {
    //            DeadManager.Instance.StartCoroutine(DeadManager.Instance.InitRunnerCount());
    //        }
    //        else
    //        {
    //            Debug.LogWarning("[DM] DeadManager 인스턴스가 null입니다");
    //        }
    //    }
    //}

    //public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player target,
    //                                          ExitGames.Client.Photon.Hashtable changedProps)
    //{
    //    // 마스터가 아니거나 이미 게임이 끝났으면 무시
    //    if (!PhotonNetwork.IsMasterClient || alreadyEnded) return;

    //    // 감옥 상태가 바뀐 경우에만 재검사
    //    if (changedProps.ContainsKey("IsInPrison"))
    //    {
    //        CheckEndCondition();   // 여기서 realAlive 재계산 → 0이면 바로 TryFinishGame
    //    }
    //}

}