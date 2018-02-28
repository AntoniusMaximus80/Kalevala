using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class ScreenState_Settings : ScreenStateBase
    {
        public ScreenState_Settings(StateManager owner, GameObject menu)
            : base(owner, ScreenStateType.SettingsMenu)
        {
            ScreenObject = menu;

            AddTransition(ScreenStateType.Pause);
            //AddTransition(ScreenStateType.MainMenu);
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
