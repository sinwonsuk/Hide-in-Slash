using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using Photon.Pun;
using Photon.Realtime;
using Unity.Cinemachine;

public enum EscapeType
{
    Dead = 0,         // 죽은상태
    ExitDoor = 1,     // 탈출구
    Hatch = 2         // 개구멍
}

public class Player : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private GameObject spriteObject; 
    [SerializeField] private Transform lightObject; 

    private Vector3 networkedPosition;
    private Vector3 networkedVelocity;
    private float lerpSpeed = 10f;
    private bool networkedIsMoving;
    private float networkedDirX;
    private float networkedDirY;
    private float lightAngle;

    private Vector2 lastDir = Vector2.right;   // 기본방향

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }

    public SpriteRenderer sr { get; private set; }

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

    private int countLife = 2;  //플레이어 수명

    [Header("이동물약")]
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float boostedSpeed = 10f;
    [SerializeField] private float boostDuration = 10f;
    [SerializeField] private bool hasEnergyDrink = false;     
    private bool isBoosted = false;         
    [SerializeField] private float boostTimer;        //디버그용 시리얼라이즈필드

    [Header("투명물약")]
    [SerializeField] private float invisibleDuration = 5f;
    [SerializeField] private bool hasInvisiblePotion = false;  
    private bool isInvisible = false;           
    [SerializeField] private float invisibleTimer;   //디버그용 시리얼라이즈필드

    [Header("손전등")]
    [SerializeField] private bool hasUpgradedFlashlight = false;
    [SerializeField] private Light2D flashlight;
    [SerializeField] private float upgradedRadius = 8f; 
    [SerializeField] private float defaultRadius = 3.5f;
    [SerializeField] private float upgradeLightDuration = 10f;
    [SerializeField] private PolygonCollider2D lightCollider;   
    private bool isUpgradedLight = false;
    [SerializeField] private float upgradedLightTimer; // 업글손전등 시간
    private bool isLightOn = false; // 켜짐
    private bool isBlinking = false;
    private float blinkTimer = 0f;
    private float blinkInterval = 0.3f; // 깜빡속도
    private Vector2[] defaultColliderPoints;

    [Header("감옥키")]
    [SerializeField] private bool hasPrisonKey = false; 
    private bool isInPrisonDoor = false;

    [Header("잡혀서 감옥감")]
    [SerializeField] private GameObject moveMap;
    [SerializeField] private string portalName = "CaughtPoint";

    [Header("미니게임")]
    [SerializeField] private GameObject miniGame;
    private bool isInGame = false;
    private bool isInMiniGame = false; 

    [Header("발전기")]
    [SerializeField] private GameObject generator;
    private bool isInGenerator = false; 
    private bool isGenerator = false; 

    [Header("상점")]
    private bool isInShop = false;

    [Header("개구멍")]
    [SerializeField] private bool hasHatch = false;
    private bool isInHatch = false;

    [Header("지도")]
    [SerializeField] private GameObject mapUI;
    [SerializeField] private bool hasMap = false;
    private bool isInMap = false;


    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();

        PlayerStateMachine = new PlayerStateMachine();

        idleState = new PlayerIdle(this, PlayerStateMachine, "Idle");
        moveState = new PlayerMove(this, PlayerStateMachine, "Move");
        deadState = new PlayerDead(this, PlayerStateMachine, "Dead");
        escapeState = new PlayerEscapeState(this, PlayerStateMachine, "Escape");

        flashlight.pointLightOuterRadius = defaultRadius;
        defaultColliderPoints = lightCollider.points;

        flashlight.enabled = false;

    }

    private void OnEnable()
    {
        EventManager.RegisterEvent(EventType.UseEnergyDrink, BecomeBoost);
        EventManager.RegisterEvent(EventType.UseInvisiblePotion, BecomeInvisible);
        EventManager.RegisterEvent(EventType.UseUpgradedLight, UseUpgradedLightHandler);
        EventManager.RegisterEvent(EventType.UsePrisonKey, usePrisonKeyItem);
        EventManager.RegisterEvent(EventType.UseHatch, useHatchItem);
        EventManager.RegisterEvent(EventType.LightRestored, TurnOnLight);
        EventManager.RegisterEvent(EventType.UseMap, HasTriggerMap);
    }

    private void OnDisable()
    {
        EventManager.UnRegisterEvent(EventType.UseEnergyDrink, BecomeBoost);
        EventManager.UnRegisterEvent(EventType.UseInvisiblePotion, BecomeInvisible);
        EventManager.UnRegisterEvent(EventType.UseUpgradedLight, UseUpgradedLightHandler);
        EventManager.UnRegisterEvent(EventType.UsePrisonKey, usePrisonKeyItem);
        EventManager.UnRegisterEvent(EventType.UseHatch, useHatchItem);
        EventManager.UnRegisterEvent(EventType.LightRestored, TurnOnLight);
        EventManager.UnRegisterEvent(EventType.UseMap, HasTriggerMap);
    }

    private void Start()
    {

        PlayerStateMachine.Initialize(idleState);

        if (photonView.IsMine)
        {
            CinemachineCamera cam = FindFirstObjectByType<CinemachineCamera>();
            if (cam != null)
                cam.Follow = transform;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            PlayerStateMachine.currentState.Update();

        }
        else
        {
            //이동보간
            transform.position = Vector3.Lerp(transform.position, networkedPosition, Time.deltaTime * lerpSpeed);
            rb.linearVelocity = networkedVelocity;

            anim.SetBool("IsMoving", networkedIsMoving);
            anim.SetFloat("DirX", networkedDirX);
            anim.SetFloat("DirY", networkedDirY);

            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * facingDir;
            scale.y = Mathf.Abs(scale.y) * facingUpDir;
            transform.localScale = scale;

            lightObject.localRotation = Quaternion.Euler(0f, 0f, lightAngle);
        }

        if (isInGame && Input.GetKeyDown(KeyCode.E))
        {
            OpenMiniGame();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleLight();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!hasMap)
            {
                Debug.Log("지도 없음");
            }
            else
            {
                if (!isInMap)
                    OpenMap();
                else
                    CloseMap();
            }
        }

        // 투명물약지속시간

        if (isInvisible)
        {
            invisibleTimer -= Time.deltaTime;
            if (invisibleTimer <= 0f)
            {
                ResetTransparency();
            }
        }

        // 이동물약지속시간
        if (isBoosted)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
            {
                ResetMoveSpeed();
            }
        }

        // 손전등 지속시간
        if (isUpgradedLight)
        {
            upgradedLightTimer -= Time.deltaTime;

            // 5초지나면 깜빡
            if (upgradedLightTimer <= 5f && !isBlinking)
            {
                isBlinking = true;
                blinkTimer = blinkInterval;
            }

            if (isBlinking)
            {
                float timeRatio = upgradedLightTimer / 5f;
                blinkInterval = Mathf.Lerp(0.05f, 0.3f, timeRatio);

                blinkTimer -= Time.deltaTime;
                if (blinkTimer <= 0f)
                {
                    flashlight.enabled = !flashlight.enabled;
                    blinkTimer = blinkInterval;
                }
            }


            if (upgradedLightTimer <= 0f)
            {
                ResetFlashlight();
                flashlight.enabled = true;
            }
        }

        // 테스트용
        if (photonView.IsMine && PlayerStateMachine.currentState != deadState && Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("플레이어죽음");
            PlayerStateMachine.ChangeState(deadState);
        }
    }


    private void FixedUpdate()
    {
        PlayerStateMachine.currentState.FixedUpdate();
        Vector3 pos = transform.position;
        //transform.position = new Vector3(pos.x, pos.y, pos.y);
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
        if (input != Vector2.zero)
            lastDir = input.normalized;

        bool isMoving = input != Vector2.zero;

        anim.SetBool("IsMoving", isMoving);
        anim.SetFloat("DirX", lastDir.x);
        anim.SetFloat("DirY", lastDir.y);

    }

    public void RotateLight(Vector2 moveInput)
    {
        if (moveInput == Vector2.zero)
            return;

        float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;

        lightObject.localPosition = Vector3.zero;

        lightObject.localRotation = Quaternion.Euler(0f, 0f, angle);

        if (photonView.IsMine)
        {
            lightAngle = angle;
        }
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
        if (x > 0 && !facingRight) Flip();
        else if (x < 0 && facingRight) Flip();

        if (y > 0 && !facingUp) FlipVertical();
        else if (y < 0 && facingUp) FlipVertical();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.IsMine) return;

        if (collision.CompareTag("PlayerSight"))
            return;

        if (collision.CompareTag("Prison"))
        {
            countLife--;
            Debug.Log($"{countLife} 번감옥이동");
            if (countLife <= 0)
            {
                Debug.Log("너죽음");
                PlayerStateMachine.ChangeState(deadState);
            }

            if (hasPrisonKey)
            {
                Debug.Log("감옥문열기가능");
                usePrisonKeyItem();
            }
        }

        if (collision.CompareTag("Ghost"))
        {
            Debug.Log("감옥으로이동");

            Transform portal = moveMap.transform.Find(portalName);
            transform.position = portal.position;
        }

        if (collision.CompareTag("MiniGame"))
        {
            Debug.Log("미니게임가능");
            isInGame = true;
        }

        if (collision.CompareTag("ExitDoor"))
        {
            Debug.Log("탈출구가능");
            escapeState.SetEscapeType(EscapeType.ExitDoor);
            PlayerStateMachine.ChangeState(escapeState);
        }

        if (collision.CompareTag("PrisonDoor"))
        {
            isInPrisonDoor = true;
            if (hasPrisonKey)
            {
                Debug.Log("감옥해방가능");
            }
        }

        if (collision.CompareTag("Hatch"))
        {
            isInHatch = true;
        }


        if (collision.CompareTag("Generator"))
        {
            Debug.Log("발전기가능");
            isInGenerator = true;
        }

        if (collision.CompareTag("Shop"))
        {
            Debug.Log("상점이용가능");
            isInShop = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!photonView.IsMine) return;
        if (collision.CompareTag("MiniGame"))
        {
            Debug.Log("미니게임가능");
            isInGame = false;
        }

        if (collision.CompareTag("Prison"))
        {
            isInPrisonDoor = false; 
        }

        if (collision.CompareTag("Hatch"))
        {
            isInHatch = false;
        }

        if (collision.CompareTag("Generator"))
        {
            Debug.Log("발전기 작동불가");
            isInGenerator = false;
        }

        if (collision.CompareTag("Shop"))
        {
            Debug.Log("상점이용불가");
            isInShop = false;
        }

    }

    private void OpenMiniGame()
    {
        Debug.Log("미니게임시작");
        if (miniGame != null)
            miniGame.SetActive(true);

    }

    public void BecomeGhost()
    {
        if(!photonView.IsMine)
           return; 

        Color c = sr.color;
        c.a = 0.5f;
        sr.color = c;

        photonView.RPC("SetGhostVisual", RpcTarget.Others);

    }

    [PunRPC]
    public void SetGhostVisual()
    {
        gameObject.SetActive(false);
    }

    //투명물약
    public void BecomeInvisible()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        Color c = sr.color;
        c.a = 0.5f;
        sr.color = c;

        hasInvisiblePotion = false;
        isInvisible = true;
        invisibleTimer = invisibleDuration;

        photonView.RPC("SetInvisibilityVisual", RpcTarget.Others, true);

        Debug.Log("투명버프");
        
    }

    private void ResetTransparency()
    {
        Color c = sr.color;
        c.a = 1f;
        sr.color = c;

        isInvisible = false;
        photonView.RPC("SetInvisibilityVisual", RpcTarget.Others, false);
        Debug.Log("투명버프끝");
    }

    [PunRPC]
    public void SetTransparencyVisual(bool isInvisible)
    {
        if (!isInvisible)
        {
            Color visible = sr.color;
            visible.a = 1f;
            sr.color = visible;
            return;
        }

        // RPC신호받는 사람기준에서 역할따라 "RPC"쏜 사람 투명도 조절
        if (PhotonNetwork.LocalPlayer.CustomProperties["Role"].ToString().StartsWith("Mon"))  
        {
            // 아예 x
            Color c = sr.color;
            c.a = 0f;
            sr.color = c;
        }
        else
        {
            // 아군은 반투명
            Color c = sr.color;
            c.a = 0.5f;
            sr.color = c;
        }
    }

    // 이동속도업글
    public void BecomeBoost()
    {
        moveSpeed = boostedSpeed;
        hasEnergyDrink = false;
        isBoosted = true;
        boostTimer = boostDuration;
        Debug.Log("이속버프");
    }
    private void ResetMoveSpeed()
    {
        moveSpeed = baseSpeed;
        isBoosted = false;
        Debug.Log("이속버프끝");
    }


    //손전등업글
    private void UseUpgradedLightHandler()
    {
        // RPC 호출로 네트워크에 상태를 전파
        photonView.RPC("UpGradeLight", RpcTarget.All);
    }

    [PunRPC]
    private void UpGradeLight()
    {
        flashlight.pointLightOuterRadius = upgradedRadius;
        flashlight.enabled = true;

        hasUpgradedFlashlight = false;
        isUpgradedLight = true;
        upgradedLightTimer = upgradeLightDuration;

        float scaleRatio = upgradedRadius / defaultRadius;
        ScalePolygonCollider(scaleRatio);

        Debug.Log("손전등 업글");
    }


    private void ResetFlashlight()
    {
        flashlight.pointLightOuterRadius = defaultRadius;

        isUpgradedLight = false;
        isBlinking = false;
        blinkTimer = 0f;

        ScalePolygonCollider(1.0f); // 손전등원래대로
        Debug.Log("손전등업그레이드 종료");
    }

    private void ToggleLight()
    {
        if (!photonView.IsMine)
            return;

        isLightOn = !isLightOn;
        flashlight.enabled = isLightOn;

        photonView.RPC("RPC_SetFlashlight", RpcTarget.Others, isLightOn);

        Debug.Log(isLightOn ? "������ ����" : "������ ����");
    }

    private void TurnOnLight()
    {
        if (!photonView.IsMine)
            return;

        isLightOn = true;
        flashlight.enabled = true;
        photonView.RPC("RPC_SetFlashlight", RpcTarget.Others, true);

        Debug.Log("손전등켜짐");
    }

    private void HasTriggerMap()
    {
        hasMap = true;
    }

    private void OpenMap()
    {
        hasMap = true;
        Debug.Log(" 맵열림");
        if (hasMap)
        {
            isInMap = true;
            if (mapUI != null)
                mapUI.SetActive(true);

        }
        else
        {
            Debug.Log("맵 없음");
        }
    }
    private void CloseMap()
    {
        isInMap = false;
        if (mapUI != null)
            mapUI.SetActive(false);
        Debug.Log("맵 닫음");
    }

    [PunRPC]
    public void RPC_SetFlashlight(bool turnOn)
    {
        isLightOn = turnOn;
        flashlight.enabled = turnOn;
    }

    private void ScalePolygonCollider(float scale)
    {
        Vector2[] scaled = new Vector2[defaultColliderPoints.Length];
        for (int i = 0; i < scaled.Length; i++)
        {
            scaled[i] = defaultColliderPoints[i] * scale;
        }
        lightCollider.points = scaled;
    }


    private void usePrisonKeyItem()
    {
        Debug.Log("감옥 키");
        EventManager.TriggerEvent(EventType.OpenPrisonDoor);
        hasPrisonKey = false;
    }

    private void useHatchItem()
    {
        Debug.Log("개구멍사용");
        escapeState.SetEscapeType(EscapeType.Hatch);
        PlayerStateMachine.ChangeState(escapeState);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //신호보냄
            stream.SendNext(transform.position);
            stream.SendNext(transform.localScale);
            stream.SendNext(rb.linearVelocity);
            stream.SendNext((int)PlayerStateMachine.currentState.StateType);
            stream.SendNext((int)escapeType);
            stream.SendNext(facingDir);
            stream.SendNext(facingUpDir);
            stream.SendNext(anim.GetBool("IsMoving"));
            stream.SendNext(anim.GetFloat("DirX"));
            stream.SendNext(anim.GetFloat("DirY"));
            stream.SendNext(lightAngle);
        }
        else
        {
            //신호받음
            networkedPosition = (Vector3)stream.ReceiveNext();
            transform.localScale = (Vector3)stream.ReceiveNext();
            networkedVelocity = (Vector2)stream.ReceiveNext();
            PlayerStateType receivedState = (PlayerStateType)stream.ReceiveNext();
            EscapeType receivedEscape = (EscapeType)stream.ReceiveNext();
            facingDir = (int)stream.ReceiveNext();
            facingUpDir = (int)stream.ReceiveNext();
            networkedIsMoving = (bool)stream.ReceiveNext();
            networkedDirX = (float)stream.ReceiveNext();
            networkedDirY = (float)stream.ReceiveNext();
            lightAngle = (float)stream.ReceiveNext();

            if (PlayerStateMachine.currentState.StateType != receivedState)
            {
                switch (receivedState)
                {
                    case PlayerStateType.Idle:
                        PlayerStateMachine.ChangeState(idleState);
                        break;
                    case PlayerStateType.Move:
                        PlayerStateMachine.ChangeState(moveState);
                        break;
                    case PlayerStateType.Dead:
                        PlayerStateMachine.ChangeState(deadState);
                        break;
                    case PlayerStateType.Escape:
                        escapeState.SetEscapeType(receivedEscape);
                        PlayerStateMachine.ChangeState(escapeState);
                        break;
                }
            }
        }
    }

    public void AnimationTrigger() => PlayerStateMachine.currentState.AnimationFinishTrigger();
}
