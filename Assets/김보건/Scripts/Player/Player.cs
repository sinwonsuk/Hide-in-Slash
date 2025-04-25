using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using Photon.Pun;
using Photon.Realtime;
using Unity.Cinemachine;
using UnityEditor.Rendering;

public enum EscapeType
{
    Dead = 0,         // Ż�� ����
    ExitDoor = 1,     // Ż�ⱸ Ż��
    Hatch = 2         // ������ Ż��
}

public class Player : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private GameObject spriteObject; // SpriteRenderer �ִ� ������Ʈ
    [SerializeField] private Transform lightObject;   // Light2D �ִ� ������Ʈ

    private Vector3 networkedPosition;
    private Vector3 networkedVelocity;
    private float lerpSpeed = 10f;
    private bool networkedIsMoving;
    private float networkedDirX;
    private float networkedDirY;
    private float lightAngle;

    private Vector2 lastDir = Vector2.right;   // �⺻���� ������

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }

    public SpriteRenderer sr { get; private set; }

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
    [SerializeField] private bool hasInvisiblePotion = false;    //���������� ������ �ִ��� 
    private bool isInvisible = false;           //�������� ����
    [SerializeField] private float invisibleTimer;   // ����Ÿ�̸�(������ �ø����ʵ�)

    [Header("��������׷��̵�")]
    [SerializeField] private bool hasUpgradedFlashlight = false;
    [SerializeField] private Light2D flashlight;
    [SerializeField] private float upgradedRadius = 8f; // ���׷��̵� �� �ݰ�
    [SerializeField] private float defaultRadius = 3.5f; // �⺻ �ݰ�
    [SerializeField] private float upgradeLightDuration = 10f;
    [SerializeField] private PolygonCollider2D lightCollider;   // ������ �ݶ��̴�
    private bool isUpgradedLight = false;
    [SerializeField] private float upgradedLightTimer; // ������
    private bool isLightOn = false; // �⺻�������Ű��
    private bool isBlinking = false;
    private float blinkTimer = 0f;
    private float blinkInterval = 0.3f; // �����̴� ����
    private Vector2[] defaultColliderPoints;

    [Header("����Ű")]
    [SerializeField] private bool hasPrisonKey = false; // ����Ű�� ������ �ִ���
    private bool isInPrisonDoor = false;

    [Header("��ġ��")]
    [SerializeField] private GameObject moveMap;
    [SerializeField] private string portalName = "CaughtPoint";

    [Header("�̴ϰ���")]
    [SerializeField] private GameObject miniGame;
    private bool isInGame = false;
    private bool isInMiniGame = false; // �̴ϰ��� ��

    [Header("������")]
    [SerializeField] private GameObject generator;
    private bool isInGenerator = false; // ������ �۵���?
    private bool isGenerator = false; // ������ �۵�����

    [Header("����")]
    private bool isInShop = false;

    [Header("������")]
    [SerializeField] private bool hasHatch = false;
    private bool isInHatch = false;

    [Header("��")]
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
        EventManager.RegisterEvent(EventType.UseUpgradedLight, UpGradeLight);
        EventManager.RegisterEvent(EventType.UsePrisonKey, usePrisonKeyItem);
        EventManager.RegisterEvent(EventType.UseHatch, useHatchItem);
        EventManager.RegisterEvent(EventType.LightRestored, TurnOnLight);
    }

    private void OnDisable()
    {
        EventManager.UnRegisterEvent(EventType.UseEnergyDrink, BecomeBoost);
        EventManager.UnRegisterEvent(EventType.UseInvisiblePotion, BecomeInvisible);
        EventManager.UnRegisterEvent(EventType.UseUpgradedLight, UpGradeLight);
        EventManager.UnRegisterEvent(EventType.UsePrisonKey, usePrisonKeyItem);
        EventManager.UnRegisterEvent(EventType.UseHatch, useHatchItem);
        EventManager.UnRegisterEvent(EventType.LightRestored, TurnOnLight);
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
            // ���� ��ġ�� �ε巴�� ����
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

        if (hasUpgradedFlashlight && Input.GetKeyDown(KeyCode.Alpha3))
        {
            photonView.RPC("UpGradeLight", RpcTarget.All);
        }
        else if (!hasUpgradedFlashlight)
        {
            Debug.Log("���׷��̵� ������ ����");
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
            Debug.Log("����Ű ����");
        }

        if (hasHatch && isInHatch && Input.GetKeyDown(KeyCode.Alpha5))
        {
            useHatchItem();
        }
        else if (hasHatch && !isInHatch && Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("�������� ����� �� ���� ���");
        }
        else
        {
            Debug.Log("��ȣ�ۿ�ȵ�");
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
            Debug.Log("�� ����");
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

            // 5�� ������ ������ �����̱� ����
            if (upgradedLightTimer <= 5f && !isBlinking)
            {
                isBlinking = true;
                blinkTimer = blinkInterval;
            }

            // �����̴� ���̸� ����Ʈ On/Off �ݺ�
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

        // ����Ʈ ��ġ ����
        lightObject.localPosition = Vector3.zero;

        // ����Ʈ ȸ���� ���� (���� ȸ��)
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
        // �¿� ����
        if (x > 0 && !facingRight) Flip();
        else if (x < 0 && facingRight) Flip();

        // ���� ����
        if (y > 0 && !facingUp) FlipVertical();
        else if (y < 0 && facingUp) FlipVertical();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 'PlayerSight' �±� ���� ����Ʈ ������Ʈ�� ����
        if (collision.CompareTag("PlayerSight"))
            return;

        if (collision.CompareTag("Prison"))
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

        if (collision.CompareTag("PrisonDoor"))
        {
            isInPrisonDoor = true;
            if (hasPrisonKey)
            {
                Debug.Log("����Ű ��밡��");
            }
        }

        if (collision.CompareTag("Hatch"))
        {
            isInHatch = true;
        }

        // ������ ������
        if (collision.CompareTag("Generator"))
        {
            Debug.Log("������ �۵�����");
            isInGenerator = true;
        }

        // ����
        if (collision.CompareTag("Shop"))
        {
            Debug.Log("���� ��ȣ�ۿ� ����");
            isInShop = true;
        }
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

        if (collision.CompareTag("Hatch"))
        {
            isInHatch = false;
        }

        if (collision.CompareTag("Generator"))
        {
            Debug.Log("������ �۵� �Ұ���");
            isInGenerator = false;
        }

        if (collision.CompareTag("Shop"))
        {
            Debug.Log("���� �̿� �Ұ���");
            isInShop = false;
        }

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

         Debug.Log("���� ���� ���");
        
    }

    private void ResetTransparency()
    {
        Color c = sr.color;
        c.a = 1f;
        sr.color = c;

        isInvisible = false;
        Debug.Log("�������� ����");
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

        Debug.Log("������ ���׷��̵�");
    }

    private void ResetFlashlight()
    {
        flashlight.pointLightOuterRadius = defaultRadius;

        isUpgradedLight = false;
        isBlinking = false;
        blinkTimer = 0f;

        ScalePolygonCollider(1.0f); // ���� ũ��� ����
        Debug.Log("������ ���׷��̵� ��");
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

        Debug.Log("������ �ٽ� ����");
    }

    private void OpenMap()
    {
        Debug.Log("�� ����");
        if (hasMap)
        {
            isInMap = true;
            // �� UI ����

        }
        else
        {
            Debug.Log("�� ����");
        }
    }
    private void CloseMap()
    {
        isInMap = false;
        Debug.Log("�� �ݱ�");
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

    // ����Ű ���

    private void usePrisonKeyItem()
    {
        Debug.Log("���� Ű ���");
        EventManager.TriggerEvent(EventType.OpenPrisonDoor);
        hasPrisonKey = false;
    }

    private void useHatchItem()
    {
        Debug.Log("������ ���");
        escapeState.SetEscapeType(EscapeType.Hatch);
        PlayerStateMachine.ChangeState(escapeState);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // ������ ����
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
            //�ٸ��� �����ޱ�
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
