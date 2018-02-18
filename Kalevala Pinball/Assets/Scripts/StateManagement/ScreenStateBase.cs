using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public abstract class ScreenStateBase
    {
        public enum ScreenStateType
        {
            MainMenu = 0,
            Game = 1,
            Pause = 2,
            SettingsMenu = 3
        }
        
        public ScreenStateType Type { get; protected set; }

        protected List<ScreenStateType> _validStates;

        public ScreenStateBase(ScreenStateType type)
        {
            Type = type;
        }

        public abstract void Activate();

        public abstract void Deactivate();

        protected abstract bool ChangeState();
    }
}
