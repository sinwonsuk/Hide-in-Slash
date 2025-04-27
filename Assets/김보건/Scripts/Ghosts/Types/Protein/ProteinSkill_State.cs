using UnityEngine;

public class ProteinSkill_State : GhostState
{
    public ProteinSkill_State(Ghost _ghost, GhostStateMachine _stateMachine, string _animBoolName) : base(_ghost, _stateMachine, _animBoolName)
    {
        stateType = GhostStateType.ProteinSkill;
    }
    public override void Enter()
    {
        base.Enter();
        ghost.SetZeroVelocity();
        ghost.anim.SetBool("IsProtein", true);
        ghost.anim.SetBool("IsMoving", false);
        Debug.Log("단백질 스킬");
    }
    public override void Update()
    {
        base.Update();

        ghost.UpdateAnimParam(moveInput);

        if (moveInput != Vector2.zero)
        {
            ghost.anim.SetBool("IsMoving", true);
        }
        else
        {
            ghost.anim.SetBool("IsMoving", false);
        }
    }
}

