using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class ScreenState_MainMenu : ScreenStateBase
    {
        public ScreenState_MainMenu() : base(ScreenStateType.MainMenu)
        {
            _validStates.Add(ScreenStateType.Game);
            _validStates.Add(ScreenStateType.SettingsMenu);
        }

        public override void Activate()
        {

        }

        public override void Deactivate()
        {

        }

        protected override bool ChangeState()
        {
            return false;
        }
    }
}
