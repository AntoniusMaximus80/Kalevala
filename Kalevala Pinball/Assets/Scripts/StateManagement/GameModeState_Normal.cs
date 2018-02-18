using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class GameModeState_Normal : GameModeStateBase
    {
        public GameModeState_Normal() : base(GameModeStateType.Normal)
        {
            _validStates.Add(GameModeStateType.ForgingOfSampo);
            _validStates.Add(GameModeStateType.Sauna);
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
