﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class GameModeState_Normal : GameModeStateBase
    {
        public GameModeState_Normal() : base(GameModeStateType.Normal)
        {
            //AddTransition(GameModeStateType.X);
            //AddTransition(GameModeStateType.Y);
        }

        public override void Update()
        {
            if (!ChangeState())
            {

            }
        }

        public override void Activate()
        {
            base.Activate();
        }

        protected override bool ChangeState()
        {
            //if (x)
            //{
            //    return Owner.PerformTransition(GameModeStateType.X);
            //}

            return false;
        }
    }
}
