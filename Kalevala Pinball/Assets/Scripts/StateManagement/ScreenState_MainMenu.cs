using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class ScreenState_MainMenu : ScreenStateBase
    {
        private GameObject _menu;

        public ScreenState_MainMenu(StateManager owner, GameObject menu)
            : base(owner, ScreenStateType.MainMenu)
        {
            _menu = menu;

            AddTransition(ScreenStateType.Play);
            AddTransition(ScreenStateType.SettingsMenu);
        }

        public override void Update()
        {
            //if ( !ChangeState() )
            //{

            //}
        }

        public override void Activate()
        {
            base.Activate();
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
