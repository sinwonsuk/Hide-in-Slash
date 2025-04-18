using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }

    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;

    [Header("�̵�")]
    public float moveSpeed = 5f;

    public PlayerStateMachine PlayerStateMachine { get; private set; }

    public PlayerIdle idleState { get; private set; }
    public PlayerMove moveState { get; private set; }
    public PlayerDead deadState { get; private set; }

    private int deadCount = 0;

    [Header("�������帵ũ")]
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float boostedSpeed = 10f;
    [SerializeField] private float boostDuration = 10f;
    [SerializeField] private bool hasEnergyDrink = false;     //�������帵ũ ������ �ִ���
    private bool isBoosted = false;           //�������帵ũ ��������� ����
    [SerializeField] private float boostTimer;        // �ν�ƮŸ�̸�(������ �ø����ʵ�)

    [Header("��������")]
    [SerializeField] private float invisibleDuration = 5f;
    [SerializeField] private bool hasInvisiblePotion = false;    //�������� ������ �ִ��� 
    private bool isInvisible = false;           //������� ����
    [SerializeField] private float invisibleTimer;   // ����Ÿ�̸�(������ �ø����ʵ�)

    [Header("��������׷��̵�")]
    [SerializeField] private bool hasUpgradedFlashlight = false;
    [SerializeField] private Light2D flashlight;
    [SerializeField] private float upgradedRadius = 8f; // ���׷��̵� �� �ݰ�
    [SerializeField] private float defaultRadius = 3.5f; // �⺻ �ݰ�
    private bool isupgradedFlashlight = false; // ���׷��̵� ���� 

    [Header("��ġ��")]
    [SerializeField] private GameObject moveMap;
    [SerializeField] private string portalName = "CaughtPoint";

    [Header("�̴ϰ���")]
    [SerializeField] private GameObject miniGame;
    private bool isInGame = false;

    [Header("������")]
    [SerializeField] private GameObject generator;
    private bool isInGenerator = false;

    [Header("����")]
    private bool isInShop = false;


    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        PlayerStateMachine = new PlayerStateMachine();

        idleState = new PlayerIdle(this, PlayerStateMachine, "Idle");
        moveState = new PlayerMove(this, PlayerStateMachine, "Move");
        deadState = new PlayerDead(this, PlayerStateMachine, "Dead");

        flashlight.pointLightOuterRadius = defaultRadius;

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

        if (hasInvisiblePotion && Input.GetKeyDown(KeyCode.Alpha1))
        {
            BecomeInvisible();
        }
        else if (!hasInvisiblePotion)
        {
            Debug.Log("���� ���� ����");
        }

        if (hasEnergyDrink && Input.GetKeyDown(KeyCode.Alpha2))
        {
            BecomeBoost();
        }
        else if (!hasEnergyDrink)
        {
            Debug.Log("�������帵ũ ����");
        }

        if(hasUpgradedFlashlight && Input.GetKeyDown(KeyCode.Alpha3))
        {
            UpGradeLight();
        }
        else if (!hasUpgradedFlashlight)
        {
            Debug.Log("���׷��̵� ������ ����");
        }

        // ����ȭ ���ӽð�

        if (isInvisible)
        {
            invisibleTimer -= Time.deltaTime;
            if (invisibleTimer <= 0f)
            {
                ResetTransparency();
            }
        }

        // �������帵ũ ���ӽð�
        if (isBoosted)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
            {
                ResetMoveSpeed();
            }
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

        // ����
        //if(collision.CompareTag("Shop"))
        //{
        //    Debug.Log("���� ��ȣ�ۿ� ����");
        //    isInShop = true;
        //}
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

        //if (collision.CompareTag("Shop"))
        //{
        //    Debug.Log("���� �̿� �Ұ���");
        //    isInShop = false;
        //}
    }

    private void OpenMiniGame()
    {
        Debug.Log("�̴ϰ��� ����");
        if (miniGame != null)
            miniGame.SetActive(true);

        // �÷��̾� ���� ���
    }

    //�÷��̾�����
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

    //���� ���� ����
    public void BecomeInvisible()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color c = sr.color;
        c.a = 0.5f;
        sr.color = c;

        hasInvisiblePotion = false;
        isInvisible = true;
        invisibleTimer = invisibleDuration;

        Debug.Log("���� ���� ���");

        //�������� �Ⱥ��̰�
        //if (!isMine())
        //{
        //    gameObject.SetActive(false); 
        //}
    }

    private void ResetTransparency()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color c = sr.color;
        c.a = 1f;
        sr.color = c;

        isInvisible = false;
        Debug.Log("������� ����");
    }

    // ������ �帵ũ ����
    public void BecomeBoost()
    {
        moveSpeed = boostedSpeed;
        hasEnergyDrink = false;
        isBoosted = true;
        boostTimer = boostDuration;
        Debug.Log("�ӵ� ����");
    }
    private void ResetMoveSpeed()
    {
        moveSpeed = baseSpeed;
        isBoosted = false;
        Debug.Log("�ӵ� ���� ����");
    }

    //������ ���׷��̵�
    private void UpGradeLight()
    {
        isupgradedFlashlight = true;
        flashlight.pointLightOuterRadius = upgradedRadius;
        Debug.Log("������ ���׷��̵�");
    }

    //���� ����
    public void BuyInvisiblePotion()
    {
        hasInvisiblePotion = true;
        Debug.Log("���� ���� ����");
    }

    public void BuyEnergyDrink()
    {
        hasEnergyDrink = true;
        Debug.Log("������ �帵ũ ����");
    }

    public void AnimationTrigger() => PlayerStateMachine.currentState.AnimationFinishTrigger();
}
