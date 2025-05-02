using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using Photon.Pun;
using Photon.Realtime;
using Unity.Cinemachine;
using System.Text.RegularExpressions;
using ExitGames.Client.Photon;

public enum EscapeType
{
    Dead = 0,         // 죽은상태
    ExitDoor = 1,     // 탈출구
    Hatch = 2         // 개구멍
}

public class Player : MonoBehaviourPun, IPunObservable
{

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        canvas = GameObject.FindGameObjectWithTag("Dark").GetComponent<Canvas>();
        canvas.enabled = false;
        originalSpeed = moveSpeed;

        PlayerStateMachine = new PlayerStateMachine();

        idleState = new PlayerIdle(this, PlayerStateMachine, "Idle");
        moveState = new PlayerMove(this, PlayerStateMachine, "Move");
        deadState = new PlayerDead(this, PlayerStateMachine, "Dead");
        escapeState = new PlayerEscapeState(this, PlayerStateMachine, "Escape");

        flashLight.pointLightOuterRadius = defaultRadius;
        defaultColliderPoints = lightCollider.points;

        // 인스펙터창에 무조건 있어야함 긴급 땜빵
        GameObject gameObject = GameObject.Find("SpawnPlayerProfile");
        profileSlotManager = gameObject.GetComponent<ProfileSlotManager>();

