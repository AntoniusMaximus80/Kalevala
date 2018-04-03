using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class GameModeState_Sampo : GameModeStateBase
    {
        public GameModeState_Sampo(StateManager owner)
            : base(owner, GameModeStateType.Sampo)
        {
            AddTransition(GameModeStateType.Normal);
        }

        public override void Update()
        {
            
        }

        public override void Activate()
        {
            base.Activate();
        }
    }
}
