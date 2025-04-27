using UnityEngine;

public class Ghost : MonoBehaviour
{
    public Vector2 MoveInput => ghostStateMachine.CurrentStateMoveInput;
    public Animator anim { get; protected set; }
    public Rigidbody2D rb { get; protected set; }

    public SpriteRenderer sr { get; protected set; }

    public int facingDir { get; private set; } = 1;
    public int facingUpDir { get; private set; } = 1;   // 1 = 위, -1 = 아래
    protected bool facingRight = true;
    protected bool facingUp = true;

    [Header("이동 속도")]
    [SerializeField] protected float moveSpeed = 5f;

    public GhostStateMachine ghostStateMachine { get; protected set; }
    public virtual GhostState idleState { get; protected set; }
    public virtual GhostState moveState { get; protected set; }

    public virtual GhostState useSkillState { get; protected set; }
    public virtual GhostState idleSkillState { get; protected set; }

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();

        ghostStateMachine = new GhostStateMachine();
        //idleState = new GhostIdle(this, ghostStateMachine, "Idle");
    }

    protected virtual void Start()
    {
        //ghostStateMachine.Initialize(idleState);
    }

    protected virtual void Update()
    {
        ghostStateMachine.currentState.Update();
    }

    private void LateUpdate()
    {
        
    }

    protected virtual void FixedUpdate()
    {
        ghostStateMachine.currentState.FixedUpdate();
        Vector3 pos = transform.position;
        //transform.position = new Vector3(pos.x, pos.y, pos.y);

    }

    public void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
    }

    public void SetZeroVelocity()
    {
        rb.linearVelocity = Vector2.zero;
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

    public void SetFacingDirection(int dir, int upDir)
    {
        this.facingDir = dir;
        this.facingUpDir = upDir;
    }


    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public virtual void UpdateAnimParam(Vector2 input) { }
}
