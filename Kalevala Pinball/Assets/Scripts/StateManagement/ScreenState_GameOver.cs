using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class ScreenState_GameOver : ScreenStateBase
    {
        public ScreenState_GameOver(StateManager owner, GameObject uiObject)
            : base(owner, ScreenStateType.GameOver)
        {
            ScreenObject = uiObject;

            AddTransition(ScreenStateType.Play);
            AddTransition(ScreenStateType.MainMenu);
        }

        public override void Update()
        {
            //if (!ChangeState())
            //{
            //}
        }

        //protected override bool ChangeState()
        //{
        //    bool stateChanged = false;



        //    return stateChanged;
        //}
    }
}
