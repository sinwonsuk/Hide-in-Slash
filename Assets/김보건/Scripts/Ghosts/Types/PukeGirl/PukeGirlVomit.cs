using UnityEngine;

public class PukeGirlVomit : GhostState
{
    public PukeGirlVomit(Ghost _ghost, GhostStateMachine _stateMachine, string _animBoolName) : base(_ghost, _stateMachine, _animBoolName)
    {
        stateType = GhostStateType.Vomit;
    }
    public override void Enter()
    {
        base.Enter();
        ghost.SetZeroVelocity();

        ghost.anim.SetBool("IsVomiting", true);
        ghost.anim.SetBool("IsMoving", false);
        Debug.Log("ลไวิ");
    }

    public override void Update()
    {
        base.Update();
    }

}
