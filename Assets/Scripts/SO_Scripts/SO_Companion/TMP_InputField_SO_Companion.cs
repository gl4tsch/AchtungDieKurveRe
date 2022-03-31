using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// SO_Companions handle view updates on data change
/// In this case: on SO_String value change -> update input field text
/// A data update initiated by the view is handled by the view element itself
/// In this case: InputField.OnValueChanged has to be set to modify stringSOs Value on the InputField itself
/// </summary>
[RequireComponent(typeof(TMP_InputField))]
public class TMP_InputField_SO_Companion : MonoBehaviour
{
    [SerializeField] SO_EventVar<string> stringSO;

    TMP_InputField inputField => GetComponent<TMP_InputField>();

    private void OnEnable()
    {
        stringSO.OnValueChanged += inputField.SetTextWithoutNotify;
        inputField.SetTextWithoutNotify(stringSO.Value);
    }

    private void OnDisable()
    {
        stringSO.OnValueChanged -= inputField.SetTextWithoutNotify;
    }
}
