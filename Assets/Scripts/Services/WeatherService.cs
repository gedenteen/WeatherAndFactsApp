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
        Debug.Log($"WeatherService: Weather api = {_apiLinks.Weather}");
    }

    public async UniTask<List<WeatherPeriod>> GetWeatherDataAsync()
    {
        var response = await UnityWebRequest.Get(_apiLinks.Weather).SendWebRequest();

        if (response.result == UnityWebRequest.Result.Success)
        {
            var weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(response.downloadHandler.text);
            return weatherResponse?.Properties?.Periods;
        }

        Debug.LogError($"WeatherService: Failed to fetch weather data: {response.error}");
        return null;
    }
}
