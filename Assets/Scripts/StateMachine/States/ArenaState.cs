using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaState : GameStateMachine.BaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Owner.ViewManager.ChangeView(typeof(UI_ArenaView));
    }
}
