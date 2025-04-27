using UnityEngine;

public class ProteinSkill_Move : GhostState
{
    public ProteinSkill_Move(Ghost _ghost, GhostStateMachine _stateMachine, string _animBoolName) : base(_ghost, _stateMachine, _animBoolName)
    {
        stateType = GhostStateType.Move;
    }

    public override void Update()
    {
        base.Update();
        Debug.Log($"[ProteinSkill_Move] moveInput: {moveInput}");
        ghost.UpdateAnimParam(moveInput);

        if (moveInput == Vector2.zero)
            stateMachine.ChangeState(ghost.idleState);
    }

    public override void FixedUpdate()
    {
        ghost.SetVelocity(moveInput * ghost.GetMoveSpeed());
    }
}
