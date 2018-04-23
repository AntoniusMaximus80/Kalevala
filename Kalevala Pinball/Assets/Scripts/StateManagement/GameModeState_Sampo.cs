using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class GameModeState_Sampo : GameModeStateBase
    {
        private ToyElevatorController _toyElevatorController;

        public GameModeState_Sampo(StateManager owner)
            : base(owner, GameModeStateType.Sampo)
        {
            AddTransition(GameModeStateType.Normal);
        }

        public override void Update()
        {
            
        }

        public override void Activate()
        {
            base.Activate();

            // When Sampo mode is activated, give extra balls thru the workshop.
            PinballManager.ActivateWorkshopExtraBalls();

            _toyElevatorController = UnityEngine.Object.FindObjectOfType<ToyElevatorController>();
            _toyElevatorController.StartGameMode(GameModeStateType.Sampo);

            
        }
    }
}
