using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Snake
{
    private class DeadSnakeState : BaseState
    {
        public override void OnEnter()
        {
            Context.LeftAction.Disable();
            Context.RightAction.Disable();
            Context.FireAction.Disable();

            Context.Ability.Stop();

            Debug.Log(Context.Color + " ded!");
        }

        public override void OnExit()
        {

        }

        public override void OnUpdate()
        {

        }
    }
}