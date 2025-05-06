using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using ClientState = Photon.Realtime.ClientState;

public class ExitManager : MonoBehaviourPun
{
    public static ExitManager Instance;

    [Header("UI Prefab")]
    public GameObject allEscapeUI;
    public GameObject aloneEscapeUI;

    private int toExtract;        // 90초가 시작될 때 살아 있던 인원
    private int extracted;        // ExitDoor에 실제로 들어온 인원
    private bool phaseActive;
    private bool phaseEnded;      // 중복 호출 방지

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /* ──────────────────────────────── *
     * 1.  Exit Phase  진입 (RPC 한 번) *
     * ──────────────────────────────── */
    [PunRPC]
    public void RPC_StartExitPhase()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        phaseActive = true;
        phaseEnded = false;
        extracted = 0;
        toExtract = CountAliveRunners();

        // 타이머 90초 재설정
        GameTimer gt = FindObjectOfType<GameTimer>();
        if (gt && PhotonNetwork.IsMasterClient)
            gt.photonView.RPC("RPC_ResetCountdown", RpcTarget.All, 90f);

        Debug.Log($"[Exit] 90초 시작 (목표 {toExtract}명)");
    }

    /* ──────────────────────────────── *
     * 2.  누군가 ExitDoor에 들어옴      *
     * ──────────────────────────────── */
    [PunRPC]
    public void RPC_PlayerEnteredExit()
    {
        if (!PhotonNetwork.IsMasterClient || phaseEnded) return;

        extracted++;
        Debug.Log($"[Exit] 도착 {extracted}/{toExtract}");

        if (extracted >= toExtract)        // 전원 성공
            EndPhase(true, "전원 도착");
    }

    /* ──────────────────────────────── *
     * 3a. 사망 또는 Hatch              *
     * ──────────────────────────────── */
    [PunRPC]
    public void RPC_PlayerFailed()
    {
        if (!PhotonNetwork.IsMasterClient || phaseEnded) return;
        EndPhase(false, "사망·Hatch");
    }

    /* ──────────────────────────────── *
     * 3b. 90초 초과                    *
     * ──────────────────────────────── */
    public void OnExitTimerExpired()
    {
        if (!phaseActive || phaseEnded) return;
        EndPhase(false, "시간 초과");
    }

    /* ──────────────────────────────── *
     * 4.  결과 브로드캐스트            *
     * ──────────────────────────────── */
    private void EndPhase(bool allSuccess, string reason)
    {
        phaseEnded = true;
        Debug.Log($"[Exit] 종료 ({reason})");

        if (allSuccess)
            photonView.RPC(nameof(RPC_ShowAllEscape), RpcTarget.All);
        else
            photonView.RPC(nameof(RPC_ShowAloneEscape), RpcTarget.All);
    }

    [PunRPC] private void RPC_ShowAllEscape() => SpawnUI(allEscapeUI);
    [PunRPC] private void RPC_ShowAloneEscape() => SpawnUI(aloneEscapeUI);

    private void SpawnUI(GameObject prefab)
    {
        var ui = Instantiate(prefab);
        DontDestroyOnLoad(ui);
        ui.SetActive(true);
    }

    /* ──────────────────────────────── *
     * 5. 도움 함수                     *
     * ──────────────────────────────── */
    private int CountAliveRunners()
    {
        int cnt = 0;
        foreach (var p in PhotonNetwork.PlayerList)
            if (!NetworkProperties.instance.GetMonsterStates(p.CustomProperties["Role"].ToString()))
                cnt++;
        return cnt;
    }
}