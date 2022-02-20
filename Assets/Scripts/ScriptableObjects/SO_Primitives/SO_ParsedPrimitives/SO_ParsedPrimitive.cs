using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SO_ParsedPrimitive<Primitive, AsPrimitive> : SO_Primitive<AsPrimitive>
{
    [SerializeField] SO_Primitive<Primitive> primitiveSO;
    public SO_Primitive<Primitive> PrimitiveSO
    {
        set
        {
            primitiveSO = value;
            value.OnValueChanged += OnPrimitiveValueChanged;
            subscribed = true;
        }

        get => primitiveSO;
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

    protected virtual void OnEnable()
    {
        if (primitiveSO == null) return;
        if (!subscribed) primitiveSO.OnValueChanged += OnPrimitiveValueChanged;
        OnPrimitiveValueChanged(primitiveSO.Value);
    }

    private void OnDisable()
    {
        if (primitiveSO == null) return;
        primitiveSO.OnValueChanged -= OnPrimitiveValueChanged;
    }

    public override void SetValue(AsPrimitive value)
    {
        base.SetValue(value);

        if (primitiveSO == null) return;

        primitiveSO.OnValueChanged -= OnPrimitiveValueChanged;
        primitiveSO.SetValue(ReverseParse(value));
        primitiveSO.OnValueChanged += OnPrimitiveValueChanged;
    }

    protected void OnPrimitiveValueChanged(Primitive primitiveToParse)
    {
        SetValue(Parse(primitiveToParse));
    }

    protected abstract AsPrimitive Parse(Primitive primitive);
    protected abstract Primitive ReverseParse(AsPrimitive primitive);
}
