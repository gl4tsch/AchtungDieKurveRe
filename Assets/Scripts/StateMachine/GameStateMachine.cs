using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateMachine : StateMachine<GameStateMachine>
{
    [SerializeField] public UI_ViewManager ViewManager;
    [SerializeField] public Arena Arena;

    public static GameStateMachine Instance { get; private set; } // this should probably not be a singleton but oh well

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        ChangeState(new SetupState());
    }
}
