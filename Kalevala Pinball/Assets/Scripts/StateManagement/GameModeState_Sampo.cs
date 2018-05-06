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

            // Moved this here as this is probably the correct place.
            Viewscreen.StartSampoMode();

            _toyElevatorController = UnityEngine.Object.FindObjectOfType<ToyElevatorController>();
            _toyElevatorController.StartGameMode(GameModeStateType.Sampo);

            
        }

        public override void Deactivate()
        {
            base.Deactivate();

            Viewscreen.EndSampoMode();

            _toyElevatorController = UnityEngine.Object.FindObjectOfType<ToyElevatorController>();
            _toyElevatorController.EndGameMode();
        }
    }
}
