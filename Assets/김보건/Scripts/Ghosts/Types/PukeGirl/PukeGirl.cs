using Photon.Pun;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PukeGirl : Ghost, IPunObservable
{
    bool isTeleporting = false;
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
    private Vector2 pukeDir;

    [Header("스킬쿨타임")]
    [SerializeField] private float cooldownTime = 5f;
    private float cooldownTimer = 0f;
    private bool isCoolingDown = false;
    [SerializeField] private Image skillImage;

    private bool wasMoving = false;

    protected override void Awake()
    {
        base.Awake();

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
            GameObject _skillImage = GameObject.Find("Ghost_SkillCoolTime_Sprite");
            skillImage = _skillImage.GetComponent<Image>();
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
            if (Input.GetKeyDown(KeyCode.E) && !isCoolingDown)
            {
                SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Vomit, false, 0.5f);
                photonView.RPC("RPC_StartVomit", RpcTarget.All);
            }
            UpdateSkillCooldown();
        }
        else
        {
            //이동보간
         

            anim.SetBool("IsMoving", networkedIsMoving);
            anim.SetFloat("DirX", networkedDirX);
            anim.SetFloat("DirY", networkedDirY);

            if (Mathf.Abs(networkedDirX) >= Mathf.Abs(networkedDirY))
            {
                sr.flipX = networkedDirX < 0;
            }
        }

    }
    private void UseSkill()
    {
        isCoolingDown = true;
        cooldownTimer = cooldownTime;
        if (skillImage != null)
            skillImage.fillAmount = 1f;
    }

    [PunRPC]
    void TeleportPlayer(int viewID, Vector3 pos)
    {
        PhotonView view = PhotonView.Find(viewID);
        if (view != null)
        {
            view.transform.position = pos;



            // 보간용 위치도 순간이동 위치로 맞춰줌
            networkedPosition = pos;
            rb.linearVelocity = Vector2.zero;
            isTeleporting = true;
            StartCoroutine(ResetTeleportFlag());

        }
    }
    public IEnumerator ResetTeleportFlag()
    {
        yield return new WaitForSeconds(0.2f);
        isTeleporting = false;
    }
    private void UpdateSkillCooldown()
    {
        if (!isCoolingDown) return;
        cooldownTimer -= Time.deltaTime;
        if (skillImage != null)
            skillImage.fillAmount = cooldownTimer / cooldownTime;
        if (cooldownTimer <= 0f)
        {
            isCoolingDown = false;
            if (skillImage != null)
                skillImage.fillAmount = 0f;
        }
    }

    [PunRPC]
    public void RPC_StartVomit()
    {
        pukeDir = lastDir;
        ghostStateMachine.ChangeState(vomitState);
        anim.SetBool("IsVomiting", true);

        if (photonView.IsMine)
        {
            UseSkill();

            photonView.RPC("RPC_TriggerFogIncrease", RpcTarget.Others);
        }
    }

    [PunRPC]
    public void RPC_TriggerFogIncrease()
    {
        var fog = Camera.main?.GetComponentInChildren<FogController>();
        if (fog != null)
        {
            fog.IncreaseFog();
            Debug.Log("생존자 화면에서 안개 진해짐!");
        }
    }

    [PunRPC]
    public void SetStartPosition(Vector3 pos)
    {
        transform.position = pos;
    }


    protected override void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            base.FixedUpdate();
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, networkedPosition, Time.fixedDeltaTime * lerpSpeed);
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

        if (photonView.IsMine)
        {
            if (isMoving && !wasMoving)
            {
                SoundManager.GetInstance().SfxPlay(SoundManager.sfx.PukeWalking, true, 0.5f);
            }
            else if (!isMoving && wasMoving)
            {
                SoundManager.GetInstance().Sfx_Stop(SoundManager.sfx.PukeWalking);
            }
            wasMoving = isMoving;
        }

    }

    public void OnVomitAnimEnd()
    {
        Debug.Log("토함 끝");

        Vector2 dir = pukeDir;

        float signX = Mathf.Abs(dir.x) > Mathf.Abs(dir.y) ? Mathf.Sign(dir.x) : 0f;

        float xOffset = 1.5f;
        float yOffset = 0f;

        Vector3 spawnPos = transform.position + new Vector3(signX * xOffset, yOffset, 0f);

        if (photonView.IsMine)
        {
            PhotonNetwork.Instantiate("PuddlePrefab", spawnPos, Quaternion.identity);
        }

        anim.SetBool("IsVomiting", false);

        Vector2 input = MoveInput;
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
            if (!isTeleporting)
            {
                networkedPosition = (Vector3)stream.ReceiveNext();
            }
            else
            {
                stream.ReceiveNext(); // 데이터는 소모해야 순서 안 꼬임
            }

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
