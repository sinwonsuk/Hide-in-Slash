using UnityEngine;

public class PlayerDead : PlayerState
{
    public PlayerDead(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        stateType = PlayerStateType.Dead;
    }

    public override void Enter()
    {
        base.Enter();
        player.SetZeroVelocity();
        player.BecomeGhost();
    }

    public override void Update()
    {
        base.Update();

        if (moveInput != Vector2.zero)
            stateMachine.ChangeState(player.moveState);
    }
}
