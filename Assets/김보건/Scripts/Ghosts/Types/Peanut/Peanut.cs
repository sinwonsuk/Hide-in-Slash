using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Peanut : Ghost, IPunObservable
{
    public override GhostState moveState { get; protected set; }
    private GhostState stunnedState;
    [SerializeField] private bool isStunned = false;
    private bool canBeStunned = true;

    private Vector2 lastDir = Vector2.right;   // 기본값은 오른쪽

    private Vector3 networkedPosition;
    private Vector3 networkedVelocity;
    private float lerpSpeed = 10f;
    private bool networkedIsMoving;
    private float networkedDirX;
    private float networkedDirY;
    bool isTeleporting = false;

    [Header("머리 위에 띄울 ! 아이콘 Prefab")]
    [SerializeField] private Transform headPosition;
    public GameObject exclamationPrefab;

	private GameObject exclamationInstance;
	private RegionTrigger currentRegion;

    [Header("스킬 쿨타임")]
    [SerializeField] private float cooldownTime = 20f;
    private float cooldownTimer = 0f;
    private bool isCoolingDown = false;
    [SerializeField] private Image skillImage;

    [Header("블랙아웃 지속시간")]
    [SerializeField] private float blackoutDuration = 5f;
    public bool isBlackOut = false;

    private bool wasMoving = false;

    protected override void Awake()
    {

        base.Awake();

        if (ghostStateMachine == null)
            ghostStateMachine = new GhostStateMachine();

        idleState = new PeanutIdle(this, ghostStateMachine, "Idle");
        moveState = new PeanutMove(this, ghostStateMachine, "Move");
        stunnedState = new PeanutStunned(this, ghostStateMachine, "sturnned");

        ghostStateMachine.Initialize(idleState);

		if (exclamationPrefab != null)
		{
            exclamationInstance = Instantiate(exclamationPrefab, headPosition.position, Quaternion.identity);
            exclamationInstance.transform.SetParent(headPosition);
            exclamationInstance.transform.localPosition = Vector3.zero;
            exclamationInstance.SetActive(false);
		}
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


    protected override void Update()
    {
        if (photonView.IsMine)
        {
            base.Update();
            CooldownSkill();

            if (Input.GetKeyDown(KeyCode.E) && !isCoolingDown)
            {
                UseSkill();
            }

        }
        else
        {
            //이동보간
            //transform.position = Vector3.Lerp(transform.position, networkedPosition, Time.deltaTime * lerpSpeed);
            
            anim.SetBool("IsMoving", networkedIsMoving);
            anim.SetFloat("DirX", networkedDirX);
            anim.SetFloat("DirY", networkedDirY);

            if (Mathf.Abs(networkedDirX) >= Mathf.Abs(networkedDirY))
            {
                sr.flipX = networkedDirX < 0;
            }
        }

		if (currentRegion != null && currentRegion.HasAnySurvivor)
			exclamationInstance?.SetActive(true);
		else
			exclamationInstance?.SetActive(false);
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
    public void CooldownSkill()
    {
        if (isCoolingDown)
        {
            cooldownTimer += Time.deltaTime;
            skillImage.fillAmount = 1f - (cooldownTimer / cooldownTime);

            if (cooldownTimer >= cooldownTime)
            {
                isCoolingDown = false;
                skillImage.fillAmount = 0f;
            }
        }
    }

    // 스킬 사용
    private void UseSkill()
    {
        isCoolingDown = true;
        cooldownTimer = 0f;
        if (photonView.IsMine && skillImage != null)
        {
            skillImage.fillAmount = 1f;
        }

        photonView.RPC("Blackout", RpcTarget.AllBuffered);

    }

    [PunRPC]
    public void Blackout()
    {
        StartCoroutine(enumerator());
    }

    IEnumerator enumerator()
    {
        EventManager.TriggerEvent(EventType.IsBlackout, true);
        EventManager.TriggerEvent(EventType.EntireLightOff);
        yield return new WaitForSeconds(blackoutDuration);
        EventManager.TriggerEvent(EventType.IsBlackout, false);
        EventManager.TriggerEvent(EventType.EntireLightOn);
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

        if (isStunned)
        {
            if (wasMoving)
            {
                SoundManager.GetInstance().Sfx_Stop(SoundManager.sfx.PeanutWalking);
                wasMoving = false;
            }
            return;
        }

        if (photonView.IsMine)
        {
            if(isMoving && !wasMoving)
            {
                SoundManager.GetInstance().SfxPlay(SoundManager.sfx.PeanutWalking, true, 0.5f);
            }
            else if(!isMoving && wasMoving)
            {
                SoundManager.GetInstance().Sfx_Stop(SoundManager.sfx.PeanutWalking);
            }
            wasMoving = isMoving;
        }

    }
    [PunRPC]
    public void SetStartPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerSight") && !isStunned && canBeStunned)
        {
            canBeStunned = false;
            StartCoroutine(StunnedTime(2f));
        }

		var rt = collision.GetComponent<RegionTrigger>();
		if (rt != null)
		{
			currentRegion = rt;
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		var rt = other.GetComponent<RegionTrigger>();
		if (rt == currentRegion)
		{
			currentRegion = null;
			exclamationInstance?.SetActive(false);
		}
	}
	private IEnumerator StunnedTime(float time)
    {
        isStunned = true;
        SoundManager.GetInstance().Sfx_Stop(SoundManager.sfx.PeanutWalking);
        wasMoving = false;

        ghostStateMachine.ChangeState(stunnedState);
        yield return new WaitForSeconds(time);

        isStunned = false;
        ghostStateMachine.ChangeState(idleState);

        yield return new WaitForSeconds(3f);
        canBeStunned = true;
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
                    case GhostStateType.Stunned:
                        ghostStateMachine.ChangeState(stunnedState);
                        break;
                }
            }
        }
    }
}

