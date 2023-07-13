using Cysharp.Threading.Tasks;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

public class WorlTimeAPIGetter : TimeGetterBase
{
    public WorlTimeAPIGetter(CancellationToken token) : base(token)
    {

    }

    private class JsonTimeData
    {
        public string datetime;
    }

    public const string URL = "https://worldtimeapi.org/api/ip";

    

    public async override UniTask<DateTime> GetDateAsync()
    {
       var req = await SendRequest(URL);
        if (req == null)
            return DateTime.MinValue;

       return ParseJson(req.downloadHandler.text);
    }


    public override async UniTask<bool> PingServerAsync()
    {
        return await PingServer(URL);
    }

    private DateTime ParseJson(string jsonText)
    {
        var jTimeData = JsonUtility.FromJson<JsonTimeData>(jsonText);

        var parseText = jTimeData.datetime;
        var dateMatch = Regex.Match(parseText, @"^\d{4}-\d{2}-\d{2}");
        var timeMatch = Regex.Match(parseText, @"\d{2}:\d{2}:\d{2}");
        var date = DateTime.Parse($"{dateMatch.Value} {timeMatch.Value}");
        return date;
    }

}
