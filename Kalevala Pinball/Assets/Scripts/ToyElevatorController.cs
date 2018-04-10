using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class ToyElevatorController : MonoBehaviour
    {
        [SerializeField]
        private Animator _toyElevatorAnimator,
            _irisDoorAnimator;

        public GameObject _sampo;

        private GameModeStateType _currentGameModeState;

        public bool _debugSampoModeStart = false;

        private void Start()
        {
            _currentGameModeState = GameModeStateType.Normal;
        }

        // Update is called once per frame
        void Update()
        {
            if (_debugSampoModeStart)
            {
                StartGameMode(GameModeStateType.Sampo);
            } else
            {
                DeactivateToy();
            }
        }

        public void RaiseElevator()
        {
            _toyElevatorAnimator.SetBool("Rise", true);
            _irisDoorAnimator.SetBool("Open", true);
        }

        public void LowerElevator()
        {
            _toyElevatorAnimator.SetBool("Rise", false);
            _irisDoorAnimator.SetBool("Open", false);
        }

        /// <summary>
        /// This method instructs the ToyElevatorController to start a specific game mode.
        /// </summary>
        /// <param name="gameModeStateType">The type of GameModeStateType that is to be started.</param>
        public void StartGameMode(GameModeStateType gameModeStateType)
        {
            if (gameModeStateType == GameModeStateType.Sampo)
            {
                _sampo.SetActive(true);
                _currentGameModeState = GameModeStateType.Sampo;
                RaiseElevator();
            }
        }

        public void EndGameMode()
        {
            LowerElevator();
        }

        public void ActivateToy()
        {
            if (_currentGameModeState == GameModeStateType.Sampo)
            {
                _sampo.GetComponent<Animator>().SetBool("Stand", true);
            }
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