using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameStateMachine : StateMachineBehaviour<GameStateMachine>
{
    [SerializeField] UI_ViewManager ViewManager;
    [SerializeField] Arena Arena;

    public static GameStateMachine Instance { get; private set; } // this should probably not be a singleton

    public BaseState SetupState = new SetupGameState();
    public BaseState ArenaState = new ArenaGameState();

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
        ChangeState(new SetupGameState());
    }
}
