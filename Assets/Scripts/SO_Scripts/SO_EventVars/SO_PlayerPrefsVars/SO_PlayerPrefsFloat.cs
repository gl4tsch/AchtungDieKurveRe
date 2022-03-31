using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Primitives/PlayerPrefsFloat")]
public class SO_PlayerPrefsFloat : SO_EventVar<float>
{
    [SerializeField] string playerPrefsKey;

    protected override void OnEnable()
    {
        LoadFromPlayerPrefs();
    }

    public void LoadFromPlayerPrefs()
    {
        base.SetValue(PlayerPrefs.GetFloat(playerPrefsKey, initialValue));
    }

    public override void SetValue(float _value)
    {
        base.SetValue(_value);
        PlayerPrefs.SetFloat(playerPrefsKey, _value);
    }

    public override void SetValueWithoutNotify(float _value)
    {
        base.SetValueWithoutNotify(_value);
        PlayerPrefs.SetFloat(playerPrefsKey, _value);
    }
}