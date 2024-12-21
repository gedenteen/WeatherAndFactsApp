using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Zenject;
using UniRx;

/// <summary>
/// Queue that executes all requests in order. 
/// You can add requests, get their results, 
/// and remove/cancel certain requests if needed.
/// </summary>
public class RequestsQueue
{
    [Inject] AppStateController _appStateController;

    // Internal queue of requests
    private readonly Queue<IRequest> _requests = new Queue<IRequest>();

    // We store the CancellationTokenSource for the currently running request
    private CancellationTokenSource _currentCts;
    
    // Flag to indicate if we are processing the queue
    private bool _isProcessing;

    private CompositeDisposable _disposables = new CompositeDisposable();

    [Inject]
    private void Construct()
    {
        _appStateController.CurrentState.Subscribe(HandleAppStateChange).AddTo(_disposables);
    }

    ~RequestsQueue()
    {
        _disposables.Dispose();
    }

    private void HandleAppStateChange(AppState newState)
    {
        switch (newState)
        {
            case AppState.ShowingWeather:
                break;
            default:
                UnityEngine.Debug.Log($"RequestsQueue: HandleAppStateChange: going to cancel all weather requests");
                CancelCurrentWeatherRequestIfAny();
                RemoveAllPendingWeatherRequests();
                break;
        }
    }

    /// <summary>
    /// Add a generic request to the queue. 
    /// The request is defined as a function that takes a CancellationToken and returns UniTask<T>.
    /// Returns a UniTask<T> that you can await to get the request result.
    /// </summary>
    public UniTask<T> EnqueueRequest<T>(Func<CancellationToken, UniTask<T>> request, RequestTag tag = RequestTag.None)
    {
        UnityEngine.Debug.Log($"RequestsQueue: EnqueueRequest: tag={tag}");

        // We use a UniTaskCompletionSource to eventually provide the result 
        var tcs = new UniTaskCompletionSource<T>();
        
        // Create a new request with the given function and completion source
        var newRequest = new Request<T>(request, tcs, tag);
        
        // Put it into the queue
        _requests.Enqueue(newRequest);

        // If we are not already processing, let's start
        if (!_isProcessing)
        {
            // No "await" here – we just fire and forget
            _ = ProcessQueue();
        }

        // Return the task so that caller can await the result
        return tcs.Task;
    }

    /// <summary>
    /// Main loop that processes each request one by one.
    /// </summary>
    private async UniTaskVoid ProcessQueue()
    {
        _isProcessing = true;

        while (_requests.Count > 0)
        {
            // Get the first request without removing it just yet
            var currentRequest = _requests.Peek();

            // Create or reset the CancellationTokenSource for this request
            _currentCts = new CancellationTokenSource();

            try
            {
                // Execute the request
                await currentRequest.Execute(_currentCts.Token);
            }
            catch (OperationCanceledException)
            {
                // Request was canceled, let's not panic
            }
            catch (Exception e)
            {
                // Any other error – in real code, do something more meaningful
                UnityEngine.Debug.LogError($"Request threw an exception: {e}");
            }
            finally
            {
                // Remove it from the queue after execution (even if canceled or errored)
                _requests.Dequeue();
            }
        }

        _isProcessing = false;
    }

    /// Cancel the currently running request (if any).
    /// This will throw OperationCanceledException inside that request.
    public void CancelCurrentRequest()
    {
        _currentCts?.Cancel();
    }

    /// Removes all queued Weather requests that haven't started yet.
    public void RemoveAllPendingWeatherRequests()
    {
        // Filter out requests with a matching "Weather" tag
        var filtered = _requests.Where(r => !r.IsTagged(RequestTag.Weather)).ToList();

        // Clear the original queue
        _requests.Clear();

        // Put back only those that don't match the weather tag
        foreach (var request in filtered)
        {
            _requests.Enqueue(request);
        }
    }

    /// Cancel the current weather request if it is running.
    public void CancelCurrentWeatherRequestIfAny()
    {
        var current = _requests.Count > 0 ? _requests.Peek() : null;
        if (current != null && current.IsTagged(RequestTag.Weather))
        {
            CancelCurrentRequest();
        }
    }
}
