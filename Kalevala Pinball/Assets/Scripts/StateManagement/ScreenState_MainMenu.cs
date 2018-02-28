using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class ScreenState_MainMenu : ScreenStateBase
    {
        public ScreenState_MainMenu(StateManager owner, GameObject uiObject)
            : base(owner, ScreenStateType.MainMenu)
        {
            ScreenObject = uiObject;

            AddTransition(ScreenStateType.Play);
            AddTransition(ScreenStateType.SettingsMenu);
        }

        public override void Update()
        {
            //if ( !ChangeState() )
            //{

            //}
        }

        //protected override bool ChangeState()
        //{
        //    // Is the Play button clicked?
        //    // If yes, go to play state

        //    if (_goToPlay)
        //    {
        //        return Owner.PerformTransition(ScreenStateType.Play);
        //    }

        //    return false;
        //}
    }
}
