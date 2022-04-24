using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SO_Set_UnityObject<T> : SO_Set<T> where T : Object
{
    public void DestroyAll()
    {
        for(int i = Items.Count - 1; i >= 0; i--)
        {
            Destroy(Items[i]);
        }

        Clear();
    }
}
