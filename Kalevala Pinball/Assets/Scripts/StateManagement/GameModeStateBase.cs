using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public abstract class GameModeStateBase
    {
        public enum GameModeStateType
        {
            Normal = 0,
            ForgingOfSampo = 1,
            TheftOfSampo = 2,
            Sauna = 3
        }

        public GameModeStateType Type { get; protected set; }

        protected List<GameModeStateType> _validStates;

        public GameModeStateBase(GameModeStateType type)
        {
            Type = type;
        }

        public abstract void Activate();

        public abstract void Deactivate();

        protected abstract bool ChangeState();
    }
}