        flashLight.enabled = false;

    }

    private void OnEnable()
    {
        EventManager.RegisterEvent(EventType.UseEnergyDrink, BecomeBoost);
        EventManager.RegisterEvent(EventType.UseInvisiblePotion, BecomeInvisible);
        EventManager.RegisterEvent(EventType.UseUpgradedLight, UseUpgradedLightHandler);
        EventManager.RegisterEvent(EventType.UsePrisonKey, usePrisonKeyItem);
        EventManager.RegisterEvent(EventType.UseHatch, HasHatchItem);
        EventManager.RegisterEvent(EventType.LightOn, TurnOnLight);
        EventManager.RegisterEvent(EventType.LightOff, TurnOffLight);
        EventManager.RegisterEvent(EventType.UseMap, HasTriggerMap);
        EventManager.RegisterEvent(EventType.InEventPlayer, SetZeroVelocity);
        EventManager.RegisterEvent(EventType.OutEventPlayer, ResumeMovement);
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        EventManager.UnRegisterEvent(EventType.UseEnergyDrink, BecomeBoost);
        EventManager.UnRegisterEvent(EventType.UseInvisiblePotion, BecomeInvisible);
        EventManager.UnRegisterEvent(EventType.UseUpgradedLight, UseUpgradedLightHandler);
        EventManager.UnRegisterEvent(EventType.UsePrisonKey, usePrisonKeyItem);
        EventManager.UnRegisterEvent(EventType.UseHatch, HasHatchItem);
        EventManager.UnRegisterEvent(EventType.LightOn, TurnOnLight);
        EventManager.UnRegisterEvent(EventType.LightOff, TurnOffLight);
        EventManager.UnRegisterEvent(EventType.UseMap, HasTriggerMap);
        EventManager.UnRegisterEvent(EventType.InEventPlayer, SetZeroVelocity);
        EventManager.UnRegisterEvent(EventType.OutEventPlayer, ResumeMovement);
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void Start()
    {

        PlayerStateMachine.Initialize(idleState);

        if (photonView.IsMine)
        {
            CinemachineCamera cam = FindFirstObjectByType<CinemachineCamera>();
            if (cam != null)
                cam.Follow = transform;

            GameObject playerCanvas = GameObject.Find("PlayerCanvas");
            if (playerCanvas != null)
            {
                Transform minimapObj = playerCanvas.transform.Find("MiniMapRender");
                if (minimapObj != null)
                {
                    minimap = minimapObj.gameObject;
                    minimap.SetActive(false); 
                }
            }
        }

        // 초기화 되기전에 아무것도 되지 마라 
        isInitialized = true;
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            PlayerStateMachine.currentState.Update();

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

            if (Input.GetKeyUp(KeyCode.R))
            {
                if (hasHatch && isInHatch)
                {
                    useHatchItem();
                    Debug.Log("개구멍사용");
                }
                else
                {
                    Debug.Log("사용못함");
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
                        flashLight.enabled = !flashLight.enabled;

                        photonView.RPC("SetFlashlightBlink", RpcTarget.Others, flashLight.enabled);

                        blinkTimer = blinkInterval;
                    }
                }


                if (upgradedLightTimer <= 0f)
                {
                    photonView.RPC("ResetFlashlight", RpcTarget.All);
                    flashLight.enabled = true;
                }
            }
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

    public void ApplySlow(float factor)
    {
        moveSpeed = originalSpeed * factor;
    }

    public void ResetSpeed()
    {
        moveSpeed = originalSpeed;
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

    public void ResumeMovement()
    {
        if (PlayerStateMachine != null)
        {
            PlayerStateMachine.ChangeState(idleState);
        }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!photonView.IsMine) return;

        if (collision.collider.CompareTag("Ghost"))
        {

            countLife--;
            Debug.Log("고스트 충돌");
         
            if (countLife <= 0)
            {
                Debug.Log("너죽음");
                EventManager.TriggerEvent(EventType.PlayerHpZero);
                profileSlotManager.photonView.RPC("SyncProfileState", RpcTarget.All, PhotonNetwork.LocalPlayer, ProfileState.deadSprite);
                PlayerStateMachine.ChangeState(deadState);
            }
            else if (countLife == 1)
            {
                photonView.RPC("CaughtByGhost", RpcTarget.AllBuffered);
                if(photonView.IsMine)
                    StartCoroutine(UpdateCameraConfinerDelayed());
                EventManager.TriggerEvent(EventType.PlayerHpOne);
                profileSlotManager.photonView.RPC("SyncProfileState", RpcTarget.All, PhotonNetwork.LocalPlayer, ProfileState.prisonSprite);
                Debug.Log("너한번잡힘 한 번 더 잡히면 너 죽음");

            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.IsMine) return;

        if (collision.CompareTag("PlayerSight"))
            return;

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
            // 이벤트로 isInPrisonDoor = true
            EventManager.TriggerEvent(EventType.InPrisonDoor, true);
        }

        if (collision.CompareTag("Hatch"))
        {
            isInHatch = true;

        }


    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.CompareTag("PrisonDoor"))
        {
            isInPrisonDoor = false;
            if (hasPrisonKey)
            {
                
            }
            // 이벤트로 isInPrisonDoor = true
            EventManager.TriggerEvent(EventType.InPrisonDoor, false);
        }

        if (collision.CompareTag("Hatch"))
        {
            isInHatch = false;
        }


    }

    [PunRPC]
    public void CaughtByGhost()
    {
        Transform portal = moveMap.transform.Find(portalName);
        if (portal != null)
            transform.position = portal.position;
        
    }

    private IEnumerator UpdateCameraConfinerDelayed()
    {
        CinemachineCamera cam = FindFirstObjectByType<CinemachineCamera>();
        CinemachineConfiner2D confiner = cam.GetComponent<CinemachineConfiner2D>();
        confiner.BoundingShape2D = moveMap.GetComponent<Collider2D>();
        canvas.enabled = true;
        yield return new WaitForSeconds(0.05f);
        confiner.InvalidateBoundingShapeCache();
        yield return new WaitForSeconds(0.25f);
        canvas.enabled = false;

    }


    public void BecomeGhost()
    {
        if (!photonView.IsMine)
            return;

        Color c = sr.color;
        c.a = 0.5f;
        sr.color = c;

        gameObject.tag = "DeadPlayer";

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

        photonView.RPC("SetTransparencyVisual", RpcTarget.Others, true);

        Debug.Log("투명버프");

    }

    private void ResetTransparency()
    {
        Color c = sr.color;
        c.a = 1f;
        sr.color = c;

        isInvisible = false;
        photonView.RPC("SetTransparencyVisual", RpcTarget.Others, false);
        Debug.Log("투명버프끝");
    }

    [PunRPC]
    public void SetTransparencyVisual(bool isInvisible)
    {
        if (!isInvisible)
        {
            // 투명 해제
            Color visible = sr.color;
            visible.a = 1f;
            sr.color = visible;
            flashLight.enabled = isLightOn;
            circleLight.enabled = true;
            return;
        }

        if (!PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Role", out object roleObj) || roleObj == null)
        {
            Debug.LogWarning("Role 없음");
            return;
        }

        string roleName = roleObj.ToString();

        // 투명물약마시면 몬스터한테는 안보임
        bool isReceiverMonster = NetworkProperties.instance.GetMonsterStates(roleName);

        if (isReceiverMonster)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f); // 완전 투명
            flashLight.enabled = false;
            circleLight.enabled = false;
        }
        else
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.5f); // 반투명
            flashLight.enabled = isLightOn;
            circleLight.enabled = true;
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
        if (!photonView.IsMine)
            return;

        photonView.RPC("UpGradeLight", RpcTarget.All);
    }

    [PunRPC]
    private void UpGradeLight()
    {
        flashLight.pointLightOuterRadius = upgradedRadius;
        flashLight.enabled = true;

        //hasUpgradedFlashlight = false;
        isUpgradedLight = true;
        upgradedLightTimer = upgradeLightDuration;

        float scaleRatio = upgradedRadius / defaultRadius;
        ScalePolygonCollider(scaleRatio);

        Debug.Log("손전등 업글");
    }


    [PunRPC]
    private void ResetFlashlight()
    {
        flashLight.pointLightOuterRadius = defaultRadius;

        isUpgradedLight = false;
        isBlinking = false;
        blinkTimer = 0f;

        ScalePolygonCollider(1.0f); // 손전등원래대로
        Debug.Log("손전등업그레이드 종료");
    }

    [PunRPC]
    public void SetFlashlightBlink(bool turnOn)
    {
        flashLight.enabled = turnOn;
    }

    private void ToggleLight()
    {
        if (!photonView.IsMine)
            return;

        isLightOn = !isLightOn;
        flashLight.enabled = isLightOn;

        photonView.RPC("SetFlashlight", RpcTarget.Others, isLightOn);

    }

    private void TurnOnLight()
    {
        if (!photonView.IsMine)
            return;

        isLightOn = true;
        flashLight.enabled = true;
        photonView.RPC("SetFlashlight", RpcTarget.Others, true);

        Debug.Log("손전등켜짐");
    }

    private void TurnOffLight()
    {
        if (!photonView.IsMine)
            return;

        isLightOn = false;
        flashLight.enabled = false;

        photonView.RPC("SetFlashlight", RpcTarget.Others, false);

        Debug.Log("손전등꺼짐");
    }

    private void HasTriggerMap()
    {
        hasMap = true;
    }

    private void OpenMap()
    {

        isInMap = true;
        Debug.Log(" 맵열림");
        if (hasMap && minimap != null)
        {

            minimap.SetActive(true);
        }
        else
        {
            Debug.Log("맵 없음");
        }
    }
    private void CloseMap()
    {
        isInMap = false;
        if (minimap != null)
        {
            minimap.SetActive(false);
        }
        Debug.Log("맵 닫음");
    }

    [PunRPC]
    public void SetFlashlight(bool turnOn)
    {
        isLightOn = turnOn;
        flashLight.enabled = turnOn;
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

    private void HasPrisonKey()
    {
        hasPrisonKey = true;
        Debug.Log("감옥키획득");
    }

    private void usePrisonKeyItem()
    {
        Debug.Log("감옥 키");
        EventManager.TriggerEvent(EventType.OpenPrisonDoor);
        hasPrisonKey = false;
    }

    private void HasHatchItem()
    {
        hasHatch = true;
    }

    private void useHatchItem()
    {
        Debug.Log("개구멍사용");
        escapeState.SetEscapeType(EscapeType.Hatch);
        PlayerStateMachine.ChangeState(escapeState);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (isInitialized == false)
        {
            return;
        }

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

            networkedPosition = (Vector3)stream.ReceiveNext();

            // 로컬 스케일 처리
            transform.localScale = (Vector3)stream.ReceiveNext();

            // 속도 처리
            networkedVelocity = (Vector2)stream.ReceiveNext();


            // PlayerStateType 형변환 안전 체크
            object receivedStateObject = stream.ReceiveNext();
            if (receivedStateObject is int)
            {
                PlayerStateType receivedState = (PlayerStateType)(int)receivedStateObject;
            }
            else
            {
                Debug.LogError("Received state is not of type int");
            }

            // EscapeType 형변환 안전 체크
            object receivedEscapeObject = stream.ReceiveNext();
            if (receivedEscapeObject is int)
            {
                EscapeType receivedEscape = (EscapeType)(int)receivedEscapeObject;
            }
            else
            {
                Debug.LogError("Received escape type is not of type int");
            }

            // facingDir, facingUpDir 형변환 안전 체크
            object receivedFacingDir = stream.ReceiveNext();
            if (receivedFacingDir is int)
            {
                facingDir = (int)receivedFacingDir;
            }
            else
            {
                Debug.LogError("Received facingDir is not of type int");
            }

            object receivedFacingUpDir = stream.ReceiveNext();
            if (receivedFacingUpDir is int)
            {
                facingUpDir = (int)receivedFacingUpDir;
            }
            else
            {
                Debug.LogError("Received facingUpDir is not of type int");
            }

            // networkedIsMoving 형변환 안전 체크
            object receivedIsMoving = stream.ReceiveNext();
            if (receivedIsMoving is bool)
            {
                networkedIsMoving = (bool)receivedIsMoving;
            }
            else
            {
                Debug.LogError("Received isMoving is not of type bool");
            }

            // networkedDirX, networkedDirY 형변환 안전 체크
            object receivedDirX = stream.ReceiveNext();
            if (receivedDirX is float)
            {
                networkedDirX = (float)receivedDirX;
            }
            else
            {
                Debug.LogError("Received DirX is not of type float");
            }

            object receivedDirY = stream.ReceiveNext();
            if (receivedDirY is float)
            {
                networkedDirY = (float)receivedDirY;
            }
            else
            {
                Debug.LogError("Received DirY is not of type float");
            }

            // lightAngle 형변환 안전 체크
            object receivedLightAngle = stream.ReceiveNext();
            if (receivedLightAngle is float)
            {
                lightAngle = (float)receivedLightAngle;
            }
            else
            {
                Debug.LogError("Received lightAngle is not of type float");
            }
        }
    }

    public void OnEvent(EventData data)
    {
        if (data.Code != EVENT_BLACKOUT) return;

        float duration = (float)data.CustomData;
        if (photonView.IsMine)
        {
            flashLight.enabled = false;
            circleLight.enabled = false;
            StartCoroutine(DelayedTurnOn(duration));
        }
    }

    private IEnumerator DelayedTurnOn(float duration)
    {
        yield return new WaitForSeconds(duration);
        flashLight.enabled = true;
        circleLight.enabled = true;
    }


    public void AnimationTrigger() => PlayerStateMachine.currentState.AnimationFinishTrigger();

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

    public ProfileSlotManager profileSlotManager;

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }

    public SpriteRenderer sr { get; private set; }
    private Canvas canvas;

    public int facingDir { get; private set; } = 1;
    public int facingUpDir { get; private set; } = 1;   // 1 = ��, -1 = �Ʒ�
    protected bool facingRight = true;
    protected bool facingUp = true;

    [Header("�̵�")]
    public float moveSpeed = 5f;
    private float originalSpeed;
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
    private bool hasEnergyDrink = false;
    private bool isBoosted = false;
    private float boostTimer;

    [Header("투명물약")]
    [SerializeField] private float invisibleDuration = 5f;
    [SerializeField] private bool hasInvisiblePotion = false;
    private bool isInvisible = false;
    [SerializeField] private float invisibleTimer;   //디버그용 시리얼라이즈필드

    [Header("손전등")]
    [SerializeField] private Light2D flashLight;
    [SerializeField] private Light2D circleLight;
    [SerializeField] private float upgradedRadius = 8f;
    [SerializeField] private float defaultRadius = 3.5f;
    [SerializeField] private float upgradeLightDuration = 10f;
    [SerializeField] private PolygonCollider2D lightCollider;
    [SerializeField] private float upgradedLightTimer; // 업글손전등 시간
    private bool isUpgradedLight = false;
    private bool isLightOn = false; // 켜짐
    private bool isBlinking = false;
    private float blinkTimer = 0f;
    private float blinkInterval = 0.3f; // 깜빡속도
    private Vector2[] defaultColliderPoints;
    //private bool hasUpgradedFlashlight = false;
    private const byte EVENT_BLACKOUT = 1;

    [Header("감옥키")]
    [SerializeField] private bool hasPrisonKey = false;
    private bool isInPrisonDoor = false;

    [Header("잡혀서 감옥감")]
    [SerializeField] private GameObject moveMap;
    [SerializeField] private string portalName = "PrisonSpawnPoint";

    [Header("개구멍")]
    [SerializeField] private bool hasHatch = false;
    private bool isInHatch = false;

    [Header("지도")]
    private GameObject minimap;
    [SerializeField] private bool hasMap = false;
    private bool isInMap = false;

    bool isInitialized = false;
}
