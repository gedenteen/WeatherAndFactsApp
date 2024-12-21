using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.Burst.Intrinsics;
using UnityEngine;
using Zenject;

public class DogsInfoUiController : MonoBehaviour
{
    [Header("References to scene objects")]
    [SerializeField] private Transform _holderForWeatherPeriodViews;

    [Header("References to assets")]
    [SerializeField] private DogBreedView _prefabDogBreedView;

    [Inject] private Timings _timings;
    [Inject] private AppStateController _appStateController;
    [Inject] private DogFactsService _dogFactsService;
    
    private CompositeDisposable _disposables = new CompositeDisposable();

    [Inject]
    private void Construct()
    {
        _appStateController.CurrentState.Subscribe(HandleAppStateChange).AddTo(_disposables);
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }

    private void HandleAppStateChange(AppState newState)
    {
        switch (newState)
        {
            case AppState.ShowingDogFacts:
                UpdateUi().Forget();
                break;
            default:
                break;
        }
    }

    private async UniTask UpdateUi()
    {
        List<string> breedsNames = await _dogFactsService.GetDogFactsDataViaRequestQueue();

        if (breedsNames == null)
        {
            Debug.LogError("DogsInfoUiController: can't get weather data");
            return;
        }

        for (int i = 0; i < breedsNames.Count; i++)
        {
            DogBreedView dogBreedView = Instantiate(_prefabDogBreedView, _holderForWeatherPeriodViews);
            dogBreedView.SetData(i + 1, breedsNames[i]);
        }
    }
}
