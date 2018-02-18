using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class ScreenState_Game : ScreenStateBase
    {
        public ScreenState_Game() : base(ScreenStateType.Game)
        {
            _validStates.Add(ScreenStateType.Pause);
        }

        public override void Activate()
        {
            Type = ScreenStateType.Game;
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
