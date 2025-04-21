using UnityEngine;

public class ProteinIdle : GhostState
{
    public ProteinIdle(Ghost _ghost, GhostStateMachine _stateMachine, string _animBoolName) : base(_ghost, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        ghost.SetZeroVelocity();
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
