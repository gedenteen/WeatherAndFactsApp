using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class WeatherUiController : MonoBehaviour
{
    [Header("References to scene objects")]
    [SerializeField] private Transform _holderForWeatherPeriodViews;

    [Header("References to assets")]
    [SerializeField] private WeatherPeriodView _prefabWeatherPeriodView;

    [Inject] private WeatherService _weatherService;

    async void Start()
    {
        List<WeatherPeriod> weatherPeriods = await _weatherService.GetWeatherDataAsync();

        if (weatherPeriods == null)
        {
            Debug.LogError("WeatherUiController: can't get weather data");
            return;
        }

        foreach (WeatherPeriod weatherPeriod in weatherPeriods)
        {
            WeatherPeriodView weatherPeriodView = Instantiate(_prefabWeatherPeriodView, _holderForWeatherPeriodViews);
            weatherPeriodView.SetData(weatherPeriod);
        }
    }
}
