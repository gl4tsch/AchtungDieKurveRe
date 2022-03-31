using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Primitives/PlayerPrefsInt")]
public class SO_PlayerPrefsInt : SO_EventVar<int>
{
    [SerializeField] string playerPrefsKey;

    protected override void OnEnable()
    {
        LoadFromPlayerPrefs();
    }

    public void LoadFromPlayerPrefs()
    {
        SetValue(PlayerPrefs.GetInt(playerPrefsKey, initialValue));
        Debug.Log("loading " + playerPrefsKey + " from PlayerPrefs: " + Value);
    }

    public override void SetValue(int _value)
    {
        base.SetValue(_value);
        PlayerPrefs.SetFloat(playerPrefsKey, _value);
    }

    public override void SetValueWithoutNotify(int _value)
    {
        base.SetValueWithoutNotify(_value);
        PlayerPrefs.SetFloat(playerPrefsKey, _value);
    }
}
