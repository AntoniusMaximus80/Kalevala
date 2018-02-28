﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class ScreenState_Pause : ScreenStateBase
    {
        private float playTimeScale = 1f;

        public ScreenState_Pause(StateManager owner, GameObject menu)
            : base(owner, ScreenStateType.Pause)
        {
            ScreenObject = menu;

            AddTransition(ScreenStateType.Play);
            AddTransition(ScreenStateType.SettingsMenu);
            AddTransition(ScreenStateType.MainMenu);
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

            // The time is stopped if it isn't already
            if (Time.timeScale > 0f)
            {
                playTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }
        }

        /// <summary>
        /// Continues time.
        /// </summary>
        public void ResumeGame()
        {
            Time.timeScale = playTimeScale;
        }

        //protected override bool ChangeState()
        //{
        //    bool stateChanged = false;



        //    return stateChanged;
        //}
    }
}
