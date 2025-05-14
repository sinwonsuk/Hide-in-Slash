using Photon.Pun;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class PlayerEscapeState : PlayerState
{
    private EscapeType currentEscapeType;
    public PlayerEscapeState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
        stateType = PlayerStateType.Escape;
    }

    public void SetEscapeType(EscapeType type)
    {
        currentEscapeType = type;
        stateType = PlayerStateType.Escape;
    }

    public override void Enter()
    {
        base.Enter();
        player.SetZeroVelocity();
        player.SetEscapeType(currentEscapeType);

        if (!player.photonView.IsMine)
            return;
        PhotonNetwork.AutomaticallySyncScene = false;
        Debug.Log($"탈출 상태: {StateType} (Code: {(int)StateType}), 탈출 유형: {currentEscapeType} (Code: {(int)currentEscapeType})");
        player.profileSlotManager.photonView.RPC("SyncProfileState", RpcTarget.All, PhotonNetwork.LocalPlayer, ProfileState.Escape);

        if (player.photonView.IsMine)      
        {
            player.BroadcastStatus(RunnerStatus.Escaped);
        }

        switch (currentEscapeType)
        {
            case EscapeType.ExitDoor:

                player.photonView.RPC("EscapePlayerObject", RpcTarget.Others);
                break;

            case EscapeType.Hatch:
                if (player.UseItemAndEscapeUI != null)
                {
                    player.UseItemAndEscapeUI.transform.SetParent(null);
                    Object.DontDestroyOnLoad(player.UseItemAndEscapeUI);
                    player.UseItemAndEscapeUI.SetActive(true);

                    Transform black = player.UseItemAndEscapeUI.transform.Find("Black");
                    if (black != null)
                    {
                        playerDeath fadeEffect = black.GetComponent<playerDeath>();
                        if (fadeEffect != null)
                            fadeEffect.TriggerFade();
                    }
                }

                player.photonView.RPC("EscapePlayerObject", RpcTarget.Others);

                player.StartCoroutine(EscapeWithDelay(5f));

                break;

            default:
                break;
        }
    }

    private IEnumerator EscapeWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (player.UseItemAndEscapeUI != null)
        {
            Object.Destroy(player.UseItemAndEscapeUI);
        }
        if(player.ExitDoorEscapeUI != null)
        {
            Object.Destroy(player.ExitDoorEscapeUI);
        }

        Player.runnerStatuses.Clear();

        if (player.photonView.IsMine)
        {
            PhotonNetwork.Destroy(player.gameObject);

            // 방을 먼저 나감
            if (PhotonNetwork.InRoom)
                PhotonNetwork.LeaveRoom();

            yield return new WaitForSeconds(0.1f);
            if (!PhotonNetwork.InLobby)
                PhotonNetwork.JoinLobby();
        }
    }

}
