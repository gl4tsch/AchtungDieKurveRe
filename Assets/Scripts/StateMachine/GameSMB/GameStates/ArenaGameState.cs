using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaGameState : GameStateMachine.BaseState
{
    public override void OnEnter()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Context.Arena.ResetArena();
        Context.ViewManager.ChangeView(typeof(UI_ArenaView));
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Context.ChangeState(new SetupGameState());
        }
    }

    public override void OnExit()
    {
        Context.Arena.EndRound();
    }
}
