using UnityEngine;

public class ProteinSkill_Idle : GhostState
{
    public ProteinSkill_Idle(Ghost _ghost, GhostStateMachine _stateMachine, string _animBoolName) : base(_ghost, _stateMachine, _animBoolName)
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
        Debug.Log($"[ProteinSkill_Idle] moveInput: {moveInput}");
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
