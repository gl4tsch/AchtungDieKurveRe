using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Primitives/Int")]
public class SO_Int : SO_EventVar<int> 
{
    public static SO_Int operator ++(SO_Int i)
    {
        i.SetValue(i.Value + 1);
        return i;
    }
}
