using UnityEngine;

public class Ghost : MonoBehaviour
{

    public Animator anim { get; protected set; }
    public Rigidbody2D rb { get; protected set; }

    public int facingDir { get; protected set; } = 1;
    protected bool facingRight = true;

    [Header("이동 속도")]
    [SerializeField] protected float moveSpeed = 5f;

    public GhostStateMachine ghostStateMachine { get; protected set; }
    public GhostIdle idleState { get; protected set; }
    public virtual GhostState moveState { get; protected set; }

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        ghostStateMachine = new GhostStateMachine();
        idleState = new GhostIdle(this, ghostStateMachine, "Idle");
    }

    protected virtual void Start()
    {
        ghostStateMachine.Initialize(idleState);
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
        transform.position = new Vector3(pos.x, pos.y, pos.y);
    }



    public virtual void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
        FlipController(velocity.x);
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

    public void FlipController(float x)
    {
        if (x > 0 && !facingRight) Flip();
        else if (x < 0 && facingRight) Flip();
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }
}
