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

        Debug.Log($"≈ª√‚ ªÛ≈¬: {StateType} (Code: {(int)StateType}), ≈ª√‚ ¿Ø«¸: {currentEscapeType} (Code: {(int)currentEscapeType})");

        switch (currentEscapeType)
        {
            case EscapeType.ExitDoor:
                player.StartCoroutine(EscapeWithDelay(2f));
                break;

            case EscapeType.Hatch:
                player.StartCoroutine(EscapeWithDelay(0.1f));
                break;

            default:
                break;
        }
    }

    private IEnumerator EscapeWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log($"{player.name} ≈ª√‚ øœ∑·, ≈ª√‚ πÊπ˝: {player.escapeType}");
        //≈ª√‚√≥∏Æ
        player.gameObject.SetActive(false); 
    }
}
