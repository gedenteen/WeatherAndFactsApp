using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;

public class AppStateController
{
    private readonly ReactiveProperty<AppState> _currentState;

    // Expose CurrentState as an observable
    public IReadOnlyReactiveProperty<AppState> CurrentState => _currentState;

    public AppStateController(AppState initialState)
    {
        _currentState = new ReactiveProperty<AppState>(initialState);
        Debug.Log($"AppStateController initialized with state: {initialState}");
    }

    public AppState GetCurrentState() => _currentState.Value;

    public void SetNewState(AppState newState)
    {
        _currentState.Value = newState;
        Debug.Log($"AppStateController: new state = {_currentState.Value}");
    }
}
