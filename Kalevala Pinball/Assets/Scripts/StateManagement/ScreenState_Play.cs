using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class ScreenState_Play : ScreenStateBase
    {
        public ScreenState_Play() : base(ScreenStateType.Play)
        {
            AddTransition(ScreenStateType.Pause);
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
            return false;
        }
    }
}
