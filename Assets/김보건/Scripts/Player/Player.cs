using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }

    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;

    [Header("이동")]
    public float moveSpeed = 5f;

    public PlayerStateMachine PlayerStateMachine { get; private set; }

    public PlayerIdle idleState { get; private set; }
    public PlayerMove moveState { get; private set; }
    public PlayerDead deadState { get; private set; }

    private int deadCount = 0;

    [Header("에너지드링크")]
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float boostedSpeed = 10f;
    [SerializeField] private float boostDuration = 10f;
    [SerializeField] private bool hasEnergyDrink = false;     //에너지드링크 가지고 있는지
    private bool isBoosted = false;           //에너지드링크 사용중인지 여부
    [SerializeField] private float boostTimer;        // 부스트타이머(디버깅용 시리얼필드)

    [Header("투명포션")]
    [SerializeField] private float invisibleDuration = 5f;
    [SerializeField] private bool hasInvisiblePotion = false;    //투명물약을 가지고 있는지 
    private bool isInvisible = false;           //투명상태 여부
    [SerializeField] private float invisibleTimer;   // 투명타이머(디버깅용 시리얼필드)

    [Header("손전등업그레이드")]
    [SerializeField] private bool hasUpgradedFlashlight = false;
    [SerializeField] private Light2D flashlight;
    [SerializeField] private float upgradedRadius = 8f; // 업그레이드 시 반경
    [SerializeField] private float defaultRadius = 3.5f; // 기본 반경
    private bool isupgradedFlashlight = false; // 업그레이드 상태 

    [Header("납치됨")]
    [SerializeField] private GameObject moveMap;
    [SerializeField] private string portalName = "CaughtPoint";

    [Header("미니게임")]
    [SerializeField] private GameObject miniGame;
    private bool isInGame = false;

    [Header("발전기")]
    [SerializeField] private GameObject generator;
    private bool isInGenerator = false;

    [Header("상점")]
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
            Debug.Log("투명 물약 없음");
        }

        if (hasEnergyDrink && Input.GetKeyDown(KeyCode.Alpha2))
        {
            BecomeBoost();
        }
        else if (!hasEnergyDrink)
        {
            Debug.Log("에너지드링크 없음");
        }

        if(hasUpgradedFlashlight && Input.GetKeyDown(KeyCode.Alpha3))
        {
            UpGradeLight();
        }
        else if (!hasUpgradedFlashlight)
        {
            Debug.Log("업그레이드 손전등 없음");
        }

        // 투명화 지속시간

        if (isInvisible)
        {
            invisibleTimer -= Time.deltaTime;
            if (invisibleTimer <= 0f)
            {
                ResetTransparency();
            }
        }

        // 에너지드링크 지속시간
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

        // 상점
        //if(collision.CompareTag("Shop"))
        //{
        //    Debug.Log("상점 상호작용 가능");
        //    isInShop = true;
        //}
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

        //if (collision.CompareTag("Shop"))
        //{
        //    Debug.Log("상점 이용 불가능");
        //    isInShop = false;
        //}
    }

    private void OpenMiniGame()
    {
        Debug.Log("미니게임 시작");
        if (miniGame != null)
            miniGame.SetActive(true);

        // 플레이어 조작 잠금
    }

    //플레이어죽음
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

    //투명 물약 관련
    public void BecomeInvisible()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color c = sr.color;
        c.a = 0.5f;
        sr.color = c;

        hasInvisiblePotion = false;
        isInvisible = true;
        invisibleTimer = invisibleDuration;

        Debug.Log("투명 물약 사용");

        //보스한테 안보이게
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
        Debug.Log("투명상태 해제");
    }

    // 에너지 드링크 관련
    public void BecomeBoost()
    {
        moveSpeed = boostedSpeed;
        hasEnergyDrink = false;
        isBoosted = true;
        boostTimer = boostDuration;
        Debug.Log("속도 버프");
    }
    private void ResetMoveSpeed()
    {
        moveSpeed = baseSpeed;
        isBoosted = false;
        Debug.Log("속도 버프 종료");
    }

    //손전등 업그레이드
    private void UpGradeLight()
    {
        isupgradedFlashlight = true;
        flashlight.pointLightOuterRadius = upgradedRadius;
        Debug.Log("손전등 업그레이드");
    }

    //상점 관련
    public void BuyInvisiblePotion()
    {
        hasInvisiblePotion = true;
        Debug.Log("투명 포션 구매");
    }

    public void BuyEnergyDrink()
    {
        hasEnergyDrink = true;
        Debug.Log("에너지 드링크 구매");
    }

    public void AnimationTrigger() => PlayerStateMachine.currentState.AnimationFinishTrigger();
}
