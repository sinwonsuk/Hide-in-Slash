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
        ghost.UpdateAnimParam(Vector2.zero);
        Debug.Log("°¡¸¸È÷");
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
    }

}
