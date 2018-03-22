using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kalevala
{
    public class ScreenState_GameOver : ScreenStateMenu
    {
        public ScreenState_GameOver(StateManager owner, GameObject uiObject)
            : base(owner, uiObject, ScreenStateType.GameOver)
        {
            AddTransition(ScreenStateType.Play);
            AddTransition(ScreenStateType.MainMenu);
        }

        public override void Update()
        {
            //if (!ChangeState())
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
