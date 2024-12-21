using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class AppStateController
{
    private readonly ReactiveProperty<AppState> _currentState = new ReactiveProperty<AppState>(AppState.None);

    // Expose CurrentState as an observable
    public IReadOnlyReactiveProperty<AppState> CurrentState => _currentState;

    public AppState GetCurrentState() => _currentState.Value;

    public void SetNewState(AppState newState)
    {
        _currentState.Value = newState;
        Debug.Log($"AppStateController: new state = {_currentState.Value}");
    }
}