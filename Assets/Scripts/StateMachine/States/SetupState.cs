using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupState : GameStateMachine.BaseState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Owner.ViewManager.ChangeView(typeof(UI_PrepView));
    }
}
