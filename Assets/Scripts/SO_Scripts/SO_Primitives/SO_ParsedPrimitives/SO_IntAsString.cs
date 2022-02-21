using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ParsedPrimitives/IntAsString")]
public class SO_IntAsString : SO_ParsedPrimitive<int, string>
{
    protected override string Parse(int primitive)
    {
        return primitive.ToString();
    }

    protected override int ReverseParse(string primitive)
    {
        return int.Parse(primitive);
    }
}
