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
[RequireComponent(typeof(TMP_InputField))]
public class TMP_InputField_SO_Companion : MonoBehaviour
{
    [SerializeField] SO_Primitive<string> valueSO;

    TMP_InputField inputField => GetComponent<TMP_InputField>();

    private void OnEnable()
    {
        valueSO.OnValueChanged += inputField.SetTextWithoutNotify;
        inputField.SetTextWithoutNotify(valueSO.Value);
    }

    private void OnDisable()
    {
        valueSO.OnValueChanged -= inputField.SetTextWithoutNotify;
    }
}
