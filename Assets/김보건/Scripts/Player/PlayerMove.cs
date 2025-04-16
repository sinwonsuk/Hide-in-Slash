using UnityEngine;
using UnityEngine.Windows;

public class PlayerMove : PlayerState
{
    public PlayerMove(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("움직이기");
    }

    public override void Update()
    {
        base.Update();

        if (moveInput == Vector2.zero)
            stateMachine.ChangeState(player.idleState);
    }

    public override void FixedUpdate()
    {
        player.SetVelocity(moveInput * player.moveSpeed);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
