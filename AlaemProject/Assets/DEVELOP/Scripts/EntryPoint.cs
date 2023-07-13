using DG.Tweening;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    [SerializeField] private AlarmModel _alarmModel;

    private void Awake()
    {
        DOTween.Init();
        _alarmModel.Init();
    }
}