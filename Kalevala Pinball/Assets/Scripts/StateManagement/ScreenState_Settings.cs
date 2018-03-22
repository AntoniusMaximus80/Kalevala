using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class ScreenState_Settings : ScreenStateMenu
    {
        public ScreenState_Settings(StateManager owner, GameObject uiObject)
            : base(owner, uiObject, ScreenStateType.SettingsMenu)
        {
            AddTransition(ScreenStateType.Pause);
        }

        public override void Update()
        {
            //if ( !ChangeState() )
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
