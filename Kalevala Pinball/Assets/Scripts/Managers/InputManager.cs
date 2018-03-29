﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Kalevala {
    public class InputManager: MonoBehaviour {

        private const string _LAUNCH = "Launch";
        private const string _LEFTFLIPPERHIT = "LeftFlipper";
        private const string _RIGHTFLIPPERHIT = "RightFlipper";
        private const string _HOR_AXIS = "Horizontal";
        private const string _VERT_AXIS = "Vertical";
        private const string _SUBMIT = "Submit";
        private const string _CANCEL = "Cancel";
        private const string _PAUSE = "Pause";
        private const string _SHOW_SCORES = "ShowHighscores";

        [SerializeField]
        private GraphicRaycaster _canvasGR;

        [SerializeField]
        private Launcher _launcher;

        [SerializeField]
        private FlipperBar _leftFlipper;

        [SerializeField]
        private FlipperBar _rightFlipper;

        [SerializeField]
        private FlipperBar _topRightFlipper;

        [SerializeField]
        private float _nudgeStrength;

        private static Vector3 _nudgeVector = Vector3.zero;

        private StateManager _stateManager;
        private ConfirmationDialog _confirmation;
        private HighscoreList _highscoreList;
        private MouseCursorController _cursor;
        private HeatMap _heatMap;

        private bool _alwaysDisplayScoreboard;
        private bool _pauseMenuActive;
        //private bool _gameOver;

        /// <summary>
        /// Is the submit button still held down from the previous screen
        /// </summary>
        private bool submitHoldover;

        /// <summary>
        /// The screen type to which should be returned
        /// after exiting the settings menu
        /// </summary>
        private ScreenStateType settingsAccessorScreen;

        public static Vector3 NudgeVector
        {
            get
            {
                return _nudgeVector;
            }
        }

        private void Start()
        {
            _stateManager = FindObjectOfType<StateManager>();
            if (_stateManager == null)
            {
                Debug.LogError("StateManager object not found in the scene.");
            }

            _confirmation = GetComponentInChildren<ConfirmationDialog>();
            _highscoreList = GameManager.Instance.HighscoreList;
            _cursor = FindObjectOfType<MouseCursorController>();
            _heatMap = FindObjectOfType<HeatMap>();

            // Allows the cursor to select the current menu's
            // default selected button when it is hidden
            _cursor.SelectMenuButtonAction = SelectDefaultSelectedMenuButton;

            // Hides the scoreboard by default
            _highscoreList.Visible = false;

            // Registers to listen to the GameOver event.
            // When the GameOver event is triggered, the first
            // button in the game over menu is highlighted
            _stateManager.GameOver += HighlightGameOverMenuButton;

            // Registers to listen to the PauseMenuActivated event.
            // Makes the scoreboard visible and doesn't allow
            // hiding it in the pause menu
            _stateManager.PauseMenuActivated += PauseMenuEntered;

            // Registers to listen to the PauseMenuDeactivated event.
            // Changes the scoreboard's visibility to what it
            // was before accessing the pause menu
            _stateManager.PauseMenuDeactivated += PauseMenuExited;
        }

        private void OnDestroy()
        {
            _stateManager.GameOver -= HighlightGameOverMenuButton;
            _stateManager.PauseMenuActivated -= PauseMenuEntered;
            _stateManager.PauseMenuDeactivated -= PauseMenuExited;
        }

        /// <summary>
        ///  Update is called once per frame.
        /// </summary>
        private void Update()
        {
            UpdateMouseControl();

            ScoreboardInput();

            if (_confirmation.Active)
            {
                ConfirmationInput();
            }
            else
            {
                switch (_stateManager.CurrentScreenState.State)
                {
                    case ScreenStateType.MainMenu:
                    {
                        MainMenuInput();
                        break;
                    }
                    case ScreenStateType.Play:
                    {
                        GameInput();
                        break;
                    }
                    case ScreenStateType.Pause:
                    {
                        PauseInput();
                        break;
                    }
                    case ScreenStateType.SettingsMenu:
                    {
                        SettingsInput();
                        break;
                    }
                    case ScreenStateType.GameOver:
                    {
                        GameOverInput();
                        break;
                    }
                }
            }

            DebugInput();
        }

        private void MainMenuInput()
        {
            // Debugging:
            // Goes straight to play mode
            if (GameManager.Instance.debug_SkipMainMenu)
            {
                StartGame(true);
                return;
            }

            // Quitting the game
            if (Input.GetButtonUp(_CANCEL))
            {
                QuitGame(false);
            }
        }

        private void PauseInput()
        {
            // Resuming the game
            if (Input.GetButtonUp(_CANCEL) ||
                Input.GetButtonUp(_PAUSE))
            {
                ResumeGame();
                //Debug.Log("Game resumed");
            }
        }

        private void SettingsInput()
        {
            // Exiting the menu
            if (Input.GetButtonUp(_CANCEL) ||
                Input.GetButtonUp(_PAUSE))
            {
                SaveSettings(false);
            }

            // Debugging
            // TODO: UI for these settings

            // Volume control
            if (Input.GetKey(KeyCode.UpArrow))
            {
                GameManager.Instance.MusicVolume =
                    Mathf.Clamp(GameManager.Instance.MusicVolume + 0.01f, 0, 1);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                GameManager.Instance.MusicVolume =
                    Mathf.Clamp(GameManager.Instance.MusicVolume - 0.01f, 0, 1);
            }

            // Changing the camera mode
            if (Input.GetKeyUp(KeyCode.F))
            {
                GameManager.Instance.SetCameraMode
                    (CameraController.CameraType.Default);
            }
            if (Input.GetKeyUp(KeyCode.G))
            {
                GameManager.Instance.SetCameraMode
                    (CameraController.CameraType.Horizontal);
            }

            // Reset highscores
            if (Input.GetKeyUp(KeyCode.R))
            {
                GameManager.Instance.EraseLocalHighscores();
            }
        }

        private void GameOverInput()
        {
            // Returning to the main menu
            if (Input.GetButtonUp(_CANCEL) ||
                Input.GetButtonUp(_PAUSE))
            {
                ReturnToMainMenu(false);
            }
        }

        private void GameInput()
        {
            // Accepts both KB+M and gamepad input

            LaunchInput();
            FlipperInput();

            // Ugly nudge hack.
            _nudgeVector.x = 0;
            if (Input.GetButtonDown("NudgeLeft")) DoNudge(-1);
            if (Input.GetButtonDown("NudgeRight")) DoNudge(1);

            // Pausing the game
            if (Input.GetButtonUp(_CANCEL) ||
                Input.GetButtonUp(_PAUSE))
            {
                _stateManager.PerformTransition(ScreenStateType.Pause);
                HighlightMenuDefaultButton();
                //Debug.Log("Game paused");
            }
        }

        private void LaunchInput()
        {
            // The launcher cannot be used until the submit button is released
            if (!submitHoldover)
            {
                if (Input.GetAxis(_LAUNCH) != 0)
                {
                    _launcher.PoweringUp();
                }
                else if (Input.GetButtonUp(_LAUNCH))
                {
                    _launcher.SwingAxe();
                }
            }
            else
            {
                CheckIfSubmitReleased();
            }
        }

        private void FlipperInput()
        {
            // If the game is in tilt, the flippers are unavailable
            if (PinballManager.Instance.Tilt)
            {
                return;
            }

            if (Input.GetAxis(_LEFTFLIPPERHIT) != 0)
            {
                _leftFlipper.UseMotor();

                if (_leftFlipper.IsReset)
                {
                    Rollover.RollLeft();
                }
            }
            else
            {
                _leftFlipper.UseSpring();
            }

            if (Input.GetAxis(_RIGHTFLIPPERHIT) != 0)
            {
                _rightFlipper.UseMotor();
                _topRightFlipper.UseMotor();

                if (_rightFlipper.IsReset)
                {
                    Rollover.RollRight();
                }
            }
            else
            {
                _rightFlipper.UseSpring();
                _topRightFlipper.UseSpring();
            }

        }

        private void DoNudge(int direction)
        {
            // TODO : This really needs a sound effect, maybe the camera shake Toni suggested?
            // Not sure as nudge is not really THAT powerful.
            _nudgeVector.x = direction * _nudgeStrength;

            //PinballManager.Instance.SpendNudge();
        }

        private void DebugInput()
        {
            // Resetting the ball
            if (Input.GetButtonDown("ResetBall"))
            {
                PinballManager.Instance.Pinballs[0].ResetBall();
            }

            // Losing and resetting the ball
            if (Input.GetButtonDown("LoseBall"))
            {
                PinballManager.Instance.DebugLoseBall();
            }

            // Toggling heat map visibility
            if (Input.GetKeyDown(KeyCode.H))
            {
                _heatMap.Visible = !_heatMap.Visible;
            }

            // Moving the camera
            if (Input.GetKeyUp(KeyCode.V))
            {
                GameManager.Instance.SetCameraFocus
                    (CameraController.CameraPosition.Playfield);
            }
            if (Input.GetKeyUp(KeyCode.B))
            {
                GameManager.Instance.SetCameraFocus
                    (CameraController.CameraPosition.Kantele);
            }
            if (Input.GetKeyUp(KeyCode.N))
            {
                GameManager.Instance.SetCameraFocus
                    (CameraController.CameraPosition.Launcher);
            }
        }

        private void ScoreboardInput()
        {
            // Toggling scoreboard visibility
            if (!_pauseMenuActive && Input.GetButtonUp(_SHOW_SCORES))
            {
                _highscoreList.Visible = !_highscoreList.Visible;
                _alwaysDisplayScoreboard = _highscoreList.Visible;
            }
        }

        private void ConfirmationInput()
        {
            ConfirmationDialog.InputType confInput = _confirmation.GetInput();

            // Decline just closes the confirmation dialog box
            // except in the case of settings menu where it
            // exits the menu without saving.
            // AltDecline has the normal effect for settings menu.
            if (confInput == ConfirmationDialog.InputType.Decline)
            {
                switch (_confirmation.Type)
                {
                    case ConfirmationType.SaveSettings:
                    {
                        ExitSettings();
                        break;
                    }
                }
            }
            else if (confInput == ConfirmationDialog.InputType.Accept)
            {
                switch (_confirmation.Type)
                {
                    case ConfirmationType.QuitGame:
                    {
                        QuitGame(true);
                        break;
                    }
                    case ConfirmationType.StartGame:
                    {
                        StartGame(true);
                        break;
                    }
                    case ConfirmationType.ReturnToMainMenu:
                    {
                        ReturnToMainMenu(true);
                        break;
                    }
                    case ConfirmationType.SaveSettings:
                    {
                        SaveSettings(true);
                        break;
                    }
                    case ConfirmationType.EraseHighscores:
                    {
                        EraseHighscores(true);
                        break;
                    }
                }
            }

            if (confInput != ConfirmationDialog.InputType.NoInput)
            {
                ExitConfirm();
            }
        }

        private void Confirm(ConfirmationType confType)
        {
            _confirmation.Activate(confType);
            _stateManager.ShowCurrentMenu(false);
            HighlightMenuDefaultButton();
        }

        private void ExitConfirm()
        {
            _confirmation.Deactivate();
            _stateManager.ShowCurrentMenu(true);
            HighlightMenuDefaultButton();
        }

        public void QuitGame(bool skipConfirm)
        {
            if (!skipConfirm)
            {
                Confirm(ConfirmationType.QuitGame);
            }
            else
            {
                Application.Quit();
            }
        }

        public void StartGame(bool skipConfirm)
        {
            if (!skipConfirm)
            {
                Confirm(ConfirmationType.StartGame);
            }
            else
            {
                submitHoldover = true;
                _stateManager.StartNewGame();
            }
        }

        public void ResumeGame()
        {
            submitHoldover = true;
            _stateManager.PerformTransition(ScreenStateType.Play);
        }

        public void ReturnToMainMenu(bool skipConfirm)
        {
            if (!skipConfirm)
            {
                Confirm(ConfirmationType.ReturnToMainMenu);
            }
            else
            {
                _stateManager.ReturnToMainMenu();
                HighlightMenuDefaultButton();
            }
        }

        public void GoToSettings()
        {
            settingsAccessorScreen = _stateManager.CurrentScreenState.State;
            _stateManager.PerformTransition(ScreenStateType.SettingsMenu);
            HighlightMenuDefaultButton();
        }

        public void SaveSettings(bool skipConfirm)
        {
            if (!skipConfirm)
            {
                Confirm(ConfirmationType.SaveSettings);
            }
            else
            {
                GameManager.Instance.SaveSettings();
                ExitSettings();
            }
        }

        private void ExitSettings()
        {
            _stateManager.PerformTransition(settingsAccessorScreen);
            //_stateManager.GoToPauseState();
            HighlightMenuDefaultButton();
        }

        public void EraseHighscores(bool skipConfirm)
        {
            if (!skipConfirm)
            {
                Confirm(ConfirmationType.EraseHighscores);
            }
            else
            {
                // TODO
            }
        }

        private void CheckIfSubmitReleased()
        {
            if (submitHoldover && !Input.GetButton(_SUBMIT))
            {
                submitHoldover = false;
            }
        }

        /// <summary>
        /// Hides the mouse cursor if any input methods
        /// other than the mouse are used.
        /// Enables the use of the cursor if it is moved.
        /// </summary>
        private void UpdateMouseControl()
        {
            if (_cursor.PlayingUsingMouse)
            {
                // Disables the use of the mouse cursor if
                // any input other than the mouse is used
                if (NonMouseInputUsed())
                {
                    DisableMouseCursor();
                }
                // Enables the use of the mouse cursor
                else
                {
                    EnableMouseCursor();
                }
            }
        }

        private bool NonMouseInputUsed()
        {
            bool directionalInputUsed =
                Input.GetAxisRaw(_HOR_AXIS) != 0 ||
                Input.GetAxisRaw(_VERT_AXIS) != 0;

            bool submitInputUsed =
                Input.GetButton(_SUBMIT);

            //bool cancelInputUsed =
            //    Input.GetButton(_CANCEL);

            return directionalInputUsed || submitInputUsed;
        }

        /// <summary>
        /// Disables the use of the mouse cursor.
        /// </summary>
        public void DisableMouseCursor()
        {
            _cursor.PlayingUsingMouse = false;
            if (_canvasGR != null)
            {
                _canvasGR.enabled = false;
            }
        }

        /// <summary>
        /// Enables the use of the mouse cursor
        /// (the cursor handles most of this by itself).
        /// </summary>
        public void EnableMouseCursor()
        {
            if (_canvasGR != null && !_canvasGR.enabled)
            {
                _canvasGR.enabled = true;
            }
        }

        private void HighlightMenuDefaultButton()
        {
            if (!_cursor.PlayingUsingMouse)
            {
                //cursor.ClearCursorHighlight();

                // Selects the menu's default selected button
                SelectDefaultSelectedMenuButton();

                //EventSystem.current.SetSelectedGameObject
                //    (EventSystem.current.firstSelectedGameObject);
            }
        }

        /// <summary>
        /// Selects the given menu's default selected button.
        /// </summary>
        /// <param name="menu">A menu</param>
        private void SelectDefaultSelectedMenuButton()
        {
            // Gets the current menu, if any
            IMenu menu = null;
            if (_confirmation.Active)
            {
                menu = _confirmation as IMenu;
            }
            else
            {
                menu = _stateManager.CurrentScreenState as IMenu;
            }

            if (menu != null)
            {
                Button defaultSelectedButton =
                    menu.GetDefaultSelectedButton();
                if (defaultSelectedButton != null)
                {
                    Utils.SelectButton(defaultSelectedButton);
                }
            }
        }

        /// <summary>
        /// Highlights the first button in the game over
        /// menu if the mouse cursor is hidden.
        /// </summary>
        private void HighlightGameOverMenuButton(bool saveScore)
        {
            HighlightMenuDefaultButton();
        }

        /// <summary>
        /// Makes the scoreboard visible.
        /// </summary>
        private void PauseMenuEntered()
        {
            _pauseMenuActive = true;
            _highscoreList.Visible = true;
        }

        /// <summary>
        /// Changes the scoreboard's visibility to what
        /// it was before accessing the pause menu.
        /// </summary>
        private void PauseMenuExited()
        {
            _pauseMenuActive = false;
            _highscoreList.Visible = _alwaysDisplayScoreboard;
        }
    }
}
