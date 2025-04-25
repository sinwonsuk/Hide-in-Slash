using UnityEngine;

public class PukeGirlMove : GhostState
{
    public PukeGirlMove(Ghost _ghost, GhostStateMachine _stateMachine, string _animBoolName) : base(_ghost, _stateMachine, _animBoolName)
    {
        stateType = GhostStateType.Move;
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
