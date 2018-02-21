using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class ScreenState_Pause : ScreenStateBase
    {
        public ScreenState_Pause(StateManager owner)
            : base(owner, ScreenStateType.Pause)
        {
            AddTransition(ScreenStateType.Play);
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
        //    bool stateChanged = false;



        //    return stateChanged;
        //}
    }
}
