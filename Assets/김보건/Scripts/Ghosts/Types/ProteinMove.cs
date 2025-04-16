using UnityEngine;

public class ProteinMove : GhostState
{
    public ProteinMove(Ghost _ghost, GhostStateMachine _stateMachine, string _animBoolName) : base(_ghost, _stateMachine, _animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(x, y).normalized;

        if (moveInput == Vector2.zero)
            stateMachine.ChangeState(ghost.idleState);
    }

    public override void FixedUpdate()
    {
        ghost.SetVelocity(moveInput * ghost.GetMoveSpeed());
    }
}
