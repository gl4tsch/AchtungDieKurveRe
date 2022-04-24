using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// SO_Companions handle view updates on data change
/// In this case: on SO_String value change -> update input field text
/// A data update initiated by the view is handled by the view element itself
/// In this case: InputField.OnValueChanged happens in the InputField itself
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class TMP_Text_SO_Companion : MonoBehaviour
{
    [SerializeField] SO_EventVar<string> valueSO;

    TextMeshProUGUI text => GetComponent<TextMeshProUGUI>();

    private void OnEnable()
    {
        valueSO.OnValueChanged += SetText;
        SetText(valueSO.Value);
    }

    private void OnDisable()
    {
        valueSO.OnValueChanged -= SetText;
    }

    void SetText(string s)
    {
        text.text = s;
    }
}
