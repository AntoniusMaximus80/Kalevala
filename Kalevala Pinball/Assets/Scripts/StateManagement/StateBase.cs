using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public abstract class StateBase
    {
        /// <summary>
        /// The owner StateManager of this state
        /// </summary>
        public StateManager Owner { get; protected set; }

        public StateBase(StateManager owner)
        {
            Owner = owner;
        }

        public abstract void Update();

        public virtual void Activate()
        {
        }

        public virtual void Deactivate()
        {
        }

        protected virtual bool ChangeState()
        {
            return false;
        }
    }
}
