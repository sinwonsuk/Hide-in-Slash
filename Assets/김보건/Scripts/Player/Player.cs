using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }

    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;

    [Header("�̵�")]
    public float moveSpeed = 12f;

    public PlayerStateMachine PlayerStateMachine { get; private set; }

    public PlayerIdle idleState { get; private set; }
    public PlayerMove moveState { get; private set; }
    public PlayerDead deadState { get; private set; }

    private int deadCount = 0;


    [Header("��ġ��")]
    [SerializeField] private GameObject moveMap;
    [SerializeField] private string portalName = "CaughtPoint";

    [Header("�̴ϰ���")]
    [SerializeField] private GameObject miniGame;
    private bool isInGame = false;

    [Header("������")]
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
            Debug.Log($"{deadCount} �� ������ ����");
            if (deadCount >= 2)
            {
                Debug.Log("�÷��̾� ���~");
                PlayerStateMachine.ChangeState(deadState);
            }
        }

        //�ͽſ��� ������ ��
        if (collision.CompareTag("Ghost"))
        {
            Debug.Log("�ͽſ��� ����");

            Transform portal = moveMap.transform.Find(portalName);
            transform.position = portal.position;
        }

        // �̴ϰ��� ����
        if (collision.CompareTag("MiniGame"))
        {
            Debug.Log("�̴ϰ��� ����");
            isInGame = true;
        }

        // ������ ������
        //if (collision.CompareTag("Generator"))
        //{
        //    Debug.Log("������ �۵�����");
        //    isInGenerator = true;
        //}

        // �������� ��ȣ�ۿ��߰�
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("MiniGame"))
        {
            Debug.Log("�̴ϰ��� ����");
            isInGame = false;
        }

        //if (collision.CompareTag("Generator"))
        //{
        //    Debug.Log("������ �۵� �Ұ���");
        //    isInGenerator = false;
        //}
    }

    private void OpenMiniGame()
    {
        Debug.Log("�̴ϰ��� ����");
        if (miniGame != null)
            miniGame.SetActive(true);

        // �÷��̾� ���� ���
    }

    public void BecomeGhost()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color c = sr.color;
        c.a = 0.5f;
        sr.color = c;

        //�ٸ�������� �Ⱥ��̰�
        //if (!isMine())
        //{
        //    gameObject.SetActive(false); 
        //}

        Collider2D col = GetComponent<Collider2D>();
        col.enabled = false;
    }

    public void AnimationTrigger() => PlayerStateMachine.currentState.AnimationFinishTrigger();
}
