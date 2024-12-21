using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

public interface IRequest
{
    bool IsTagged(RequestTag tag);
    UniTask Execute(CancellationToken token);
}

// Concrete request implementation
public class Request<T> : IRequest
{
    private readonly Func<CancellationToken, UniTask<T>> _request;
    private readonly UniTaskCompletionSource<T> _tcs;
    private readonly RequestTag _tag;

    public Request(Func<CancellationToken, UniTask<T>> request, UniTaskCompletionSource<T> tcs, RequestTag tag)
    {
        _request = request;
        _tcs = tcs;
        _tag = tag;
    }

    public bool IsTagged(RequestTag tag) => _tag == tag;

    public async UniTask Execute(CancellationToken token)
    {
        try
        {
            T result = await _request(token);
            _tcs.TrySetResult(result);
        }
        catch (OperationCanceledException)
        {
            _tcs.TrySetCanceled();
            throw;
        }
        catch (Exception e)
        {
            _tcs.TrySetException(e);
            throw;
        }
    }
}
