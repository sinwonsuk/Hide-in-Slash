using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ghost : MonoBehaviourPun
{

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();

        ghostStateMachine = new GhostStateMachine();
        //idleState = new GhostIdle(this, ghostStateMachine, "Idle");

        if (photonView.IsMine)
        {
            GameObject bossCanvas = GameObject.Find("BossCanvas");

            if (bossCanvas != null)
            {
                Transform allDeath = bossCanvas.transform.Find("AllDeath");
                if (allDeath != null)
                    allKillUI = allDeath.gameObject;

                Transform noneKill = bossCanvas.transform.Find("BossLose");
                if (noneKill != null)
                    noneKillUI = noneKill.gameObject;

                Transform someDeath = bossCanvas.transform.Find("DeathAndEscape");
                if (someDeath != null)
                    someKillUI = someDeath.gameObject;

            }
        }
    }

    protected virtual void Start()
    {
        //ghostStateMachine.Initialize(idleState);
    }

    protected virtual void Update()
    {
        ghostStateMachine.currentState.Update();
    }

    private void LateUpdate()
    {
        
    }

    protected virtual void FixedUpdate()
    {
        ghostStateMachine.currentState.FixedUpdate();
        Vector3 pos = transform.position;
        //transform.position = new Vector3(pos.x, pos.y, pos.y);

    }

    public void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
    }

    public void SetZeroVelocity()
    {
        rb.linearVelocity = Vector2.zero;
    }

    public void Flip()
    {
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    private void FlipVertical()
    {
        facingUpDir *= -1;
        facingUp = !facingUp;
        transform.Rotate(180, 0, 0);
    }

    public void FlipController(float x, float y)
    {
        // 좌우 반전
        if (x > 0 && !facingRight) Flip();
        else if (x < 0 && facingRight) Flip();

        // 상하 반전
        if (y > 0 && !facingUp) FlipVertical();
        else if (y < 0 && facingUp) FlipVertical();
    }

    public void SetFacingDirection(int dir, int upDir)
    {
        this.facingDir = dir;
        this.facingUpDir = upDir;
    }


    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    private bool IsRunner(Photon.Realtime.Player player)
    {
        if (!player.CustomProperties.TryGetValue("Role", out var roleObj) || roleObj == null)
            return false;

        string roleName = roleObj.ToString();
        bool isMonster = NetworkProperties.instance.GetMonsterStates(roleName);
        return !isMonster;
    }

    public void TryCheckGhostEnding()
    {
        Debug.Log("[Ghost] TryCheckGhostEnding() 호출됨");

        // 이미 연출을 보여준 경우는 다시 실행하지 않음
        if (hasShownGhostEnding)
        {
            return;
        }

        // 도망자 리스트
        List<int> runnerList = new List<int>();
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (IsRunner(player)) // 도망자
            {
                runnerList.Add(player.ActorNumber);
            }
        }

        //아직 상태가 도착하지 않은 도망자가 있다면 연출x
        foreach (int actor in runnerList)
        {
            if (!Player.runnerStatuses.ContainsKey(actor))
            {
                return;
            }
        }

        // 도망자들의 상태 목록
        List<RunnerStatus> runnerStatusList = new List<RunnerStatus>();
        foreach (int actor in runnerList)
        {
            runnerStatusList.Add(Player.runnerStatuses[actor]);
        }

        // 살아 있는 도망자가 있으면 연출 x
        foreach (RunnerStatus status in runnerStatusList)
        {
            if (status == RunnerStatus.Alive)
            {
                return;
            }
        }

        // 전원 죽거나 감옥 =>  연출
        hasShownGhostEnding = true;

        int deadCount = 0;
        int escapeCount = 0;
        foreach (RunnerStatus status in runnerStatusList)
        {
            if (status == RunnerStatus.Dead)
            {
                deadCount++;
            }

            if (status == RunnerStatus.Escaped)
            {
                escapeCount++;
            }
        }

        if (deadCount == runnerList.Count)
        {
            ShowGhostUI(allKillUI);
        }
        else if (escapeCount == runnerList.Count)
        {
            ShowGhostUI(noneKillUI);
        }
        else
        {
            ShowGhostUI(someKillUI);
        }
    }

    private void ShowGhostUI(GameObject ui)
    {
        Debug.Log("[Ghost 연출 시작]");

        if (ui == null) return;

        ui.transform.SetParent(null);          // BossCanvas에서 떼어냄
        DontDestroyOnLoad(ui);                 // 씬 전환해도 유지
        ui.SetActive(true);                    // 연출 보여주기

        Transform black = ui.transform.Find("Black");
        if (black)
        {
            var fade = black.GetComponent<playerDeath>();
            if (fade) fade.TriggerFade();
        }

        // 로비 전환 핸들러 실행
        GameObject handlerGO = new GameObject("EndingHandler");
        EndingHandler handler = handlerGO.AddComponent<EndingHandler>();
        handler.StartEndSequence(ui, uiDuration);
    }

    public virtual void UpdateAnimParam(Vector2 input) { }



    [Header("UI")]
    private bool hasShownGhostEnding = false;
    public GameObject allKillUI;
    public GameObject someKillUI;
    public GameObject noneKillUI;
    public float uiDuration = 5f;


    public Vector2 MoveInput => ghostStateMachine.CurrentStateMoveInput;
    public Animator anim { get; protected set; }
    public Rigidbody2D rb { get; protected set; }

    public SpriteRenderer sr { get; protected set; }

    public int facingDir { get; private set; } = 1;
    public int facingUpDir { get; private set; } = 1;   // 1 = 위, -1 = 아래
    protected bool facingRight = true;
    protected bool facingUp = true;

    [Header("이동 속도")]
    [SerializeField] protected float moveSpeed = 5f;

    public GhostStateMachine ghostStateMachine { get; protected set; }
    public virtual GhostState idleState { get; protected set; }
    public virtual GhostState moveState { get; protected set; }

    public virtual GhostState useSkillState { get; protected set; }
    public virtual GhostState idleSkillState { get; protected set; }
}
