using Photon.Pun;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Protein : Ghost, IPunObservable
{

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
            GameObject _skillImage = GameObject.Find("Ghost_SkillCoolTime_Sprite");
            skillImage = _skillImage.GetComponent<Image>();
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
		base.Update();

        if (photonView.IsMine)
		{
			if (Input.GetKeyDown(KeyCode.E) && !isProtein && !isCoolingDown)
			{
                SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Grow, false, 0.5f);
                photonView.RPC("DrinkProtein", RpcTarget.All);
                
            }

			diceTimer += Time.deltaTime;

			if (diceTimer >= diceCoolTime)
			{
				diceTimer = 0f;
                int roll = GetWeightedDiceRoll();
				Debug.Log("프로틴 주사위 굴리기: " + roll);
                SoundManager.GetInstance().SfxPlay(SoundManager.sfx.RollADice, false, 0.5f);
                photonView.RPC("RollDice", RpcTarget.All, roll);
			}

            UpdateSkillCooldown();
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

		if (isProteinCooldown)
		{
			proteinCooldownTimer -= Time.deltaTime;
			if (proteinCooldownTimer <= 0)
			{
				isProteinCooldown = false;
				proteinCooldownTimer = ProteincooldownDuration;
			}
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
            rb.linearVelocity = networkedVelocity;
        }
    }

    private int GetWeightedDiceRoll()
    {
        int[] weightedPool = new int[]
        {
            1,
            2, 2, 2,     
            3, 3, 3, 3, 3,
            4, 4, 4, 4, 4, 4, 4, 
            5, 5, 5,     
            6,      
        };

        int index = Random.Range(0, weightedPool.Length);
        return weightedPool[index];
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

    private void UseSkill()
    {
        isCoolingDown = true;
        cooldownTimer = cooldownTime;
        if (skillImage != null)
            skillImage.fillAmount = 1f;
    }

    [PunRPC]
    private void RollDice(int dice)
    {
        if(diceCoroutine != null)
            StopCoroutine(diceCoroutine);

        if (diceRollEffectPrefab != null && headPosition != null)
        {
            GameObject effect = Instantiate(diceRollEffectPrefab, headPosition.position, Quaternion.identity);
            effect.transform.SetParent(headPosition);
            effect.transform.localPosition = Vector3.zero; 
        }

        diceCoroutine = StartCoroutine(DiceCoroutine(dice));
    }

    private IEnumerator DiceCoroutine(int dice)
    {
        float duration = 10f; // 지속시간

        moveSpeed = originalSpeed;
        transform.localScale = originalScale;

        switch(dice)
        {
            case 1:
                moveSpeed = 4f;
                transform.localScale = originalScale * 2f;
                if (photonView.IsMine && impulseSource != null)
                    impulseSource.GenerateImpulse();
                break;
            case 2:
                moveSpeed = 4.4f;
                transform.localScale = originalScale * 1.6f;
                break;
            case 3:
                moveSpeed = 4.8f;
                transform.localScale = originalScale * 1.3f;
                break;
            case 4:
                moveSpeed = 5f;
                transform.localScale = originalScale * 1.1f;
                break;
            case 5:
                moveSpeed = 6f;
                transform.localScale = originalScale * 1.0f;
                break;
            case 6:
                moveSpeed = 6.5f;
                transform.localScale = originalScale * 0.9f;
                break;
            default:
                Debug.LogError("Invalid dice value: " + dice);
                break;
        }

        yield return new WaitForSeconds(duration);
        moveSpeed = originalSpeed;
        transform.localScale = originalScale;
    }

    [PunRPC]
    private void RotateSurvivorScreen(float angle, float duration)
    {
        if (!photonView.IsMine) return;

        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogWarning("Main Camera not found!");
            return;
        }

        StartCoroutine(RotateCameraCoroutine(mainCam.transform, angle, duration));
    }

    private IEnumerator RotateCameraCoroutine(Transform camTransform, float angle, float duration)
    {
        Quaternion originalRot = camTransform.rotation;
        camTransform.rotation = Quaternion.Euler(0, 0, angle);
        yield return new WaitForSeconds(duration);
        camTransform.rotation = originalRot;
    }

    [PunRPC]
    private void DrinkProtein()
    {
        if (!photonView.IsMine) return;

        UseSkill();

        float angle = Random.value > 0.5f ? 90f : 180f;
        float duration = 3f;

        Debug.Log($"[프로틴] 화면 회전 스킬 발동! {angle}도");

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView pv = obj.GetComponent<PhotonView>();
            if (pv != null)
            {
                pv.RPC("RotateSurvivorScreen", RpcTarget.All, angle, duration);
            }
        }

        ghostStateMachine.ChangeState(useSkillState);
    }

    [PunRPC]
    public void EndDrinkProtein()
    {
        isProtein = false;
        isProteinCooldown = true;
        proteinCooldownTimer = ProteincooldownDuration;

        moveSpeed = originalSpeed;
        transform.localScale = originalScale;
        Debug.Log("단백질 섭취 종료");

        anim.SetBool("IsProtein", false);
        anim.SetBool("IsOriginal", true);

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


        Vector2 input = MoveInput;


        if (input == Vector2.zero)
        {
            ghostStateMachine.ChangeState(normalIdleState);
        }
        else
        {
            ghostStateMachine.ChangeState(normalMoveState);
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
                    case GhostStateType.ProteinSkill:
                        ghostStateMachine.ChangeState(skillIdleState);
                        break;

                }
            }
        }
    }

    public override GhostState moveState { get; protected set; }
    private GhostState normalIdleState;
    private GhostState normalMoveState;
    private GhostState skillIdleState;
    private GhostState skillMoveState;



    [Header("단백질섭취(벌크업)")]
    [SerializeField] private float ProteinDuration = 10f; //벌크업 지속시간
    [SerializeField] private float ProteincooldownDuration = 5f;    //쿨타임
    [SerializeField] private float buffedSpeed = 2f;
    private bool isDashing = false; //대쉬 여부
    private bool isProtein = false; //단백질 섭취 여부
    private bool isProteinCooldown = false; //단백질 쿨타임 여부
    [SerializeField] private float proteinTimer; //지속시간 타이머
    [SerializeField] private float proteinCooldownTimer; //쿨타임 타이머

    [Header("스킬쿨타임")]
    [SerializeField] private float cooldownTime = 50f;
    private float cooldownTimer = 0f;
    private bool isCoolingDown = false;
    [SerializeField] private Image skillImage;

    [Header("주사위 패시브 구현")]
    [SerializeField] private float diceCoolTime = 10f;
    [SerializeField] private GameObject diceRollEffectPrefab;
    [SerializeField] private Transform headPosition; // 머리 위 기준 Transform
    private float diceTimer = 0f;
    private Coroutine diceCoroutine;

    [Header("주사위 1 디버프")]
    [SerializeField] private CinemachineImpulseSource impulseSource;

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
    bool isTeleporting = false;
}
