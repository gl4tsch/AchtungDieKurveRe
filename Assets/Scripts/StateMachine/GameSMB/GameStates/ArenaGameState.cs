using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameStateMachine
{
    private class ArenaGameState : BaseState
    {
        Arena arena => Context.Arena;

        public override void OnEnter()
        {
            Cursor.lockState = CursorLockMode.Locked;
            arena.ResetArena();
            Context.ViewManager.ChangeView(typeof(UI_ArenaView));
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Context.ChangeState(Context.SetupState);
            }
        }

        public override void OnExit()
        {
            arena.EndRound();
        }
    }
}
