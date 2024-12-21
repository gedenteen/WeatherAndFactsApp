using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class WeatherService
{
    [Inject] private ApiLinks _apiLinks;
    [Inject] private RequestsQueue _requestsQueue;

    [Inject]
    public void Construct()
    {
        Debug.Log($"WeatherService: Weather api = {_apiLinks.Weather}");
    }

    private async UniTask<List<WeatherPeriod>> FetchWeatherDataAsync(CancellationToken ct)
    {
        // Request to API
        var request = UnityWebRequest.Get(_apiLinks.Weather);

        try
        {
            // Await response with CancellationToken
            await request.SendWebRequest().ToUniTask(null, PlayerLoopTiming.Update, cancellationToken: ct);

            if (request.result == UnityWebRequest.Result.Success)
            {
                var weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(request.downloadHandler.text);
                return weatherResponse?.Properties?.Periods;
            }

            // If request was not successful
            Debug.LogError($"WeatherService: Failed to fetch weather data: {request.error}");
            return null;
        }
        catch (OperationCanceledException)
        {
            // This happens if the request was canceled via the CancellationToken
            Debug.LogWarning("WeatherService: Weather request canceled");
            return null; 
        }
        catch (Exception e)
        {
            // Any other error (e.g. network, deserialization, etc.)
            Debug.LogError($"WeatherService: Exception while fetching weather data: {e}");
            return null;
        }
    }

    public async UniTask<List<WeatherPeriod>> GetWeatherDataViaRequestQueue()
    {
        var weatherTask = _requestsQueue.EnqueueRequest(FetchWeatherDataAsync, RequestTag.Weather);
        var weatherData = await weatherTask;
        return weatherData;
    }
}
