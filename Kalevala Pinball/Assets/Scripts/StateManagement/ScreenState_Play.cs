﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class ScreenState_Play : ScreenStateBase
    {
        public ScreenState_Play(StateManager owner)
            : base(owner, ScreenStateType.Play)
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
