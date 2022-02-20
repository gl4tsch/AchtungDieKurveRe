using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class Toggle_SO_Companion : MonoBehaviour
{
    [SerializeField] SO_Bool valueSO;

    Toggle toggle => GetComponent<Toggle>();

    private void OnEnable()
    {
        valueSO.OnValueChanged += toggle.SetIsOnWithoutNotify;
        toggle.SetIsOnWithoutNotify(valueSO.Value);
    }

    private void OnDisable()
    {
        valueSO.OnValueChanged -= toggle.SetIsOnWithoutNotify;
    }
}
