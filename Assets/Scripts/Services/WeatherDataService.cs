using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class WeatherDataService
{
    [Inject] private ApiLinks _apiLinks;
    [Inject] private RequestsQueue _requestsQueue;

    private Dictionary<string, Sprite> _cachedSpritesObtainedFromApi = new Dictionary<string, Sprite>();

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
            await request.SendWebRequest().ToUniTask(null, PlayerLoopTiming.Update, ct);

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

    private async UniTask<Sprite> LoadIconAsync(string url, CancellationToken ct)
    {
        // Request to API
        var request = UnityWebRequestTexture.GetTexture(url);  

        try
        {
            // Await response with CancellationToken
            await request.SendWebRequest().ToUniTask(null, PlayerLoopTiming.Update, ct);

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Get texture
                Texture2D texture = DownloadHandlerTexture.GetContent(request);

                // Create sprite
                Sprite icon = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    Vector2.zero
                );

                return icon;
            }

            // If request was not successful
            Debug.LogError($"WeatherService: LoadIconAsync: Failed to fetch data: {request.error}");
            return null;
        }
        catch (OperationCanceledException)
        {
            // This happens if the request was canceled via the CancellationToken
            Debug.LogWarning("WeatherService: LoadIconAsync: request canceled");
            return null; 
        }
        catch (Exception e)
        {
            // Any other error (e.g. network, deserialization, etc.)
            Debug.LogError($"WeatherService: LoadIconAsync: Exception while fetching data: {e}");
            return null;
        }
    }

    public async UniTask<Sprite> GetIcon(string url)
    {
        if (_cachedSpritesObtainedFromApi.ContainsKey(url))
        {
            //Debug.Log($"WeatherDataService: GetIcon: i have cached sprite");
            return _cachedSpritesObtainedFromApi[url];
        }

        // Debug.Log($"WeatherDataService: GetIcon: i haven't cached sprite, let's download it");

        // The lambda expression (ct) => LoadIconAsync(url, ct) matches the Func<CancellationToken, UniTask<Sprite>> signature
        var task = _requestsQueue.EnqueueRequest(ct => LoadIconAsync(url, ct), RequestTag.Weather);
        var icon = await task;
        _cachedSpritesObtainedFromApi[url] = icon;
        return icon;
    }

}
