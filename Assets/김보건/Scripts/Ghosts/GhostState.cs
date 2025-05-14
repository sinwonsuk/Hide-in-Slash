using UnityEngine;

public enum GhostStateType
{
    Idle,
    Move,
    Stunned,
    Vomit,
    ProteinSkill
}
public class GhostState
{
    public GhostState(Ghost _ghost, GhostStateMachine _stateMachine, string _animBoolName)
    {
        this.ghost = _ghost;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        //if (ghost.anim != null)
        //    ghost.anim.SetBool(animBoolName, true);
        rb = ghost.rb;
        triggerCalled = false;
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;

        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }

    public virtual void FixedUpdate() { }

    public virtual void Exit()
    {
        //if (ghost.anim != null)
        //    ghost.anim.SetBool(animBoolName, false);
    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }

    protected void HandleFlipByInput()
    {
        ghost.FlipController(moveInput.x, moveInput.y);
    }

    public virtual Vector2 GetMoveInput()
    {
        return moveInput;
    }


    protected GhostStateMachine stateMachine;
    protected Ghost ghost;

    protected Rigidbody2D rb;

    protected Vector2 moveInput;
    private string animBoolName;

    protected GhostStateType stateType;
    public GhostStateType StateType => stateType;

    protected float stateTimer;
    protected bool triggerCalled;

}
