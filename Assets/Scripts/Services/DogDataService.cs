using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class DogDataService
{
    [Inject] private ApiLinks _apiLinks;
    [Inject] private RequestsQueue _requestsQueue;

    [Inject]
    public void Construct()
    {
        Debug.Log($"DogDataService: dog facts api = {_apiLinks.DogFacts}");
    }

    private async UniTask<DogApiResponse> FetchDogBreedsDataAsync(CancellationToken ct)
    {
        // Request to API
        var request = UnityWebRequest.Get(_apiLinks.DogFacts);

        try
        {
            // Await response with CancellationToken
            await request.SendWebRequest().ToUniTask(null, PlayerLoopTiming.Update, ct);

            Debug.Log($"FetchDogBreedsDataAsync: got {request.downloadHandler.text}");

            if (request.result == UnityWebRequest.Result.Success)
            {
                var json = request.downloadHandler.text;
                DogApiResponse response = JsonConvert.DeserializeObject<DogApiResponse>(json);

                Debug.Log($"FetchDogBreedsDataAsync: Parsed {response.Data.Count} breeds from the API.");

                return response;
            }

            // If request was not successful
            Debug.LogError($"FetchDogBreedsDataAsync: Failed to fetch data: {request.error}");
            return null;
        }
        catch (OperationCanceledException)
        {
            // This happens if the request was canceled via the CancellationToken
            Debug.LogWarning("FetchDogBreedsDataAsync: request canceled");
            return null; 
        }
        catch (Exception e)
        {
            // Any other error (e.g. network, deserialization, etc.)
            Debug.LogError($"FetchDogBreedsDataAsync: Exception while fetching data: {e}");
            return null;
        }
    }

    public async UniTask<DogApiResponse> GetDogBreedsDataViaRequestQueue()
    {
        var task = _requestsQueue.EnqueueRequest(FetchDogBreedsDataAsync, RequestTag.DogFacts);
        var data = await task;
        return data;
    }
}
