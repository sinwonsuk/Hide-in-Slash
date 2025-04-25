using Photon.Pun;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Protein : Ghost, IPunObservable
{
    public override GhostState moveState { get; protected set; }

    [Header("�ܹ�������(��ũ��)")]
    [SerializeField] private float ProteinDuration = 10f; //��ũ�� ���ӽð�
    [SerializeField] private float ProteincooldownDuration = 5f;    //��Ÿ��
    [SerializeField] private float buffedSpeed = 2f;
    private bool isProtein = false; //�ܹ��� ���� ����
    private bool isProteinCooldown = false; //�ܹ��� ��Ÿ�� ����
    [SerializeField] private float proteinTimer; //���ӽð� Ÿ�̸�
    [SerializeField] private float proteinCooldownTimer; //��Ÿ�� Ÿ�̸�

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

        idleState = new PeanutIdle(this, ghostStateMachine, "Idle");
        moveState = new PeanutMove(this, ghostStateMachine, "Move");

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
                photonView.RPC("ActivateProtein", RpcTarget.All);
            }
        }
        else
        {
            //�̵�����
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
    private void ActivateProtein()
    {
        isProtein = true;
        proteinTimer = ProteinDuration;
        moveSpeed = buffedSpeed;
        transform.localScale = originalScale * 1.5f;

        Debug.Log("�ܹ��� ����");
    }

    [PunRPC]
    private void EndProtein()
    {
        isProtein = false;
        isProteinCooldown = true;
        proteinCooldownTimer = ProteincooldownDuration;

        moveSpeed = originalSpeed;
        transform.localScale = originalScale;
        Debug.Log("�ܹ��� ���� ����");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // ���� ����
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.localScale);
            stream.SendNext(rb.linearVelocity);
            stream.SendNext((int)ghostStateMachine.CurrentStateType);
            stream.SendNext(facingDir);
            stream.SendNext(facingUpDir);
            //stream.SendNext(anim.GetBool("IsMoving"));
            //stream.SendNext(anim.GetFloat("DirX"));
            //stream.SendNext(anim.GetFloat("DirY"));
        }
        else // ������ ���� �� ����
        {
            networkedPosition = (Vector3)stream.ReceiveNext();
            transform.localScale = (Vector3)stream.ReceiveNext();
            networkedVelocity = (Vector2)stream.ReceiveNext();
            GhostStateType receivedState = (GhostStateType)stream.ReceiveNext();
            int receivedFacingDir = (int)stream.ReceiveNext();
            int receivedFacingUpDir = (int)stream.ReceiveNext();
            SetFacingDirection(receivedFacingDir, receivedFacingUpDir);
            //networkedIsMoving = (bool)stream.ReceiveNext();
            //networkedDirX = (float)stream.ReceiveNext();
            //networkedDirY = (float)stream.ReceiveNext();

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
