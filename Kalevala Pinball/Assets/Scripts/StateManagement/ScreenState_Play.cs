using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class ScreenState_Play : ScreenStateBase
    {
        public ScreenState_Play(StateManager owner)
            : base(owner, ScreenStateType.Play)
        {
            AddTransition(ScreenStateType.Pause);
            AddTransition(ScreenStateType.GameOver);
        }

        public override void Update()
        {
            if (!ChangeState())
            {
                PinballManager.Instance.UpdatePinballs();
            }
        }

        protected override bool ChangeState()
        {
            bool stateChanged = false;

            if (PinballManager.Instance.OutOfBalls())
            {
                Owner.PerformTransition(ScreenStateType.GameOver);
                Owner.EndGame(true);
                stateChanged = true;
            }

            return stateChanged;
        }
    }
}
