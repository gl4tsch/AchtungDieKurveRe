using System.Collections;
using UnityEngine;

public class CoWorker : MonoBehaviour
{
    private static CoWorker _instance;

    public static Coroutine Work(IEnumerator _task)
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("Can not run coroutine outside of play mode.");
            return null;
        }

        if (!_instance)
        {
            _instance = new GameObject("CoroutineWorker").AddComponent<CoWorker>();
            DontDestroyOnLoad(_instance.gameObject);
        }

        Coroutine coroutine = _instance.StartCoroutine(_task);
        return coroutine;
    }
}
