using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AlarmModel : MonoBehaviour
{
    [Serializable]
    public struct TimeData
    {
        public TimeData(int hours, int minutes, int seconds)
        {
            this.hours = hours;
            this.minutes = minutes;
            this.seconds = seconds;
        }

        //С range аттрибутом какие то баги в версии юнити 2022 lts 
        /*[Range(0, 23)]*/
        [Min(0)]
        public int hours;
        /*[Range(0, 59)]*/
        [Min(0)]
        public int minutes;
        /*[Range(0, 59)]*/
        [Min(0)]
        public int seconds;

        public int TotalSeconds => (hours * 3600) + (minutes * 60) + seconds;

        public static TimeData Zero => new TimeData(0, 0, 0);

        public static bool operator ==(TimeData a, TimeData b)
        {
            return a.hours == b.hours &&
                   a.minutes == b.minutes &&
                   a.seconds == b.seconds;
        }

        public static bool operator !=(TimeData a, TimeData b)
        {
            return a.hours != b.hours ||
                   a.minutes != b.minutes ||
                   a.seconds != b.seconds;
        }
    }

    [SerializeField] private AlarmController _controller;
    [SerializeField] private TimeData _verificationPeriod;
    [SerializeField] private bool _updateAlarmEveryFrame = true;
    private InternetTimeHandler _timeServerHandler;
    private CancellationToken _destroyToken;
    private bool _isInited = false;

    /// <summary>
    /// Юнитевское время последней верификации времени с сервером
    /// </summary>
    private float _lastVerificatedTime;

    private float _lastSetTime;

    private void Update()
    {
        if (!_isInited)
            return;

        if (_updateAlarmEveryFrame)
            CalculateTime();
    }

    public async UniTaskVoid Init()
    {
        //init handler
        _destroyToken = this.GetCancellationTokenOnDestroy();
        _timeServerHandler = new InternetTimeHandler(
            new WorlTimeAPIGetter(_destroyToken),
            new MicrosoftGetter(_destroyToken)
            );

        //get server time at start
        await GetTimeFromServer();

        //get server time every N time
        StartGetServerTimeEveryNTime();

        _isInited = true;
    }

    private async UniTask GetTimeFromServer()
    {
        var serverTime = await _timeServerHandler.GetDateAsync();

        _controller.SetTime(serverTime);
        _lastVerificatedTime = Time.realtimeSinceStartup;
        _lastSetTime = Time.realtimeSinceStartup;
        Debug.Log($"Server time is {serverTime}");
    }

    private void CalculateTime()
    {
        var addableSeconds = Time.realtimeSinceStartup - _lastSetTime;
        var time = _controller.CurrentTime.AddSeconds(addableSeconds);
        _controller.SetTime(time);
        _lastSetTime = Time.realtimeSinceStartup;

    }

    private async UniTaskVoid StartGetServerTimeEveryNTime()
    {
        if (_verificationPeriod == TimeData.Zero)
            return;
        do
        {
            await UniTask.WaitUntil(() => Time.realtimeSinceStartup >= _lastVerificatedTime + _verificationPeriod.TotalSeconds,
                PlayerLoopTiming.Update,
                _destroyToken);

            if (_destroyToken.IsCancellationRequested)
                break;

            await GetTimeFromServer();
        } while (true);
    }
}