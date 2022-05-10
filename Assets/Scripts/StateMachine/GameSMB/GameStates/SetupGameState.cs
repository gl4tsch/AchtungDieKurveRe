using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameStateMachine
{
    private class SetupGameState : BaseState
    {
        public override void OnEnter()
        {
            Cursor.lockState = CursorLockMode.None;
            Context.ViewManager.ChangeView(typeof(UI_PrepView));
        }

        public override void OnUpdate() { }

        public override void OnExit() { }
    }
}