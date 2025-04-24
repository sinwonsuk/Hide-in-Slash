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
    Dead = 0,         // 탈출 못함
    ExitDoor = 1,     // 탈출구 탈출
    Hatch = 2         // 개구멍 탈출
}

public class Player : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private GameObject spriteObject; // SpriteRenderer 있는 오브젝트
    [SerializeField] private Transform lightObject;   // Light2D 있는 오브젝트

    private Vector3 networkedPosition;
    private Vector3 networkedVelocity;
    private float lerpSpeed = 10f;
    private bool networkedIsMoving;
    private float networkedDirX;
    private float networkedDirY;
    private float lightAngle; 

    private Vector2 lastDir = Vector2.right;   // 기본값은 오른쪽

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }

    public SpriteRenderer sr { get; private set; }

    // 서버용 트랜스폼 스케일변수
    public float posX, posY, posZ;  
    public float scaleX, scaleY, scaleZ;

    public int facingDir { get; private set; } = 1;
    public int facingUpDir { get; private set; } = 1;   // 1 = 위, -1 = 아래
    protected bool facingRight = true;
    protected bool facingUp = true;

    [Header("이동")]
    public float moveSpeed = 5f;

    public PlayerStateMachine PlayerStateMachine { get; private set; }

    public PlayerIdle idleState { get; private set; }
    public PlayerMove moveState { get; private set; }
    public PlayerDead deadState { get; private set; }
    public PlayerEscapeState escapeState { get; private set; }
    public EscapeType escapeType { get; private set; } = EscapeType.Dead;
    public int EscapeCode => (int)escapeType;


    private int countLife = 2;  //목숨

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
    [SerializeField] private float upgradeLightDuration = 10f;
    [SerializeField] private PolygonCollider2D lightCollider;   // 손전등 콜라이더
    private bool isUpgradedLight = false;
    [SerializeField] private float upgradedLightTimer; // 디버깅용
    private bool isLightOn = false; // 기본손전등껏다키기
    private bool isBlinking = false;
    private float blinkTimer = 0f;
    private float blinkInterval = 0.3f; // 깜빡이는 간격
    private Vector2[] defaultColliderPoints;

    [Header("감옥키")]
    [SerializeField] private bool hasPrisonKey = false; // 감옥키를 가지고 있는지
    private bool isInPrisonDoor = false;

    [Header("납치됨")]
    [SerializeField] private GameObject moveMap;
    [SerializeField] private string portalName = "CaughtPoint";

    [Header("미니게임")]
    [SerializeField] private GameObject miniGame;
    private bool isInGame = false;      
    private bool isInMiniGame = false; // 미니게임 중

    [Header("발전기")]
    [SerializeField] private GameObject generator;
    private bool isInGenerator = false; // 발전기 작동중?
    private bool isGenerator = false; // 발전기 작동가능

    [Header("상점")]
    private bool isInShop = false;

    [Header("개구멍")]
    [SerializeField] private bool hasHatch = false;
    private bool isInHatch = false;

    [Header("맵")]
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
        MapEventManager.RegisterEvent(MapEventType.UseEnergyDrink, BecomeBoost);
        MapEventManager.RegisterEvent(MapEventType.UseInvisiblePotion, BecomeInvisible);
        MapEventManager.RegisterEvent(MapEventType.UseUpgradedLight, UpGradeLight);
        MapEventManager.RegisterEvent(MapEventType.UsePrisonKey, usePrisonKeyItem);
        MapEventManager.RegisterEvent(MapEventType.UseHatch, useHatchItem);
    }

    private void OnDisable()
    {
        MapEventManager.RegisterEvent(MapEventType.UseEnergyDrink, BecomeBoost);
        MapEventManager.RegisterEvent(MapEventType.UseInvisiblePotion, BecomeInvisible);
        MapEventManager.RegisterEvent(MapEventType.UseUpgradedLight, ToggleLight);
        MapEventManager.RegisterEvent(MapEventType.UsePrisonKey, usePrisonKeyItem);
        MapEventManager.RegisterEvent(MapEventType.UseHatch, useHatchItem);
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
            // 상대방 위치를 부드럽게 보간
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

        if (hasUpgradedFlashlight && Input.GetKeyDown(KeyCode.Alpha3))
        {
            photonView.RPC("UpGradeLight", RpcTarget.All);
        }
        else if (!hasUpgradedFlashlight)
        {
            Debug.Log("업그레이드 손전등 없음");
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleLight();
        }

        if (hasPrisonKey && isInPrisonDoor && Input.GetKeyDown(KeyCode.Alpha4))
        {
            usePrisonKeyItem();
        }
        else if (!hasPrisonKey && Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("감옥키 없음");
        }

        if (hasHatch && isInHatch && Input.GetKeyDown(KeyCode.Alpha5))
        {
            useHatchItem();
        }
        else if (hasHatch && !isInHatch && Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("개구멍을 사용할 수 없는 장소");
        }
        else
        {
            Debug.Log("상호작용안됨");
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (!isInMap)
                OpenMap();
            else
                CloseMap();
        }
        else if (!hasMap && Input.GetKeyDown(KeyCode.Alpha6))
        {
            Debug.Log("맵 없음");
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

        // 손전등 업그레이드 지속시간
        if (isUpgradedLight)
        {
            upgradedLightTimer -= Time.deltaTime;

            // 5초 남았을 때부터 깜빡이기 시작
            if (upgradedLightTimer <= 5f && !isBlinking)
            {
                isBlinking = true;
                blinkTimer = blinkInterval;
            }

            // 깜빡이는 중이면 라이트 On/Off 반복
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

    }


    private void FixedUpdate()
    {
        PlayerStateMachine.currentState.FixedUpdate();
        Vector3 pos = transform.position;
        //transform.position = new Vector3(pos.x, pos.y, pos.y);

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

        // 라이트 위치 고정
        lightObject.localPosition = Vector3.zero;

        // 라이트 회전만 조절 (로컬 회전)
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
        // 좌우 반전
        if (x > 0 && !facingRight) Flip();
        else if (x < 0 && facingRight) Flip();

        // 상하 반전
        if (y > 0 && !facingUp) FlipVertical();
        else if (y < 0 && facingUp) FlipVertical();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 'PlayerSight' 태그 가진 라이트 오브젝트는 무시
        if (collision.CompareTag("PlayerSight"))
            return;

        if (collision.CompareTag("Prison"))
        {
            countLife--;
            Debug.Log($"{countLife} 번 감옥에 들어옴");
            if (countLife <= 0)
            {
                Debug.Log("플레이어 사망~");
                PlayerStateMachine.ChangeState(deadState);
            }

            if (hasPrisonKey)
            {
                Debug.Log("감옥키 사용가능");
                usePrisonKeyItem();
            }
        }

        //귀신에게 잡혔을 때
        if (collision.CompareTag("Ghost") )
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

        if (collision.CompareTag("ExitDoor"))
        {
            Debug.Log("탈출구 : 탈출가능");
            escapeState.SetEscapeType(EscapeType.ExitDoor);
            PlayerStateMachine.ChangeState(escapeState);
        }

        if (collision.CompareTag("PrisonDoor"))
        {
            isInPrisonDoor = true; 
            if (hasPrisonKey)
            {
                Debug.Log("감옥키 사용가능");
            }
        }

        if (collision.CompareTag("Hatch"))
        {
            isInHatch = true;
        }

        // 발전기 돌리기
        if (collision.CompareTag("Generator"))
        {
            Debug.Log("발전기 작동가능");
            isInGenerator = true;
        }

        // 상점
        if (collision.CompareTag("Shop"))
        {
            Debug.Log("상점 상호작용 가능");
            isInShop = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("MiniGame"))
        {
            Debug.Log("미니게임 종료");
            isInGame = false;
        }

        if (collision.CompareTag("Prison"))
        {
            isInPrisonDoor = false; // 감옥 범위 밖으로 나감
        }

        if (collision.CompareTag("Hatch"))
        {
            isInHatch = false;
        }

        if (collision.CompareTag("Generator"))
        {
            Debug.Log("발전기 작동 불가능");
            isInGenerator = false;
        }

        if (collision.CompareTag("Shop"))
        {
            Debug.Log("상점 이용 불가능");
            isInShop = false;
        }

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
        Color c = sr.color;
        c.a = 0.5f;
        sr.color = c;

        Collider2D col = GetComponent<Collider2D>();
        col.enabled = false;

    }

    //투명 물약 관련
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

         Debug.Log("투명 물약 사용");
        
    }

    private void ResetTransparency()
    {
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

        Debug.Log("손전등 업그레이드");
    }

    private void ResetFlashlight()
    {
        flashlight.pointLightOuterRadius = defaultRadius;

        isUpgradedLight = false;
        isBlinking = false;
        blinkTimer = 0f;

        ScalePolygonCollider(1.0f); // 원래 크기로 복귀
        Debug.Log("손전등 업그레이드 끝");
    }

    private void ToggleLight()
    {
        if (!photonView.IsMine)
            return; 

        isLightOn = !isLightOn;
        flashlight.enabled = isLightOn;

        photonView.RPC("RPC_SetFlashlight", RpcTarget.Others, isLightOn);

        Debug.Log(isLightOn ? "손전등 켜짐" : "손전등 꺼짐");
    }

    private void OpenMap()
    {
        Debug.Log("맵 열기");
        if (hasMap)
        {
            isInMap = true;
            // 맵 UI 열기
            // MapUIManager.OpenMap();
        }
        else
        {
            Debug.Log("맵 없음");
        }
    }
    private void CloseMap()
    {
        isInMap = false;
        Debug.Log("맵 닫기");
        // mapUI.SetActive(false); 등
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

    // 감옥키 사용

    private void usePrisonKeyItem()
    {
        Debug.Log("감옥 키 사용");
        MapEventManager.TriggerEvent(MapEventType.OpenPrisonDoor);
        hasPrisonKey = false;
    }
    
    private void useHatchItem()
    {
        Debug.Log("개구멍 사용");
        escapeState.SetEscapeType(EscapeType.Hatch);
        PlayerStateMachine.ChangeState(escapeState);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 내정보 보냄
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
            //다른애 정보받기
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
