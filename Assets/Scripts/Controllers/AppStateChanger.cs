using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class AppStateChanger : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private AppState _appState;

    [Inject] AppStateController _appStateController;

    public void OnPointerClick(PointerEventData eventData)
    {
        _appStateController.SetNewState(_appState);
    }
}