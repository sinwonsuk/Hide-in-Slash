using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum EscapeType
{
    Dead = 0,         // Ż�� ����
    ExitDoor = 1,     // Ż�ⱸ Ż��
    Hatch = 2         // ������ Ż��
}

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject spriteObject; // SpriteRenderer �ִ� ������Ʈ
    [SerializeField] private Transform lightObject;   // Light2D �ִ� ������Ʈ

    private Vector2 lastDir = Vector2.right;   // �⺻���� ������

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }

    public SpriteRenderer sr { get; private set; }

    // �κ��丮 Action
    public Action useEnergyDrink;
    public Action useInvisiblePotion;
    public Action useUpgradedLight;
    public Action usePrisonKey;

    // ������ Ʈ������ �����Ϻ���
    public float posX, posY, posZ;  
    public float scaleX, scaleY, scaleZ;

    public int facingDir { get; private set; } = 1;
    public int facingUpDir { get; private set; } = 1;   // 1 = ��, -1 = �Ʒ�
    protected bool facingRight = true;
    protected bool facingUp = true;

    [Header("�̵�")]
    public float moveSpeed = 5f;

    public PlayerStateMachine PlayerStateMachine { get; private set; }

    public PlayerIdle idleState { get; private set; }
    public PlayerMove moveState { get; private set; }
    public PlayerDead deadState { get; private set; }
    public PlayerEscapeState escapeState { get; private set; }
    public EscapeType escapeType { get; private set; } = EscapeType.Dead;
    public int EscapeCode => (int)escapeType;


    private int countLife = 2;  //���

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
    [SerializeField] private float upgradeLightDuration = 10f;
    private bool isUpgradedLight = false;
    [SerializeField] private float upgradedLightTimer; // ������

    [Header("����Ű")]
    [SerializeField] private bool hasPrisonKey = false; // ����Ű�� ������ �ִ���
    private bool isInPrisonDoor = false;

    [Header("��ġ��")]
    [SerializeField] private GameObject moveMap;
    [SerializeField] private string portalName = "CaughtPoint";

    [Header("�̴ϰ���")]
    [SerializeField] private GameObject miniGame;
    private bool isInGame = false;      
    private bool isInMiniGame = false; // �̴ϰ��� ��(��¦��)
    private bool isMiniGameSuccess = false; // �̴ϰ��� ����

    [Header("������")]
    [SerializeField] private GameObject generator;
    private bool isInGenerator = false; // ������ �۵���?
    private bool isGenerator = false; // 
    private bool isGeneratorSuccess = false; // ������ ����
    private bool isStarForce = false; // ��Ÿ���� ���� ����

    [Header("����")]
    private bool isInShop = false;



    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        PlayerStateMachine = new PlayerStateMachine();

        idleState = new PlayerIdle(this, PlayerStateMachine, "Idle");
        moveState = new PlayerMove(this, PlayerStateMachine, "Move");
        deadState = new PlayerDead(this, PlayerStateMachine, "Dead");
        escapeState = new PlayerEscapeState(this, PlayerStateMachine, "Escape");

        flashlight.pointLightOuterRadius = defaultRadius;

    }

    private void Start()
    {
        PlayerStateMachine.Initialize(idleState);
        useEnergyDrink = BecomeBoost;
        useInvisiblePotion = BecomeInvisible;
        useUpgradedLight = UpGradeLight;
        usePrisonKey = usePrisonKeyItem;
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

        if(hasPrisonKey && isInPrisonDoor && Input.GetKeyDown(KeyCode.Alpha4))
        {
            usePrisonKeyItem();
        }
        else if (!hasPrisonKey && Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("����Ű ����");
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

        // ������ ���׷��̵� ���ӽð�
        if (isUpgradedLight)
        {
            upgradedLightTimer -= Time.deltaTime;

           
            if (upgradedLightTimer <= 0f)
            {
                ResetFlashlight();
            }
        }

        
    }


    private void FixedUpdate()
    {
        PlayerStateMachine.currentState.FixedUpdate();
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y, pos.y);

        posX = transform.position.x;
        posY = transform.position.y;
        posZ = transform.position.z;

        scaleX = transform.localScale.x;
        scaleY = transform.localScale.y;
        scaleZ = transform.localScale.z;
    }
    public void SetEscapeType(EscapeType type)
    {
        escapeType = type;
    }

    public void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
    }

    public void SetZeroVelocity()
    {
        rb.linearVelocity = Vector2.zero;
    }

    public void UpdateAnimParam(Vector2 input)
    {
        // �̵� �Է��� ���� ���� lastDir ����
        if (input != Vector2.zero)
            lastDir = input.normalized;   // ũ�⸦ 1�� ���� �δ� �� ���

        bool isMoving = input != Vector2.zero;

        // DirX��DirY ���� �׻� lastDir �� �ִ´�
        anim.SetBool("IsMoving", isMoving);
        anim.SetFloat("DirX", lastDir.x);
        anim.SetFloat("DirY", lastDir.y);

    }

    public void RotateLight(Vector2 moveInput)
    {
        if (moveInput == Vector2.zero)
            return;

        float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;

        // ��ġ ����
        lightObject.localPosition = Vector3.zero;

        // ȸ���� ���� (���� ȸ��)
        lightObject.localRotation = Quaternion.Euler(0f, 0f, angle);
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
        // �¿� ����
        if (x > 0 && !facingRight) Flip();
        else if (x < 0 && facingRight) Flip();

        // ���� ����
        if (y > 0 && !facingUp) FlipVertical();
        else if (y < 0 && facingUp) FlipVertical();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Prison"))
        {
            countLife--;
            Debug.Log($"{countLife} �� ������ ����");
            if (countLife <= 0)
            {
                Debug.Log("�÷��̾� ���~");
                PlayerStateMachine.ChangeState(deadState);
            }

            if (hasPrisonKey)
            {
                Debug.Log("����Ű ��밡��");
                usePrisonKeyItem();
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

        if (collision.CompareTag("ExitDoor"))
        {
            Debug.Log("Ż�ⱸ : Ż�Ⱑ��");
            escapeState.SetEscapeType(EscapeType.ExitDoor);
            PlayerStateMachine.ChangeState(escapeState);
        }

        if (collision.CompareTag("Hatch"))
        {
            Debug.Log("������ : Ż�Ⱑ��");
            escapeState.SetEscapeType(EscapeType.Hatch);
            PlayerStateMachine.ChangeState(escapeState);
        }

        if (collision.CompareTag("PrisonDoor"))
        {
            isInPrisonDoor = true; 
            if (hasPrisonKey)
            {
                Debug.Log("����Ű ��밡��");
            }
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

        if (collision.CompareTag("Prison"))
        {
            isInPrisonDoor = false; // ���� ���� ������ ����
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
        Color c = sr.color;
        c.a = 0.5f;
        sr.color = c;

        Collider2D col = GetComponent<Collider2D>();
        col.enabled = false;

    }

    //���� ���� ����
    public void BecomeInvisible()
    {
        Color c = sr.color;
        c.a = 0.5f;
        sr.color = c;

        hasInvisiblePotion = false;
        isInvisible = true;
        invisibleTimer = invisibleDuration;

        Debug.Log("���� ���� ���");
    }

    private void ResetTransparency()
    {
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
        flashlight.pointLightOuterRadius = upgradedRadius;
        hasUpgradedFlashlight = false;
        isUpgradedLight = true;
        upgradedLightTimer = upgradeLightDuration;

        Debug.Log("������ ���׷��̵�");
    }

    private void ResetFlashlight()
    {
        flashlight.pointLightOuterRadius = defaultRadius;
        isUpgradedLight = false;

        Debug.Log("������ ���׷��̵� ��");
    }

    private void usePrisonKeyItem()
    {
        Debug.Log("���� Ű ���");
        MapEventManager.TriggerEvent(MapEventType.OpenPrisonDoor);
        hasPrisonKey = false;
    }

    private IEnumerator ShutdownLight(float delay)
    {
        while (true) {

        }
    }

    public void AnimationTrigger() => PlayerStateMachine.currentState.AnimationFinishTrigger();
}
