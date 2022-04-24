using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SO_ParsedEventVar<Var, AsVar> : SO_EventVar<AsVar>
{
    [SerializeField] SO_EventVar<Var> varSO;
    public SO_EventVar<Var> VarSO
    {
        set
        {
            varSO = value;
            value.OnValueChanged += OnVarValueChanged;
            subscribed = true;
        }

        get => varSO;
    }

    [NonSerialized] bool subscribed = false;
    [SerializeField, HideInInspector]
    bool disclaimerSet = false; // TODO there has to be a better way. does this even work?

    private void Awake()
    {
        if (disclaimerSet) return;
        description = "Initial Value will be taken from PrimitiveSO\n" + description;
        disclaimerSet = true;
    }

    protected new virtual void OnEnable()
    {
        if (varSO == null) return;
        if (!subscribed) varSO.OnValueChanged += OnVarValueChanged;
        OnVarValueChanged(varSO.Value);
    }

    private void OnDisable()
    {
        if (varSO == null) return;
        varSO.OnValueChanged -= OnVarValueChanged;
    }

    public override void SetValue(AsVar value)
    {
        base.SetValue(value);

        if (varSO == null) return;

        varSO.OnValueChanged -= OnVarValueChanged;
        varSO.SetValue(ReverseParse(value));
        varSO.OnValueChanged += OnVarValueChanged;
    }

    protected void OnVarValueChanged(Var varToParse)
    {
        SetValue(Parse(varToParse));
    }

    protected abstract AsVar Parse(Var primitive);
    protected abstract Var ReverseParse(AsVar primitive);
}
