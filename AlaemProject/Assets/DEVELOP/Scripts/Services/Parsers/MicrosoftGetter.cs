using Cysharp.Threading.Tasks;
using System;
using System.Globalization;
using System.Threading;

public class MicrosoftGetter : TimeGetterBase
{
    public MicrosoftGetter(CancellationToken token) : base(token)
    {
    }

    public const string URL = "https://www.microsoft.com";


    public override async UniTask<DateTime> GetDateAsync()
    {
        var r = await SendRequest(URL);
        string todaysDates = r.GetResponseHeaders()["date"];
        return DateTime.ParseExact(todaysDates,
                               "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                               CultureInfo.InvariantCulture.DateTimeFormat,
                               DateTimeStyles.AssumeUniversal);
    }

    public override async UniTask<bool> PingServerAsync()
    {
        return await PingServer(URL);
    }

}
