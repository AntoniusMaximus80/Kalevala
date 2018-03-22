using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kalevala
{
    public class ScreenState_MainMenu : ScreenStateMenu
    {
        public ScreenState_MainMenu(StateManager owner, GameObject uiObject)
            : base(owner, uiObject, ScreenStateType.MainMenu)
        {
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
