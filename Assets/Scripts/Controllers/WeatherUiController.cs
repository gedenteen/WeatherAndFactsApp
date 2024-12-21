using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class WeatherUiController : MonoBehaviour
{
    [Header("References to scene objects")]
    [SerializeField] private Transform _holderForWeatherPeriodViews;

    [Header("References to assets")]
    [SerializeField] private WeatherPeriodView _prefabWeatherPeriodView;

    [Inject] private Timings _timings;
    [Inject] private WeatherService _weatherService;

    private CancellationTokenSource _cancellationTokenSource;
    private List<WeatherPeriodView> _viewPool = new List<WeatherPeriodView>();

    [Inject]
    private void Construct()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        UpdateUi(_cancellationTokenSource.Token).Forget();
    }

    private void OnDestroy()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }

    private async UniTask UpdateUi(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            List<WeatherPeriod> weatherPeriods = await _weatherService.GetWeatherDataViaRequestQueue();

            if (weatherPeriods == null)
            {
                Debug.LogError("WeatherUiController: can't get weather data");
                return;
            }

            Debug.Log($"WeatherUiController: going to show data, " +
                      $"weatherPeriods.Count={weatherPeriods.Count} _viewPool.Count={_viewPool.Count}");

            int i;
            for (i = 0; i < weatherPeriods.Count; i++)
            {
                WeatherPeriodView weatherPeriodView = GetWeatherPeriodViewByIndex(i);
                _viewPool[i].SetData(weatherPeriods[i]);
            }

            // Disable unused items
            for (i++; i < _viewPool.Count; i++)
            {
                _viewPool[i].gameObject.SetActive(false);
            }

            await UniTask.WaitForSeconds(_timings.SecondsForUpdateWeatherData);
        }
    }

    private WeatherPeriodView GetWeatherPeriodViewByIndex(int index)
    {
        // Create WeatherPeriodView if needed, else activate created item
        if (index >= _viewPool.Count)
        {
            WeatherPeriodView weatherPeriodView = Instantiate(_prefabWeatherPeriodView, _holderForWeatherPeriodViews);
            _viewPool.Add(weatherPeriodView);
        }
        else
        {
            _viewPool[index].gameObject.SetActive(true);
        }

        return _viewPool[index];
    }
}