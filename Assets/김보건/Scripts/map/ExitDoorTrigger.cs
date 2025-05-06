using Photon.Pun;
using UnityEngine;

public class ExitDoorTrigger : MonoBehaviourPun
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        var player = col.GetComponent<Player>();
        if (player && player.photonView.IsMine)
        {
            // 탈출 상태 전환 (Exit Door)
            player.escapeState.SetEscapeType(EscapeType.ExitDoor);
            player.PlayerStateMachine.ChangeState(player.escapeState);

            // ExitManager에 보고
            ExitManager.Instance.photonView.RPC("RPC_PlayerEnteredExit", RpcTarget.MasterClient);
        }
    }
}