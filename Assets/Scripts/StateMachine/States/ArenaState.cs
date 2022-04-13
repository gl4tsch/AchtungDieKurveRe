using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaState : GameStateMachine.BaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Cursor.lockState = CursorLockMode.Locked;
        Owner.Arena.ResetArena();
        Owner.ViewManager.ChangeView(typeof(UI_ArenaView));
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Owner.ChangeState(new SetupState());
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        Owner.Arena.EndRound();
    }
}
