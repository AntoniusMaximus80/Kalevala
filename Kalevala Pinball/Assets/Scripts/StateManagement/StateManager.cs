using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class StateManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _mainMenu;

        [SerializeField]
        private GameObject _pauseMenu;

        [SerializeField]
        private GameObject _settingsMenu;

        [SerializeField]
        private GameObject _gameOverScreen;

        // DEBUGGING
        [SerializeField]
        private ScreenStateType _screen = ScreenStateType.None;
        [SerializeField]
        private GameModeStateType _gameMode = GameModeStateType.None;

        private IList<ScreenStateBase> _screenStates = new List<ScreenStateBase>();
        private IList<GameModeStateBase> _gameModeStates = new List<GameModeStateBase>();

        public ScreenStateBase CurrentScreenState { get; set; }
        public GameModeStateBase CurrentGameModeState { get; set; }

        private void Start()
        {
            InitScreens();
            InitGameModes();
        }

        private void Update()
        {
            if (CurrentScreenState != null)
            {
                CurrentScreenState.Update();
            }

            if (CurrentGameModeState != null)
            {
                CurrentGameModeState.Update();
            }
        }

        private void InitScreens()
        {
            ScreenState_MainMenu mainMenuScreen =
                new ScreenState_MainMenu(this, _mainMenu);
            ScreenState_Play playScreen =
                new ScreenState_Play(this);
            ScreenState_Pause pauseScreen =
                new ScreenState_Pause(this, _pauseMenu);
            ScreenState_Settings settingsScreen =
                new ScreenState_Settings(this, _settingsMenu);
            ScreenState_GameOver gameOverScreen =
                new ScreenState_GameOver(this, _gameOverScreen);
            _screenStates.Add(mainMenuScreen);
            _screenStates.Add(playScreen);
            _screenStates.Add(pauseScreen);
            _screenStates.Add(settingsScreen);
            _screenStates.Add(gameOverScreen);

            SetState(CurrentScreenState, mainMenuScreen);
        }

        private void InitGameModes()
        {
            GameModeState_Normal normal = new GameModeState_Normal(this);
            _gameModeStates.Add(normal);

            SetState(CurrentGameModeState, normal);
        }

        public void GoToMainMenuState()
        {
            GameOver(false);
            GameManager.Instance.ResetAll();
            PerformTransition(ScreenStateType.MainMenu);
        }

        public void GoToPlayState()
        {
            PerformTransition(ScreenStateType.Play);
        }

        public void GoToPauseState()
        {
            PerformTransition(ScreenStateType.Pause);
        }

        public void GoToSettingsMenuState()
        {
            PerformTransition(ScreenStateType.SettingsMenu);
        }

        public void StartNewGame()
        {
            GameManager.Instance.ResetAll();
            //PinballManager.Instance.SetPinballPhysicsEnabled(true);
            GoToPlayState();
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        /// <summary>
        /// Ends the game. If the player lost their last ball, the game ends
        /// normally and their score is saved. The score is not saved if the
        /// player drops out by returning to the main menu.
        /// </summary>
        /// <param name="saveScore">is the score saved</param>
        private void GameOver(bool saveScore)
        {
            if (CurrentGameModeState.State != GameModeStateType.Normal)
            {
                PerformTransition(GameModeStateType.Normal);
            }

            PinballManager.Instance.SetPinballPhysicsEnabled(false);
            GameManager.Instance.GameOver(saveScore);
        }

        public bool PerformTransition(ScreenStateType targetState)
        {
            if (!CurrentScreenState.CheckTransition(targetState))
            {
                Debug.LogWarning(targetState +
                    " is currently not a valid state.");
                return false;
            }

            bool result = false;

            ScreenStateBase state = GetStateByType(targetState);
            if (state != null)
            {
                ScreenState_Pause pauseState =
                    CurrentScreenState as ScreenState_Pause;

                // Time continues if the pause state is exited
                // to something else than settings menu state
                if (pauseState != null &&
                    targetState != ScreenStateType.SettingsMenu)
                {
                    pauseState.ResumeGame();
                }

                // Game over state
                else if (targetState == ScreenStateType.GameOver)
                {
                    GameOver(true);
                }

                SetState(CurrentScreenState, state);
                result = true;
            }
            else
            {
                Debug.LogError(targetState + " object is missing.");
            }

            return result;
        }

        public bool PerformTransition(GameModeStateType targetState)
        {
            if (!CurrentGameModeState.CheckTransition(targetState))
            {
                Debug.LogWarning(targetState +
                    " is currently not a valid state.");
                return false;
            }

            bool result = false;

            GameModeStateBase state = GetStateByType(targetState);
            if (state != null)
            {
                SetState(CurrentGameModeState, state);
                result = true;
            }
            else
            {
                Debug.LogError(targetState + " object is missing.");
            }

            return result;
        }

        private ScreenStateBase GetStateByType(ScreenStateType stateType)
        {
            // Returns the first object from the state list whose State property's
            // value equals to stateType. If no object was found, null is returned.

            foreach (ScreenStateBase state in _screenStates)
            {
                if (state.State == stateType)
                {
                    return state;
                }
            }

            return null;

            // Does the same as all of the previous lines
            //return _screenStates.FirstOrDefault(state => state.State == stateType);
        }

        private GameModeStateBase GetStateByType(GameModeStateType stateType)
        {
            // Returns the first object from the state list whose State
            // property's value equals to stateType. If no object was found,
            // null is returned.

            foreach (GameModeStateBase state in _gameModeStates)
            {
                if (state.State == stateType)
                {
                    return state;
                }
            }

            return null;

            // Does the same as all of the previous lines
            //return _screenStates.FirstOrDefault
            //    (state => state.State == stateType);
        }

        private void SetState(StateBase currentState, StateBase newState)
        {
            ScreenStateBase screenState = newState
                as ScreenStateBase;
            GameModeStateBase gameModeState = newState
                as GameModeStateBase;

            if (currentState != null)
            {
                currentState.Deactivate();
            }

            if (screenState != null)
            {
                //CurrentScreenState.Deactivate();
                CurrentScreenState = screenState;
                CurrentScreenState.Activate();

                // Debugging
                _screen = CurrentScreenState.State;
            }
            else if (gameModeState != null)
            {
                //CurrentGameModeState.Deactivate();
                CurrentGameModeState = gameModeState;
                CurrentGameModeState.Activate();

                // Debugging
                _gameMode = CurrentGameModeState.State;
            }

            //Debug.Log("Changed from " + currentState + " to " + newState);
        }

        //private void SetScreen(ScreenStateBase state)
        //{
        //    //ShowOrHideMenu(state.State);

        //    CurrentScreenState = state;
        //    CurrentScreenState.Activate();
        //    _screen = CurrentScreenState.State;
        //}

        //private void SetGameMode(GameModeStateBase state)
        //{
        //    CurrentGameModeState = state;
        //    CurrentGameModeState.Activate();
        //    _gameMode = CurrentGameModeState.State;
        //}

        //private void ShowOrHideMenu(ScreenStateType enteredState)
        //{
        //    switch (enteredState)
        //    {
        //        case ScreenStateType.Play:
        //        {
        //            SetScreenActive(_mainMenu, false);
        //            SetScreenActive(_pauseMenu, false);
        //            //SetScreenActive(_settingsMenu, false);
        //            break;
        //        }
        //        case ScreenStateType.MainMenu:
        //        {
        //            SetScreenActive(_pauseMenu, false);
        //            SetScreenActive(_mainMenu, true);
        //            break;
        //        }
        //        case ScreenStateType.Pause:
        //        {
        //            SetScreenActive(_settingsMenu, false);
        //            SetScreenActive(_pauseMenu, true);
        //            break;
        //        }
        //        case ScreenStateType.SettingsMenu:
        //        {
        //            SetScreenActive(_pauseMenu, false);
        //            SetScreenActive(_settingsMenu, true);
        //            break;
        //        }
        //    }
        //}

        //private void SetScreenActive(GameObject screen, bool active)
        //{
        //    if (screen != null && screen.activeSelf != active)
        //    {
        //        screen.SetActive(active);
        //    }
        //}
    }
}
