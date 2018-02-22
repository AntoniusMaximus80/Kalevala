using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class ScreenState_Pause : ScreenStateBase
    {
        private GameObject _menu;

        public ScreenState_Pause(StateManager owner, GameObject menu)
            : base(owner, ScreenStateType.Pause)
        {
            _menu = menu;

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
