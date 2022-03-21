using UnityEngine;

/// <summary>
/// State Machine implementation.
/// Uses BaseState as base class for storing currently operating state.
/// </summary>
public partial class StateMachine<T> : MonoBehaviour where T : MonoBehaviour
{
    // Reference to currently operating state.
    protected BaseState currentState;

    /// <summary>
    /// Unity method called each frame
    /// </summary>
    protected virtual void Update()
    {
        // If we have reference to state, we should update it!
        if (currentState != null)
        {
            currentState.OnUpdate();
        }
    }

    /// <summary>
    /// Method used to change state
    /// </summary>
    /// <param name="newState">New state.</param>
    public void ChangeState(BaseState newState)
    {
        // If we currently have state, we need to destroy it!
        if (currentState != null)
        {
            currentState.OnExit();
        }

        // Swap reference
        currentState = newState;

        // If we passed reference to new state, we should assign owner of that state and initialize it!
        // If we decided to pass null as new state, nothing will happened.
        if (currentState != null)
        {
            currentState.Owner = this as T;
            currentState.OnEnter();
        }
    }
}