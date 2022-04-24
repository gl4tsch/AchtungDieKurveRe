using System.Collections;
using UnityEngine;

public class SO_Base : ScriptableObject
{
    [TextArea(5, 25), SerializeField]
    protected string description;

    protected Coroutine StartCoroutine(IEnumerator _task)
    {
        return CoWorker.Work(_task);
    }
}
