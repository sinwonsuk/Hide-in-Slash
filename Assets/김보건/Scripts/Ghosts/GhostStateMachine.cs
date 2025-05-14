using UnityEngine;

public class GhostStateMachine
{
    public void Initialize(GhostState _startState)
    {
        currentState = _startState;
        currentState.Enter();
    }

    public void ChangeState(GhostState _newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = _newState;

        if (currentState != null)
            currentState.Enter();
    }

    public GhostState currentState { get; private set; }
    public Vector2 CurrentStateMoveInput => currentState.GetMoveInput();
    public GhostStateType CurrentStateType => currentState.StateType;

}
