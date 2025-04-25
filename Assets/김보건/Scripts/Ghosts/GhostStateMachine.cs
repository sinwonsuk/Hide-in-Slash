using UnityEngine;

public class GhostStateMachine
{
    public GhostState currentState { get; private set; }
    public Vector2 CurrentStateMoveInput => currentState.GetMoveInput();
    public GhostStateType CurrentStateTyper => currentState.StateType;

    public void Initialize(GhostState _startState)
    {
        currentState = _startState;
        currentState.Enter();
    }

    public void ChangeState(GhostState _newState)
    {
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }
}
