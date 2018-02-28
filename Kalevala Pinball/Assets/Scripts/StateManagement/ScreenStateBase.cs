﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public enum ScreenStateType
    {
        None = 0,
        MainMenu = 1,
        Play = 2,
        Pause = 3,
        SettingsMenu = 4
    }

    public abstract class ScreenStateBase
    {
        public ScreenStateType State { get; protected set; }

        public List<ScreenStateType> TargetStates { get; protected set; }

        /// <summary>
        /// The owner GameManager of this state
        /// (GameManager is the state controller class)
        /// </summary>
        public StateManager Owner { get; protected set; }

        public GameObject ScreenObject { get; protected set; }

        public ScreenStateBase(StateManager owner, ScreenStateType state)
        {
            TargetStates = new List<ScreenStateType>();
            State = state;
            Owner = owner;
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
            if (ScreenObject != null)
            {
                ScreenObject.SetActive(true);
            }
        }

        public virtual void Deactivate()
        {
            if (ScreenObject != null)
            {
                ScreenObject.SetActive(false);
            }
        }

        //protected abstract bool ChangeState();
    }
}
