using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PukeGirl : Ghost, IPunObservable
{
    public override GhostState moveState { get; protected set; }
    private GhostState vomitState;
    private Vector2 lastDir = Vector2.right;   // 기본값은 오른쪽

    private PhotonView photonView;

    private Vector3 networkedPosition;
    private Vector3 networkedVelocity;
    private float lerpSpeed = 10f;
    private bool networkedIsMoving;
    private bool networkedIsVomiting;
    private float networkedDirX;
    private float networkedDirY;

	[Header("뿌릴 Puddle Prefab")]
	[SerializeField] private GameObject puddlePrefab;
	[SerializeField] private float puddleDuration = 40f;
    private Vector2 pukeDir;

    [Header("토 후 부스트")]
    [SerializeField] private float boostMultiplier = 1.5f;
    [SerializeField] private float boostDuration = 3f;

    private float originalSpeed;
    private bool isBoosted = false;
    private float boostTimer = 0f;

    protected override void Awake()
    {
        base.Awake();
        originalSpeed = moveSpeed;

        photonView = GetComponent<PhotonView>(); 

        if (ghostStateMachine == null)
            ghostStateMachine = new GhostStateMachine();

        idleState = new PeanutIdle(this, ghostStateMachine, "Idle");
        moveState = new PeanutMove(this, ghostStateMachine, "Move");
        vomitState = new PukeGirlVomit(this, ghostStateMachine, "IsVomiting");

        ghostStateMachine.Initialize(idleState);
    }

    protected override void Start()
    {
        if (photonView.IsMine)
        {
            base.Start();
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
        if (photonView.IsMine)
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                photonView.RPC("RPC_StartVomit", RpcTarget.All);
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

        if (isBoosted)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
            {
                isBoosted = false;
                moveSpeed = originalSpeed;
            }
        }

    }

    [PunRPC]
    public void RPC_StartVomit()
    {
        pukeDir = lastDir;

        ghostStateMachine.ChangeState(vomitState);
		anim.SetBool("IsVomiting", true);
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

    public void OnVomitAnimEnd()
    {
        Debug.Log("토함 끝");

        Vector2 dir = pukeDir;

        float signX = Mathf.Abs(dir.x) > Mathf.Abs(dir.y) ? Mathf.Sign(dir.x) : 0f;

        float xOffset = 1.5f;
        float yOffset = -1.2f;

        Vector3 spawnPos = transform.position + new Vector3(signX * xOffset, yOffset, 0f);

        var puddle = Instantiate(puddlePrefab, spawnPos, Quaternion.identity);
        Destroy(puddle, puddleDuration);

        anim.SetBool("IsVomiting", false);

        isBoosted = true;
        boostTimer = boostDuration;
        moveSpeed = originalSpeed * boostMultiplier;

        Vector2 input = MoveInput;

        anim.SetBool("IsVomiting", false);
        ghostStateMachine.ChangeState(moveState);
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
            stream.SendNext(anim.GetBool("IsVomiting"));
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
            networkedIsVomiting = (bool)stream.ReceiveNext();
            anim.SetBool("IsVomiting", networkedIsVomiting);

            // 상태 변경 시 동기화
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
                    case GhostStateType.Vomit:
                        ghostStateMachine.ChangeState(vomitState);
                        break;
                }
            }
        }
    }
}
