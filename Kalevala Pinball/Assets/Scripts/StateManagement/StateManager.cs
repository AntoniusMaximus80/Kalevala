using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class StateManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject mainMenu;

        [SerializeField]
        private GameObject pauseMenu;

        [SerializeField]
        private GameObject settingsMenu;

        // DEBUGGING
        [SerializeField]
        private ScreenStateType screen = ScreenStateType.None;
        [SerializeField]
        private GameModeStateType gameMode = GameModeStateType.None;

        private IList<ScreenStateBase> _screenStates = new List<ScreenStateBase>();
        private IList<GameModeStateBase> _gameModeStates = new List<GameModeStateBase>();

        public ScreenStateBase CurrentScreenState { get; set; }
        public GameModeStateBase CurrentGameModeState { get; set; }

        private void Start()
        {
            InitScreens();
            InitGameModes();
        }

        private void InitScreens()
        {
            ScreenState_MainMenu mainMenu = new ScreenState_MainMenu(this);
            ScreenState_Play play = new ScreenState_Play(this);
            ScreenState_Pause pause = new ScreenState_Pause(this);
            _screenStates.Add(mainMenu);
            _screenStates.Add(play);
            _screenStates.Add(pause);

            SetScreen(mainMenu);
        }

        private void InitGameModes()
        {
            GameModeState_Normal normal = new GameModeState_Normal(this);
            _gameModeStates.Add(normal);

            SetGameMode(normal);
        }

        private void SetScreen(ScreenStateBase state)
        {
            ShowOrHideMenu(state.State);

            CurrentScreenState = state;
            CurrentScreenState.Activate();
            screen = CurrentScreenState.State;
        }

        private void SetGameMode(GameModeStateBase state)
        {
            CurrentGameModeState = state;
            CurrentGameModeState.Activate();
            gameMode = CurrentGameModeState.State;
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
                CurrentScreenState.StartDeactivating();
                SetScreen(state);
                result = true;
                //Debug.Log("Changed screen to " + state);
            }
            else
            {
                Debug.LogError("An object of " + targetState + " is missing.");
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
                CurrentGameModeState.StartDeactivating();
                SetGameMode(state);
                result = true;
                //Debug.Log("Changed game mode to " + state);
            }
            else
            {
                Debug.LogError("An object of " + targetState + " is missing.");
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
            // Returns the first object from the state list whose State property's
            // value equals to stateType. If no object was found, null is returned.

            foreach (GameModeStateBase state in _gameModeStates)
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

        public void GoToPlayState()
        {
            PerformTransition(ScreenStateType.Play);
        }

        public void GoToMainMenuState()
        {
            PerformTransition(ScreenStateType.MainMenu);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        private void ShowOrHideMenu(ScreenStateType enteredState)
        {
            switch (enteredState)
            {
                case ScreenStateType.Play:
                {
                    if (mainMenu != null && mainMenu.activeSelf)
                    {
                        mainMenu.SetActive(false);
                    }
                    else if (pauseMenu != null && pauseMenu.activeSelf)
                    {
                        pauseMenu.SetActive(false);
                    }
                    break;
                }
                case ScreenStateType.MainMenu:
                {
                    if (mainMenu != null)
                    {
                        mainMenu.SetActive(true);
                    }
                    else if (pauseMenu != null && pauseMenu.activeSelf)
                    {
                        pauseMenu.SetActive(false);
                    }
                    break;
                }
                case ScreenStateType.Pause:
                {
                    if (pauseMenu != null)
                    {
                        pauseMenu.SetActive(true);
                    }
                    break;
                }
            }
        }
    }
}
