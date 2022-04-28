using UnityEngine;

public class StateMachineBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    public abstract class BaseState
    {
        public T Context;

        public abstract void OnEnter();
        public abstract void OnUpdate();
        public abstract void OnExit();
    }

    protected BaseState currentState;

    protected virtual void Update()
    {
        if (currentState != null)
        {
            currentState.OnUpdate();
        }
    }

    public void ChangeState(BaseState newState)
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }

        currentState = newState;

        if (currentState != null)
        {
            currentState.Context = this as T;
            currentState.OnEnter();
        }
    }
}