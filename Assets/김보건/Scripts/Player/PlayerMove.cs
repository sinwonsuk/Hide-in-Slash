using UnityEngine;
using UnityEngine.Windows;

public class PlayerMove : PlayerState
{
	private float speedMultiplier = 1f;
	public PlayerMove(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        stateType = PlayerStateType.Move;
    }

    public override void Enter()
    {
        base.Enter();
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Walk, true);
    }

    public override void Update()
    {
        base.Update();

        player.UpdateAnimParam(moveInput);

        if (moveInput != Vector2.zero)
        {
            player.RotateLight(moveInput); // 라이트 회전
        }

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
        SoundManager.GetInstance().Sfx_Stop(SoundManager.sfx.Walk);
    }
}
