using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class ScreenState_MainMenu : ScreenStateBase
    {
        private bool _goToPlay;

        public ScreenState_MainMenu() : base(ScreenStateType.MainMenu)
        {
            AddTransition(ScreenStateType.Play);
            AddTransition(ScreenStateType.SettingsMenu);
        }

        public override void Update()
        {
            if ( !ChangeState() )
            {

            }
        }

        public override void Activate()
        {
            base.Activate();
        }

        protected override bool ChangeState()
        {
            // Is the Play button clicked?
            // If yes, go to play state

            if (_goToPlay)
            {
                return Owner.PerformTransition(ScreenStateType.Play);
            }

            return false;
        }

        public void OnPlayButtonUp()
        {
            _goToPlay = true;
        }
    }
}
