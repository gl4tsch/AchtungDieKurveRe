using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SO_EventVar<T> : SO_Base
{
    public T Value => value;
    private T value;

    [SerializeField] protected T initialValue;

    public Action<T> OnValueChanged;
    public Action<T, T> OnValueChangedFromTo;

    protected virtual void OnEnable()
    {
        SetValue(initialValue);
    }

    public virtual void SetValue(T _value)
    {
        T oldValue = value;
        value = _value;
        OnValueChanged?.Invoke(value);
        OnValueChangedFromTo?.Invoke(oldValue, value);
    }

    public virtual void SetValueWithoutNotify(T _value)
    {
        value = _value;
    }
}
