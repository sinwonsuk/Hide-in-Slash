using UnityEngine;

public class PeanutStunned : GhostState
{
    public PeanutStunned(Ghost _ghost, GhostStateMachine _stateMachine, string _animBoolName) : base(_ghost, _stateMachine, _animBoolName)
    {
        stateType = GhostStateType.Stunned;
    }

    public override void Enter()
    {
        base.Enter();
        ghost.SetZeroVelocity();

        ghost.anim.SetBool("IsMoving", false);
        Debug.Log("����");
    }

    public override void Update()
    {
        base.Update();
    }
}
