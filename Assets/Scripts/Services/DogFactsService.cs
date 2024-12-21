using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class DogFactsService
{
    [Inject] private ApiLinks _apiLinks;
    [Inject] private RequestsQueue _requestsQueue;

    [Inject]
    public void Construct()
    {
        Debug.Log($"DogFactsService: dog facts api = {_apiLinks.DogFacts}");
    }

    private async UniTask<List<string>> FetchDogFactsDataAsync(CancellationToken ct)
    {
        // Request to API
        var request = UnityWebRequest.Get(_apiLinks.DogFacts);

        try
        {
            // Await response with CancellationToken
            await request.SendWebRequest().ToUniTask(null, PlayerLoopTiming.Update, ct);

            Debug.Log($"FetchDogFactsDataAsync: got {request.downloadHandler.text}");

            if (request.result == UnityWebRequest.Result.Success)
            {
                var json = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<DogApiResponse>(json);

                Debug.Log($"FetchDogFactsDataAsync: Parsed {response.Data.Count} breeds from the API.");

                List<string> names = new List<string>();

                for (int i = 0; i < response.Data.Count; i++)
                {
                    names.Add(response.Data[i].Attributes.Name);
                }

                return names;//response.Data;
            }

            // If request was not successful
            Debug.LogError($"FetchDogFactsDataAsync: Failed to fetch data: {request.error}");
            return null;
        }
        catch (OperationCanceledException)
        {
            // This happens if the request was canceled via the CancellationToken
            Debug.LogWarning("FetchDogFactsDataAsync: request canceled");
            return null; 
        }
        catch (Exception e)
        {
            // Any other error (e.g. network, deserialization, etc.)
            Debug.LogError($"FetchDogFactsDataAsync: Exception while fetching data: {e}");
            return null;
        }
    }

    public async UniTask<List<string>> GetDogFactsDataViaRequestQueue()
    {
        var task = _requestsQueue.EnqueueRequest(FetchDogFactsDataAsync, RequestTag.DogFacts);
        var data = await task;
        return data;
    }
}
