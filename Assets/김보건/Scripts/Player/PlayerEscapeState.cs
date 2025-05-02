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

        Debug.Log($"탈출 상태: {StateType} (Code: {(int)StateType}), 탈출 유형: {currentEscapeType} (Code: {(int)currentEscapeType})");

        switch (currentEscapeType)
        {
            case EscapeType.ExitDoor:

                player.StartCoroutine(EscapeWithDelay(2f));
                break;

            case EscapeType.Hatch:
                if (player.UseItemAndEscapeUI != null)
                {
                    player.UseItemAndEscapeUI.SetActive(true);

                    Transform black = player.UseItemAndEscapeUI.transform.Find("Black");
                    if (black != null)
                    {
                        playerDeath fadeEffect = black.GetComponent<playerDeath>();
                        if (fadeEffect != null)
                            fadeEffect.TriggerFade(); 
                    }
                }

                player.StartCoroutine(EscapeWithDelay(7f));
                break;

            default:
                break;
        }
    }

    private IEnumerator EscapeWithDelay(float delay)
    {

        yield return new WaitForSeconds(delay);   // UI 보여주는 시간

        //if (player.UseItemAndEscapeUI != null)
        //{
        //    player.UseItemAndEscapeUI.SetActive(false);
        //}

        //// 탈출 처리 (플레이어 오브젝트 비활성화)
        //player.gameObject.SetActive(false);
    }
}
