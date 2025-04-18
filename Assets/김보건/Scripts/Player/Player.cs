using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }

    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;

    [Header("이동")]
    public float moveSpeed = 12f;

    public PlayerStateMachine PlayerStateMachine { get; private set; }

    public PlayerIdle idleState { get; private set; }
    public PlayerMove moveState { get; private set; }
    public PlayerDead deadState { get; private set; }

    private int deadCount = 0;


    [Header("납치됨")]
    [SerializeField] private GameObject moveMap;
    [SerializeField] private string portalName = "CaughtPoint";

    [Header("미니게임")]
    [SerializeField] private GameObject miniGame;
    private bool isInGame = false;

    [Header("발전기")]
    [SerializeField] private GameObject generator;
    private bool isInGenerator = false;


    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        PlayerStateMachine = new PlayerStateMachine();

        idleState = new PlayerIdle(this, PlayerStateMachine, "Idle");
        moveState = new PlayerMove(this, PlayerStateMachine, "Move");
        deadState = new PlayerDead(this, PlayerStateMachine, "Dead");
    }

    private void Start()
    {
        PlayerStateMachine.Initialize(idleState);
    }

    private void Update()
    {
        PlayerStateMachine.currentState.Update();

        if (isInGame && Input.GetKeyDown(KeyCode.E))
        {
            OpenMiniGame();
        }
    }


    private void FixedUpdate()
    {
        PlayerStateMachine.currentState.FixedUpdate();
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y, pos.y);
    }

    public void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
        FlipController(velocity.x);
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

    public void FlipController(float x)
    {
        if (x > 0 && !facingRight) Flip();
        else if (x < 0 && facingRight) Flip();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Prison"))
        {
            deadCount++;
            Debug.Log($"{deadCount} 번 감옥에 들어옴");
            if (deadCount >= 2)
            {
                Debug.Log("플레이어 사망~");
                PlayerStateMachine.ChangeState(deadState);
            }
        }

        //귀신에게 잡혔을 때
        if (collision.CompareTag("Ghost"))
        {
            Debug.Log("귀신에게 잡힘");

            Transform portal = moveMap.transform.Find(portalName);
            transform.position = portal.position;
        }

        // 미니게임 진입
        if (collision.CompareTag("MiniGame"))
        {
            Debug.Log("미니게임 가능");
            isInGame = true;
        }

        // 발전기 돌리기
        //if (collision.CompareTag("Generator"))
        //{
        //    Debug.Log("발전기 작동가능");
        //    isInGenerator = true;
        //}

        // 상점같은 상호작용추가
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("MiniGame"))
        {
            Debug.Log("미니게임 종료");
            isInGame = false;
        }

        //if (collision.CompareTag("Generator"))
        //{
        //    Debug.Log("발전기 작동 불가능");
        //    isInGenerator = false;
        //}
    }

    private void OpenMiniGame()
    {
        Debug.Log("미니게임 시작");
        if (miniGame != null)
            miniGame.SetActive(true);

        // 플레이어 조작 잠금
    }

    public void BecomeGhost()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color c = sr.color;
        c.a = 0.5f;
        sr.color = c;

        //다른사람한테 안보이게
        //if (!isMine())
        //{
        //    gameObject.SetActive(false); 
        //}

        Collider2D col = GetComponent<Collider2D>();
        col.enabled = false;
    }

    public void AnimationTrigger() => PlayerStateMachine.currentState.AnimationFinishTrigger();
}
