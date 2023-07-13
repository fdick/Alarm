using System;
using UnityEngine;

public class AlarmController : MonoBehaviour
{
    [SerializeField] private AlarmUIData _data;

    public DateTime CurrentTime { get; private set; }
    public Action OnSetTime { get; set; }

    private const int HCEL = 30;
    private const int MSCEL = 6;

    private void OnDestroy()
    {
        OnSetTime = null;
    }

    public void SetArrowsRotation(DateTime time)
    {
        //angle calculation
        var h = _data._arrows[0];
        var m = _data._arrows[1];
        var s = _data._arrows[2];

        var hRot = Quaternion.AngleAxis(time.TimeOfDay.Hours * HCEL, -Vector3.forward);
        var mRot = Quaternion.AngleAxis(time.TimeOfDay.Minutes * MSCEL, -Vector3.forward);
        var sRot = Quaternion.AngleAxis(time.TimeOfDay.Seconds * MSCEL, -Vector3.forward);

        h.transform.rotation = hRot;
        m.transform.rotation = mRot;
        s.transform.rotation = sRot;
    }

    public void SetDigitTime(DateTime time)
    {
        var outText = time.TimeOfDay.ToString();
        if (outText.Length > 8)
            outText = outText.Remove(8, outText.Length - 8);
        _data._digitalTimeText.text = outText;
    }

    public void SetTime(DateTime newTime, bool updateVisual = true)
    {
        OnSetTime?.Invoke();
        CurrentTime = newTime;

        if (updateVisual)
        {
            SetArrowsRotation(CurrentTime);
            SetDigitTime(CurrentTime);
        }
    }
}