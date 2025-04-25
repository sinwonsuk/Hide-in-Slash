using UnityEngine;

public class PukeGirl : Ghost
{
    public override GhostState moveState { get; protected set; }
    private GhostState vomitState;
    private Vector2 lastDir = Vector2.right;   // 기본값은 오른쪽
    protected override void Awake()
    {

        base.Awake();
        idleState = new PeanutIdle(this, ghostStateMachine, "Idle");
        moveState = new PeanutMove(this, ghostStateMachine, "Move");
        vomitState = new PukeGirlVomit(this, ghostStateMachine, "Puking");

        ghostStateMachine.Initialize(idleState);
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        //토관련
        if (Input.GetKeyDown(KeyCode.Space))
        {
           
            ghostStateMachine.ChangeState(vomitState);
        }

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
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
        anim.SetBool("IsVomiting", false);

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
}
