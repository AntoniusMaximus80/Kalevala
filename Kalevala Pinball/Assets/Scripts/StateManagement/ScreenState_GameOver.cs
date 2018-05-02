using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kalevala
{
    public class ScreenState_GameOver : ScreenStateMenu
    {
        private bool _displayMenu;
        private float _uiDelay;
        private float _elapsedTime;

        public ScreenState_GameOver(StateManager owner, GameObject uiObject, float uiDelay)
            : base(owner, uiObject, ScreenStateType.GameOver)
        {
            _uiDelay = uiDelay;

            AddTransition(ScreenStateType.Play);
            AddTransition(ScreenStateType.MainMenu);
        }

        public override void Update()
        {
            //if (!ChangeState())
            //{
            //}

            if (!_displayMenu)
            {
                _elapsedTime += Time.deltaTime;
                if (_elapsedTime >= _uiDelay)
                {
                    _displayMenu = true;
                    ScreenObject.SetActive(true);
                }
            }
        }

        public override void Activate()
        {
            _elapsedTime = 0;
        }

        public override void Deactivate()
        {
            base.Deactivate();

            _displayMenu = false;
        }

        //protected override bool ChangeState()
        //{
        //    bool stateChanged = false;



        //    return stateChanged;
        //}
    }
}
