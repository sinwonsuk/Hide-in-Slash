using UnityEngine;

public class PeanutStunned : GhostState
{
    public PeanutStunned(Ghost _ghost, GhostStateMachine _stateMachine, string _animBoolName) : base(_ghost, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        ghost.SetZeroVelocity();
        Debug.Log("Ω∫≈œ");
    }

    public override void Update()
    {
        base.Update();
    }
}
