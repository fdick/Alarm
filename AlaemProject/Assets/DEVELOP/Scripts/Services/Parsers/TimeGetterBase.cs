using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public abstract class TimeGetterBase
{
    public TimeGetterBase(CancellationToken token)
    {
        if (token == null)
            Debug.LogError($"Cancelation toke is null!");
        _token = token;
    }

    protected CancellationToken _token;


    public abstract UniTask<DateTime> GetDateAsync();

    public abstract UniTask<bool> PingServerAsync();

    protected async UniTask<UnityWebRequest> SendRequest(string url)
    {
        var request = UnityWebRequest.Get(url);

        try
        {
            await request.SendWebRequest().WithCancellation(_token);
        }
        catch (Exception ex)
        {
            Debug.Log($@"Something went wrong! \n1. {request.error}. \n2. {ex}");
            return null;
        }

        return request;
    }

    protected async UniTask<bool> PingServer(string url)
    {
        var req = UnityWebRequest.Get(url);
        try
        {
            await req.SendWebRequest().WithCancellation(_token);

        }
        catch (Exception ex)
        {
            Debug.Log($"No answer from {url}");
            Debug.Log($"Something went wrong! 1. {req.error}. 2. {ex}");
            return false;
        }

        return true;
    }
}
