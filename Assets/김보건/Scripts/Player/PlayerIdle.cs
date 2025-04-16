using UnityEngine;
using UnityEngine.Windows;

public class PlayerIdle : PlayerState
{
    public PlayerIdle(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetZeroVelocity();

        Debug.Log("°¡¸¸È÷");
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
