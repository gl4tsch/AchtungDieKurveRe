using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ParsedPrimitives/FloatAsString")]
public class SO_FloatAsString : SO_ParsedPrimitive<float, string>
{
    protected override string Parse(float primitive)
    {
        return primitive.ToString();
    }

    protected override float ReverseParse(string primitive)
    {
        return float.Parse(primitive);
    }
}
