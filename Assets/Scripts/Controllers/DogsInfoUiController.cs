using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using Unity.Burst.Intrinsics;
using UnityEngine;
using Zenject;

public class DogsInfoUiController : MonoBehaviour
{
    [Header("References to scene objects")]
    [SerializeField] private Transform _holderForViews;
    [SerializeField] private GameObject _popUpWithInfo;
    [SerializeField] private TextMeshProUGUI _textMeshOnPopUp;

    [Header("References to assets")]
    [SerializeField] private DogBreedView _prefabDogBreedView;

    [Inject] private Timings _timings;
    [Inject] private AppStateController _appStateController;
    [Inject] private DogFactsService _dogFactsService;
    
    private CompositeDisposable _disposables = new CompositeDisposable();
    private List<DogBreedView> _viewPool = new List<DogBreedView>();
    private DogApiResponse _dogApiResponse;

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
        DogApiResponse dogApiResponse = await _dogFactsService.GetDogFactsDataViaRequestQueue();
        _dogApiResponse = dogApiResponse;

        if (dogApiResponse == null)
        {
            Debug.LogError("DogsInfoUiController: can't get weather data");
            return;
        }

        List<string> breedsNames = new List<string>();

        int i;
        for (i = 0; i < dogApiResponse.Data.Count; i++)
        {
            breedsNames.Add(dogApiResponse.Data[i].Attributes.Name);
        }

        for (i = 0; i < breedsNames.Count; i++)
        {
            DogBreedView dogBreedView = GetDogBreedViewByIndex(i);
            dogBreedView.SetData(i, breedsNames[i], this);
        }

        // Disable unused items
        for (i++; i < _viewPool.Count; i++)
        {
            _viewPool[i].gameObject.SetActive(false);
        }
    }

    private DogBreedView GetDogBreedViewByIndex(int index)
    {
        // Create WeatherPeriodView if needed, else activate created item
        if (index >= _viewPool.Count)
        {
            DogBreedView dogBreedView = Instantiate(_prefabDogBreedView, _holderForViews);
            _viewPool.Add(dogBreedView);
        }
        else
        {
            _viewPool[index].gameObject.SetActive(true);
        }

        return _viewPool[index];
    }

    public void ShowPopUpWithInfo(int breedIndex)
    {
        Debug.Log($"DogsInfoUiController: ShowPopUpWithInfo: breedIndex={breedIndex}");

        if (_dogApiResponse == null)
        {
            Debug.LogError($"DogsInfoUiController: ShowPopUpWithInfo: _dogApiResponse is null");
            return;
        }

        if (breedIndex < 0 || breedIndex >= _dogApiResponse.Data.Count)
        {
            Debug.LogError($"DogsInfoUiController: ShowPopUpWithInfo: invalid breedIndex = {breedIndex}");
            return;
        }

        _textMeshOnPopUp.text = string.Concat(
            "Name: ", _dogApiResponse.Data[breedIndex].Attributes.Name, "\n\n",
            "Description: ", _dogApiResponse.Data[breedIndex].Attributes.Description, "\n\n",
            "Life min: ", _dogApiResponse.Data[breedIndex].Attributes.Life.Min, "\n\n",
            "Life max: ", _dogApiResponse.Data[breedIndex].Attributes.Life.Max, "\n\n"
            );
            
        _popUpWithInfo.SetActive(true);
    }
}
