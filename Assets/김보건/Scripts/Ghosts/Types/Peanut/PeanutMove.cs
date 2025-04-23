using UnityEngine;

public class PeanutMove : GhostState
{
    public PeanutMove(Ghost _ghost, GhostStateMachine _stateMachine, string _animBoolName) : base(_ghost, _stateMachine, _animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        ghost.UpdateAnimParam(moveInput);

        if (moveInput == Vector2.zero)
            stateMachine.ChangeState(ghost.idleState);
    }

    public override void FixedUpdate()
    {
        ghost.SetVelocity(moveInput * ghost.GetMoveSpeed());
    }
}
