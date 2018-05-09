using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class ToyElevatorController : MonoBehaviour
    {
        public enum ToyElevatorState
        {
            Start,
            Moving,
            Idle,
            End
        }

        public ToyElevatorState _currentToyElevatorState;

        [SerializeField]
        private Animator _toyElevatorAnimator,
            _irisDoorAnimator;

        public GameObject _sampo;

        private GameModeStateType _currentGameModeState;

        //public bool _debugSampoModeStart = false;

        private void Start()
        {
            _currentGameModeState = GameModeStateType.Normal;
            _currentToyElevatorState = ToyElevatorState.Start;
        }

        // Update is called once per frame
        void Update()
        {
            //if (_debugSampoModeStart)
            //{
            //    StartGameMode(GameModeStateType.Sampo);
            //} else
            //{
            //    DeactivateToy();
            //}

            if (_currentToyElevatorState == ToyElevatorState.End)
            {
                Debug.Log("EndGameMode 1");
                EndGameMode();
            }
        }

        /// <summary>
        /// This method opens the iris door and raises the elevator to the surface.
        /// </summary>
        public void RaiseElevator()
        {
            _currentToyElevatorState = ToyElevatorState.Moving;
            _irisDoorAnimator.SetBool("Open", true);
            _toyElevatorAnimator.SetBool("Rise", true);
        }

        public void ActivateToy()
        {
            Debug.Log("Activate toy");

            _currentToyElevatorState = ToyElevatorState.Idle;
            if (_currentGameModeState == GameModeStateType.Sampo)
            {
                Debug.Log("toy stands");
                _sampo.GetComponent<Animator>().SetBool("Stand", true);
            }
        }

        public void LowerElevator()
        {
            _currentToyElevatorState = ToyElevatorState.Moving;
            _irisDoorAnimator.SetBool("Open", false);
            _toyElevatorAnimator.SetBool("Rise", false);
        }

        /// <summary>
        /// This method instructs the ToyElevatorController to start a specific game mode.
        /// </summary>
        /// <param name="gameModeStateType">The type of GameModeStateType that is to be started.</param>
        public void StartGameMode(GameModeStateType gameModeStateType)
        {
            if (gameModeStateType == GameModeStateType.Sampo)
            {
                _currentGameModeState = GameModeStateType.Sampo;
                _sampo.SetActive(true);
                RaiseElevator();
            }
        }

        public void EndGameMode()
        {
            _sampo.GetComponent<Sampo>().EndSampoMode();
            DeactivateToy();
            LowerElevator();
        }

        public void DeactivateToy()
        {
            if (_currentGameModeState == GameModeStateType.Sampo)
            {
                _sampo.GetComponent<Animator>().SetBool("Stand", false);
            }
        }
    }
}