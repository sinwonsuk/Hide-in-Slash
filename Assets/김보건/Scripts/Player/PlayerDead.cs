using Photon.Pun;
using System.Collections;
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

        //if (!player.photonView.IsMine) return;

        //// 연출 시작
        //if (DeadManager.Instance != null)
        //{
        //    DeadManager.Instance.CheckAllPlayerDead(); // AllDeath or PlayerDeath UI 보여줌
        //}

    }

    public override void Update()
    {
        base.Update();

        if (moveInput != Vector2.zero)
            stateMachine.ChangeState(player.moveState);
    }
    //public IEnumerator DeathWithDelay(float delay)
    //{
    //    yield return new WaitForSeconds(delay);

    //    PhotonNetwork.LoadLevel("RobbyScene");
    //}
}
