using UnityEngine;
using UnityEngine.Windows;

public class PlayerIdle : PlayerState
{
    public PlayerIdle(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        stateType = PlayerStateType.Idle;
    }

    public override void Enter()
    {
        base.Enter();

        player.SetZeroVelocity();

        // 마지막 바라보던 방향 그대로 유지
        player.UpdateAnimParam(Vector2.zero);
    }
    public override void Update()
    {
        base.Update();

        if (moveInput != Vector2.zero)
            stateMachine.ChangeState(player.moveState);

    }
    public override void Exit()
    {
        base.Exit();
    }

}
