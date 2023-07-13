using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlarmUIData : MonoBehaviour
{
    [field: Header("Расположить стрелки по убывающей важности!")]
    [field: SerializeField] public Image[] _arrows;

    [field: SerializeField] public TextMeshProUGUI _digitalTimeText;
}
