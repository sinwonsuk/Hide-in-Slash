using Photon.Pun;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDead : PlayerState
{
    public PlayerDead(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        stateType = PlayerStateType.Dead;
    }

    bool hasReportedDeath;

    public override void Enter()
    {
        base.Enter();
        player.SetZeroVelocity();
        player.BecomeGhost();

        player.isDead = true;

        //if (player.photonView.IsMine)
        //{
        //    ExitGames.Client.Photon.Hashtable props = new();
        //    props["IsDead"] = true;
        //    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        //}

        if (player.photonView.IsMine && !hasReportedDeath)          // 중복 호출 방지
        {
            hasReportedDeath = true;
            player.BroadcastStatus(RunnerStatus.Dead);
        }


    }

    public override void Update()
    {
        base.Update();

        if (moveInput != Vector2.zero)
            stateMachine.ChangeState(player.moveState);
    }
}
