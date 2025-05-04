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


        switch (currentEscapeType)
        {
            case EscapeType.ExitDoor:

                player.StartCoroutine(EscapeWithDelay(2f));
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
        if (player.photonView.IsMine)
        {
            PhotonNetwork.Destroy(player.gameObject);    
            PhotonNetwork.LoadLevel("RobbyScene");
        }
    }

}
