using Photon.Pun;
using System.Collections;
using System.Linq;
using UnityEngine;

public class DeadManager : MonoBehaviourPun
{
    public static DeadManager Instance;

    [Header("UI")]
    public GameObject allDeadUI;      // 전원 사망 연출용
    public GameObject someDeadUI;       // 일부 사망
    public float uiDuration = 5f;     // 연출 후 로비 전환까지 지연 시간

    private int runnersAlive;         // 살아있는 도망자 수 (마스터만 신뢰)
    private int runnersEscaped;         // 탈출 성공인원
    private bool alreadyEnded;        // 중복 호출 방지


    private void Awake()
    {
        // 중복 방지
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);  
    }

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        StartCoroutine(InitRunnerCount());
    }

    private IEnumerator InitRunnerCount()
    {
        // 모든 플레이어가 Role / SpawnIndex 를 받을 때까지 대기
        while (PhotonNetwork.PlayerList.Any(p =>
               !p.CustomProperties.ContainsKey("Role")))
        {
            yield return null;      // 한 프레임씩 쉬어 줌
        }

        runnersAlive = CountRunners();
        runnersEscaped = 0;
        Debug.Log($"[DM] Runner 초기화 완료 : {runnersAlive}");
    }

    //타이머 종료
    public void OnTimeOut()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        TryFinishGame("타이머 종료");
    }

    //생존자 사망
    [PunRPC]
    public void RunnerDied()
    {
        if (!PhotonNetwork.IsMasterClient || alreadyEnded) return;

        // 혹시 음수로 내려가지 않도록 Clamp
        runnersAlive = Mathf.Max(0, runnersAlive - 1);
        Debug.Log($"[DM] Runner 남음 : {runnersAlive}");

        if (runnersAlive == 0)
            TryFinishGame("모든 Runner 사망");
    }

    // 생존자 탈출
    [PunRPC]
    public void RunnerEscaped()
    {
        if (!PhotonNetwork.IsMasterClient || alreadyEnded) return;

        runnersAlive = Mathf.Max(0, runnersAlive - 1);
        runnersEscaped++;
        CheckEndCondition();
    }

    //엔드 게임판정
    private void CheckEndCondition()
    {
        if (runnersAlive == 0)         // 생존자 0명
            TryFinishGame("생존자 전멸");
    }


    //게임 종료
    private void TryFinishGame(string reason)
    {
        if (alreadyEnded) return;
        alreadyEnded = true;

        Debug.Log($"[DM] 게임 종료 : {reason}  (Escaped:{runnersEscaped})");

        if (runnersEscaped > 0)            // 탈출 인원이 1명 이상
            photonView.RPC(nameof(ShowSomeDead), RpcTarget.All);
        else                               // 전원 사망
            photonView.RPC(nameof(ShowAllDead), RpcTarget.All);
    }

    [PunRPC] private void ShowAllDead() => PlayUI(allDeadUI);
    [PunRPC] private void ShowSomeDead() => PlayUI(someDeadUI);

    private void PlayUI(GameObject ui)
    {
        if (ui == null) return;

        ui.transform.SetParent(null);                // 부모 해제 (캔버스 등)
        DontDestroyOnLoad(ui);                       // 씬 전환 후에도 유지
        ui.SetActive(true);                          // 보이기

        Transform black = ui.transform.Find("Black");
        if (black)
        {
            var fade = black.GetComponent<playerDeath>();
            if (fade) fade.TriggerFade();
        }

        StartCoroutine(ReturnToLobbyAfter(uiDuration, ui));
    }

    private IEnumerator ReturnToLobbyAfter(float sec, GameObject ui)
    {
        yield return new WaitForSeconds(sec);
        if (ui != null)
            Destroy(ui);

        PhotonNetwork.LoadLevel("RobbyScene");

        yield return new WaitForSeconds(0.1f);
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();
    }


    private int CountRunners()
    {
        int cnt = 0;
        foreach (var p in PhotonNetwork.PlayerList)
        {
            if (NetworkProperties.instance.GetMonsterStates(p.CustomProperties["Role"].ToString()) == false)
                cnt++;
        }
        return cnt;
    }
}
