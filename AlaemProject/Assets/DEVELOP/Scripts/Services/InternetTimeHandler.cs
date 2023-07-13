using System;
using Cysharp.Threading.Tasks;


public class InternetTimeHandler
{
    public InternetTimeHandler(params TimeGetterBase[] timeServers)
    {
        _timeServers = timeServers;
    }

    private TimeGetterBase[] _timeServers;

    public async UniTask<DateTime> GetDateAsync()
    {
        if (_timeServers == null || _timeServers.Length == 0)
            return DateTime.MinValue;

        var server = await GetWorkingServer();

        if(server == null)
            return DateTime.MinValue;

        return await server.GetDateAsync();
    }

    private async UniTask<TimeGetterBase> GetWorkingServer()
    {
        foreach (var s in _timeServers)
        {
            if (await s.PingServerAsync())
                return s;
        }

        return null;
    }

}