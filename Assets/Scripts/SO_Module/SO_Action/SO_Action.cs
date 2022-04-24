using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;

[CreateAssetMenu(fileName = "New SO Action", menuName = "ScriptableObjects/Actions/Action", order = 1)]
public class SO_Action : ScriptableObject
{
    private Action action;

    public void Invoke()
    {
        action?.Invoke();
    }

    public void Subscribe(Action _delegate){
        action += _delegate;
    }

    public void Unsubscribe(Action _delegate)
    {
        action -= _delegate;
    }
}

public abstract class SO_Action<T> : ScriptableObject
{
    private Action<T> action;

    public void Invoke(T _t)
    {
        action?.Invoke(_t);
    }

    public void Subscribe(Action<T> _delegate)
    {
        action += _delegate;
    }

    public void Unsubscribe(Action<T> _delegate)
    {
        action -= _delegate;
    }
}

public abstract class SO_Action<T1, T2> : ScriptableObject
{
    private Action<T1, T2> action;

    public void Invoke(T1 _t1, T2 _t2)
    {
        action?.Invoke(_t1, _t2);
    }

    public void Subscribe(Action<T1, T2> _delegate)
    {
        action += _delegate;
    }

    public void Unsubscribe(Action<T1, T2> _delegate)
    {
        action -= _delegate;
    }
}