using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class Slider_SO_Companion : MonoBehaviour
{
    [SerializeField] SO_EventVar<float> valueSO;

    Slider slider => GetComponent<Slider>();

    private void OnEnable()
    {
        valueSO.OnValueChanged += slider.SetValueWithoutNotify;
        slider.SetValueWithoutNotify(valueSO.Value);
    }

    private void OnDisable()
    {
        valueSO.OnValueChanged -= slider.SetValueWithoutNotify;
    }
}
