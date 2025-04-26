using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public enum PlayerStateType
{
    Idle = 0,
    Move = 1,
    Dead = 2,
    Escape = 3
}

public class PlayerState
{

    protected PlayerStateMachine stateMachine;
    protected Player player;

    protected Rigidbody2D rb;

    protected Vector2 moveInput;
    private string animBoolName;

    protected float stateTimer;
    protected bool triggerCalled;

    protected PlayerStateType stateType;
    public PlayerStateType StateType => stateType;


    public PlayerState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        //if (player.anim != null)
        //    player.anim.SetBool(animBoolName, true);
        rb = player.rb;
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
        //if (player.anim != null)
        //    player.anim.SetBool(animBoolName, false);
    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }

    protected void HandleFlipByInput()
    {
        player.FlipController(moveInput.x, moveInput.y);
    }

}
