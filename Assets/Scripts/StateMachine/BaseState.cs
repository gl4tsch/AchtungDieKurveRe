using UnityEngine;

public partial class StateMachine<T> : MonoBehaviour where T : MonoBehaviour
{
    public abstract class BaseState
    {
        public T Owner;

        public virtual void OnEnter() { }

        public virtual void OnUpdate() { }

        public virtual void OnExit() { }
    }
}