using Photon.Pun;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Protein : Ghost, IPunObservable
{
    public override GhostState moveState { get; protected set; }
    private GhostState normalIdleState;
    private GhostState normalMoveState;
    private GhostState skillIdleState;
    private GhostState skillMoveState;

    [Header("단백질섭취(벌크업)")]
    [SerializeField] private float ProteinDuration = 10f; //벌크업 지속시간
    [SerializeField] private float ProteincooldownDuration = 5f;    //쿨타임
    [SerializeField] private float buffedSpeed = 2f;
    private bool isProtein = false; //단백질 섭취 여부
    private bool isProteinCooldown = false; //단백질 쿨타임 여부
    [SerializeField] private float proteinTimer; //지속시간 타이머
    [SerializeField] private float proteinCooldownTimer; //쿨타임 타이머

    private Vector2 lastDir = Vector2.right;   // 기본값은 오른쪽

    private float originalSpeed;
    private Vector3 originalScale;

    private PhotonView photonView;

    private Vector3 networkedPosition;
    private Vector3 networkedVelocity;
    private float lerpSpeed = 10f;
    private bool networkedIsMoving;
    private float networkedDirX;
    private float networkedDirY;


    protected override void Awake()
    {

        base.Awake();
        photonView = GetComponent<PhotonView>();

        if (ghostStateMachine == null)
            ghostStateMachine = new GhostStateMachine();

        useSkillState = new ProteinSkill_State(this, ghostStateMachine, "ProteinSkill");

        normalIdleState = new ProteinIdle(this, ghostStateMachine, "Idle");
        normalMoveState = new ProteinMove(this, ghostStateMachine, "Move");
        skillIdleState = new ProteinSkill_Idle(this, ghostStateMachine, "SkillIdle");
        skillMoveState = new ProteinSkill_Move(this, ghostStateMachine, "SkillMove");

        moveState = normalMoveState;
        idleState = normalIdleState;

        ghostStateMachine.Initialize(idleState);
    }

    protected override void Start()
    {
        if (photonView.IsMine)
        {
            base.Start();
            originalSpeed = moveSpeed;
            originalScale = transform.localScale;
            CinemachineCamera cam = FindFirstObjectByType<CinemachineCamera>();
            if (cam != null)
                cam.Follow = transform;
        }

        if (!photonView.IsMine)
        {
            Light2D light = GetComponentInChildren<Light2D>();
            if (light != null)
                light.enabled = false;
        }
        
    }

    protected override void Update()
    {
        base.Update();

        if (photonView.IsMine)
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.Alpha1) && !isProtein && !isProteinCooldown)
            {
                photonView.RPC("DrinkProtein", RpcTarget.All);
            }
        }
        else
        {
            //이동보간
            transform.position = Vector3.Lerp(transform.position, networkedPosition, Time.deltaTime * lerpSpeed);

            anim.SetBool("IsMoving", networkedIsMoving);
            anim.SetFloat("DirX", networkedDirX);
            anim.SetFloat("DirY", networkedDirY);

            if (Mathf.Abs(networkedDirX) >= Mathf.Abs(networkedDirY))
            {
                sr.flipX = networkedDirX < 0;
            }
        }

        if (isProtein)
        {
            proteinTimer -= Time.deltaTime;
            if (proteinTimer <= 0)
            {
                photonView.RPC("EndProtein", RpcTarget.All);
            }
        }

        if(isProteinCooldown)
        {
            proteinCooldownTimer -= Time.deltaTime;
            if (proteinCooldownTimer <= 0)
            {
                isProteinCooldown = false;
                proteinCooldownTimer = ProteincooldownDuration;
            }
        }
    }

    protected override void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            base.FixedUpdate();
        }
        else
        {
            rb.linearVelocity = networkedVelocity;
        }
    }

    [PunRPC]
    private void DrinkProtein()
    {
        ActivateProtein();
        ghostStateMachine.ChangeState(useSkillState);
    }

    public void EndDrinkProtein()
    {
        anim.SetBool("IsProtein", false);

        idleState = skillIdleState;
        moveState = skillMoveState;

        Vector2 input = MoveInput;

        if (input == Vector2.zero)
        {
            ghostStateMachine.ChangeState(skillIdleState);
        }
        else
        {
            ghostStateMachine.ChangeState(skillMoveState);
        }
    }

    [PunRPC]
    private void ActivateProtein()
    {
        isProtein = true;
        proteinTimer = ProteinDuration;
        moveSpeed = buffedSpeed;
        transform.localScale = originalScale * 1.5f;

        Debug.Log("단백질 섭취");
    }

    [PunRPC]
    private void EndProtein()
    {
        isProtein = false;
        isProteinCooldown = true;
        proteinCooldownTimer = ProteincooldownDuration;

        moveSpeed = originalSpeed;
        transform.localScale = originalScale;
        Debug.Log("단백질 섭취 종료");

        anim.SetBool("IsOriginal", true);
    }

    public override void UpdateAnimParam(Vector2 input)
    {
        if (input != Vector2.zero)
            lastDir = input.normalized;

        bool isMoving = input != Vector2.zero;

        anim.SetBool("IsMoving", isMoving);
        anim.SetFloat("DirX", lastDir.x);
        anim.SetFloat("DirY", lastDir.y);

        if (Mathf.Abs(lastDir.x) >= Mathf.Abs(lastDir.y))
        {
            sr.flipX = lastDir.x < 0;
        }

    }

    public void EndProteinRelease()
    {
        anim.SetBool("IsOriginal", false);

        idleState = normalIdleState;
        moveState = normalMoveState;

        Vector2 input = MoveInput;

        if (input == Vector2.zero)
        {
            ghostStateMachine.ChangeState(idleState);
        }
        else
        {
            ghostStateMachine.ChangeState(moveState);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // 내가 보냄
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.localScale);
            stream.SendNext(rb.linearVelocity);
            stream.SendNext((int)ghostStateMachine.CurrentStateType);
            stream.SendNext(facingDir);
            stream.SendNext(facingUpDir);
            stream.SendNext(anim.GetBool("IsMoving"));
            stream.SendNext(anim.GetFloat("DirX"));
            stream.SendNext(anim.GetFloat("DirY"));
        }
        else // 상대방이 보낸 것 받음
        {
            networkedPosition = (Vector3)stream.ReceiveNext();
            transform.localScale = (Vector3)stream.ReceiveNext();
            networkedVelocity = (Vector2)stream.ReceiveNext();
            GhostStateType receivedState = (GhostStateType)stream.ReceiveNext();
            int receivedFacingDir = (int)stream.ReceiveNext();
            int receivedFacingUpDir = (int)stream.ReceiveNext();
            SetFacingDirection(receivedFacingDir, receivedFacingUpDir);
            networkedIsMoving = (bool)stream.ReceiveNext();
            networkedDirX = (float)stream.ReceiveNext();
            networkedDirY = (float)stream.ReceiveNext();

            if (ghostStateMachine != null && ghostStateMachine.CurrentStateType != receivedState)
            {
                switch (receivedState)
                {
                    case GhostStateType.Idle:
                        ghostStateMachine.ChangeState(idleState);
                        break;
                    case GhostStateType.Move:
                        ghostStateMachine.ChangeState(moveState);
                        break;
                }
            }
        }
    }
}
