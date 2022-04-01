using UnityEngine;

public partial class StateMachine<T> : MonoBehaviour where T : MonoBehaviour
{
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
            currentState.Owner = this as T;
            currentState.OnEnter();
        }
    }
}