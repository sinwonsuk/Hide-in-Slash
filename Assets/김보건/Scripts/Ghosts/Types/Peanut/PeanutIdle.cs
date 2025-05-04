using UnityEngine;

public class PeanutIdle : GhostState
{
    public PeanutIdle(Ghost _ghost, GhostStateMachine _stateMachine, string _animBoolName) : base(_ghost, _stateMachine, _animBoolName)
    {
        stateType = GhostStateType.Idle;
    }

    public override void Enter()
    {
        base.Enter();
        ghost.SetZeroVelocity();
        ghost.rb.constraints |= RigidbodyConstraints2D.FreezePosition;
        ghost.UpdateAnimParam(Vector2.zero);
    }
    public override void Update()
    {
        base.Update();
        if (moveInput != Vector2.zero)
        {
            stateMachine.ChangeState(ghost.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        ghost.rb.constraints &= ~RigidbodyConstraints2D.FreezePosition;
    }

}
