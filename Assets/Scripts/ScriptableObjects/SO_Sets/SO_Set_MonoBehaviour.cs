using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SO_Set_MonoBehaviour<T> : SO_Set_UnityObject<T> where T : MonoBehaviour
{
    public void DestroyAllGameObjects()
    {
        for(int i = Items.Count - 1; i >= 0; i--)
        {
            Destroy(Items[i].gameObject);
        }

        Clear();
    }
}
