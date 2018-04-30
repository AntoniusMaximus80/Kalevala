using System;
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

        //Added here since other UI things are here and because it does not deserve a file of its own at this point.
        [SerializeField]
        private GameObject _focusShow;
        private static GameObject _staticFocusShow;

        // DEBUGGING
        [SerializeField]
        private ScreenStateType _screen = ScreenStateType.None;
        [SerializeField]
        private GameModeStateType _gameMode = GameModeStateType.None;

        private IList<ScreenStateBase> _screenStates =
            new List<ScreenStateBase>();
        private IList<GameModeStateBase> _gameModeStates =
            new List<GameModeStateBase>();

        public ScreenStateBase CurrentScreenState { get; set; }
        public GameModeStateBase CurrentGameModeState { get; set; }

        /// <summary>
        /// An event which is fired when the game ends.
        /// </summary>
        public event Action<bool> GameOver;

        /// <summary>
        /// An event which is fired when the pause menu is activated.
        /// </summary>
        public event Action PauseMenuActivated;

        /// <summary>
        /// An event which is fired when the pause menu is deactivated.
        /// </summary>
        public event Action PauseMenuDeactivated;

        private void Awake()
        {
            // Store the focus canvas statically.
            _staticFocusShow = _focusShow;
        }

        public void Init()
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
            GameModeState_Sampo sampo = new GameModeState_Sampo(this);
            _gameModeStates.Add(normal);
            _gameModeStates.Add(sampo);

            SetState(CurrentGameModeState, normal);
        }

        public void ShowCurrentMenu(bool show)
        {
            CurrentScreenState.ShowScreenObject(show);
        }

        public void StartNewGame()
        {
            GameManager.Instance.ResetAll();
            PerformTransition(ScreenStateType.Play);
        }

        public void ReturnToMainMenu()
        {
            EndGame(false);
            //GameManager.Instance.ResetAll();
            PerformTransition(ScreenStateType.MainMenu);
            HideEventCam();
        }

        /// <summary>
        /// Ends the game. If the player lost their last ball, the game ends
        /// normally and their score is saved. The score is not saved if the
        /// player drops out by returning to the main menu.
        /// </summary>
        /// <param name="saveScore">is the score saved</param>
        public void EndGame(bool saveScore)
        {
            if (CurrentGameModeState.State != GameModeStateType.Normal)
            {
                PerformTransition(GameModeStateType.Normal);
            }

            PinballManager.Instance.SetPinballPhysicsEnabled(false);
            GameOver(saveScore);
        }

        public bool PerformTransition(ScreenStateType targetState)
        {
            //Debug.Log("Next screen: " + targetState);

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
                // The pause state if it is currently active
                ScreenState_Pause pauseState =
                    CurrentScreenState as ScreenState_Pause;

                // Time continues if the pause state is exited
                // to something else than settings menu state
                if (pauseState != null &&
                    targetState != ScreenStateType.SettingsMenu)
                {
                    pauseState.ResumeGame();
                }

                SetState(CurrentScreenState, state);
                result = true;
            }
            else
            {
                Debug.LogError(targetState + " screen object is missing.");
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
                Debug.LogError(targetState + " mode object is missing.");
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
            //return _screenStates.FirstOrDefault
            //    (state => state.State == stateType);
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

        public static void ShowEventCam()
        {
            if (GameManager.Instance.Screen.State == ScreenStateType.Play &&
                Settings.Instance.EnableEventCamera)
            {
                _staticFocusShow.SetActive(true);
            }
        }

        public static void HideEventCam()
        {
            _staticFocusShow.SetActive(false);
        }

        /// <summary>
        /// Fires the PauseMenuActivated event.
        /// </summary>
        public void ActivatePauseMenu()
        {
            PauseMenuActivated();
        }

        /// <summary>
        /// Fires the PauseMenuDeactivated event.
        /// </summary>
        public void DeactivatePauseMenu()
        {
            PauseMenuDeactivated();
        }

        /// <summary>
        /// Checks whether the game is paused or not.
        /// Can check if the game is either in the
        /// pause menu only or in any menu.
        /// </summary>
        /// <param name="anyMenu">Is any menu accepted</param>
        /// <returns>Is the game paused</returns>
        public bool GameIsPaused(bool anyMenu)
        {
            // Pause menu is checked in any case
            bool result = (CurrentScreenState.State == ScreenStateType.Pause);

            // Other menus are checked only if anyMenu is true
            if (!result && anyMenu)
            {
                result = (CurrentScreenState.State == ScreenStateType.MainMenu ||
                          CurrentScreenState.State == ScreenStateType.SettingsMenu);
            }

            return result;
        }
    }
}
