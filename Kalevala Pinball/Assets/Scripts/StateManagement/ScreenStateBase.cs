﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public enum ScreenStateType
    {
        MainMenu = 0,
        Play = 1,
        Pause = 2,
        SettingsMenu = 3
    }

    public abstract class ScreenStateBase
    {
        public ScreenStateType State { get; protected set; }

        public List<ScreenStateType> TargetStates { get; protected set; }

        /// <summary>
        /// The owner GameManager of this state
        /// (GameManager is the state controller class)
        /// </summary>
        public GameManager Owner { get; protected set; }

        public ScreenStateBase(ScreenStateType state)
        {
            TargetStates = new List<ScreenStateType>();
            State = state;
            Owner = GameManager.Instance;
        }

        public abstract void Update();

        /// <summary>
        /// Adds a new target state to the target state
        /// list if it isn't on the list already.
        /// </summary>
        /// <param name="targetState">A target state to
        /// be added to the target state list</param>
        /// <returns>Was the state added to the list</returns>
        public bool AddTransition(ScreenStateType targetState)
        {
            // Attempts to add a target state.
            // Will return false if the state was already added.

            if (TargetStates.Contains(targetState))
            {
                return false;
            }
            else
            {
                TargetStates.Add(targetState);
                return true;
            }
        }

        /// <summary>
        /// Removes a target state from the target
        /// state list if it is on the list.
        /// </summary>
        /// <param name="targetState">A target state to
        /// be removed from the target state list</param>
        /// <returns>Was the state removed from the list</returns>
        public bool RemoveTransition(ScreenStateType targetState)
        {
            return TargetStates.Remove(targetState);
        }

        /// <summary>
        /// Checks if the target state is on the target state list.
        /// </summary>
        /// <param name="targetState">A target state</param>
        /// <returns>Is the target state on the target state list</returns>
        public virtual bool CheckTransition(ScreenStateType targetState)
        {
            return TargetStates.Contains(targetState);
        }

        public virtual void Activate()
        {

        }

        public virtual void StartDeactivating()
        {

        }

        protected abstract bool ChangeState();
    }
}
