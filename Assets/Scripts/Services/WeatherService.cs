using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class WeatherService
{
    [Inject] private ApiLinks _apiLinks;

    [Inject]
    public void Construct()
    {
        Debug.Log($"Weather api = {_apiLinks.Weather}");
        FetchWeatherAsync().Forget();
    }

    public async UniTask FetchWeatherAsync()
    {
        Debug.Log($"Fetching weather data from: {_apiLinks.Weather}");

        var response = await UnityWebRequest.Get(_apiLinks.Weather).SendWebRequest();

        if (response.result == UnityWebRequest.Result.Success)
        {
            try
            {
                // Deserialize JSON to WeatherResponse object
                var weatherData = JsonConvert.DeserializeObject<WeatherResponse>(response.downloadHandler.text);
                DisplayWeather(weatherData);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse weather data: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError($"Failed to fetch weather data: {response.error}");
        }
    }

    private void DisplayWeather(WeatherResponse weatherData)
    {
        if (weatherData == null || weatherData.Properties?.Periods == null)
        {
            Debug.LogError("Weather data is empty or invalid.");
            return;
        }

        foreach (var period in weatherData.Properties.Periods)
        {
            Debug.Log($"Period: {period.Name}");
            Debug.Log($"Temperature: {period.Temperature}{period.TemperatureUnit}");
            Debug.Log($"Forecast: {period.ShortForecast}");
            Debug.Log($"Icon URL: {period.Icon}");
            Debug.Log("-----");
        }
    }
}
