using Photon.Pun;
using System.Collections;
using Unity.Cinemachine;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Peanut : Ghost, IPunObservable
{

    bool isInitialized =true;
    public override GhostState moveState { get; protected set; }
    private GhostState stunnedState;
    [SerializeField] private bool isStunned = false;

    private Vector2 lastDir = Vector2.right;   // 기본값은 오른쪽

    private PhotonView photonView;

    private Vector3 networkedPosition;
    private Vector2 networkedVelocity;
    private float lerpSpeed = 10f;
    private bool networkedIsMoving;
    private float networkedDirX;
    private float networkedDirY;

	[Tooltip("머리 위에 띄울 ! 아이콘 Prefab")]
	public GameObject exclamationPrefab;

	private GameObject exclamationInstance;
	private RegionTrigger currentRegion;

	protected override void Awake()
    {

        base.Awake();
        photonView = GetComponent<PhotonView>();

        if (ghostStateMachine == null)
            ghostStateMachine = new GhostStateMachine();

        idleState = new PeanutIdle(this, ghostStateMachine, "Idle");
        moveState = new PeanutMove(this, ghostStateMachine, "Move");
        stunnedState = new PeanutStunned(this, ghostStateMachine, "sturnned");

        ghostStateMachine.Initialize(idleState);

		if (exclamationPrefab != null)
		{
			exclamationInstance = Instantiate(exclamationPrefab, transform);
			exclamationInstance.transform.localPosition = Vector3.up * 2f;
			exclamationInstance.SetActive(false);
		}
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

        isInitialized = false;
    }

    protected override void Update()
    {
        if (photonView.IsMine)
        {
            base.Update();
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerSight") && !isStunned)
        {
            StartCoroutine(StunnedTime(5f));
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
        ghostStateMachine.ChangeState(stunnedState);
        yield return new WaitForSeconds(time);
        isStunned = false;
        ghostStateMachine.ChangeState(idleState);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!isInitialized)
        {
            return;
        }

        if (stream.IsWriting) // 내가 보내는 쪽
        {
            stream.SendNext((int)ghostStateMachine.CurrentStateType);
            stream.SendNext(facingDir);
            stream.SendNext(facingUpDir);
            stream.SendNext(anim.GetBool("IsMoving"));
            stream.SendNext(anim.GetFloat("DirX"));
            stream.SendNext(anim.GetFloat("DirY"));
        }
        else // 받는 쪽
        {


   
            object receivedState = stream.ReceiveNext();
            GhostStateType ghostState = GhostStateType.Idle;
            if (receivedState is int stateInt)
            {
                ghostState = (GhostStateType)stateInt;
            }
            else
            {
                Debug.LogError($"Expected int for ghost state but got {receivedState?.GetType()}");
            }

            object receivedFacingDir = stream.ReceiveNext();
            object receivedFacingUpDir = stream.ReceiveNext();
            if (receivedFacingDir is int dir && receivedFacingUpDir is int upDir)
            {
                SetFacingDirection(dir, upDir);
            }
            else
            {
                Debug.LogError("FacingDir or FacingUpDir receive error.");
            }

            object receivedIsMoving = stream.ReceiveNext();
            if (receivedIsMoving is bool isMoving)
            {
                networkedIsMoving = isMoving;
            }
            else
            {
                Debug.LogError("Expected bool for IsMoving.");
            }

            object receivedDirX = stream.ReceiveNext();
            if (receivedDirX is float dirX)
            {
                networkedDirX = dirX;
            }
            else
            {
                Debug.LogError("Expected float for DirX.");
            }

            object receivedDirY = stream.ReceiveNext();
            if (receivedDirY is float dirY)
            {
                networkedDirY = dirY;
            }
            else
            {
                Debug.LogError("Expected float for DirY.");
            }

            // 상태 변경
            if (ghostStateMachine != null && ghostStateMachine.CurrentStateType != ghostState)
            {
                switch (ghostState)
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
                    default:
                        Debug.LogWarning($"Unhandled ghost state: {ghostState}");
                        break;
                }
            }
        }
    }
}

