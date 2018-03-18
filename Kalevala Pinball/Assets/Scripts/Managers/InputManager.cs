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

        //private bool _gameOver;

        /// <summary>
        /// Is the submit button still held down from the previous screen
        /// </summary>
        private bool submitHoldover;

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
            _highscoreList = FindObjectOfType<HighscoreList>();
            _cursor = FindObjectOfType<MouseCursorController>();

            _highscoreList.Visible = false;

            // When the GameOver event is triggered, the first
            // button in the game over menu is highlighted
            _stateManager.GameOver += HighlightGameOverMenuButton;
        }

        /// <summary>
        ///  Update is called once per frame.
        /// </summary>
        private void Update()
        {
            UpdateMouseControl();
            //CheckIfGameOver();

            if (_confirmation.Active)
            {
                ConfirmationInput();
            }
            else
            {
                // Toggling scoreboard visibility
                if (Input.GetButtonUp(_SHOW_SCORES))
                {
                    _highscoreList.Visible = !_highscoreList.Visible;
                }

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

        /// <summary>
        /// Highlights the first button in the game over
        /// menu if the mouse cursor is hidden.
        /// </summary>
        private void HighlightGameOverMenuButton(bool saveScore)
        {
            SetHighlightedButtonOnScreenChange();
        }

        private void MainMenuInput()
        {
            // Debugging:
            // Goes straight to the game
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
                SetHighlightedButtonOnScreenChange();
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
            if (Input.GetAxis(_LEFTFLIPPERHIT) != 0)
            {
                _leftFlipper.UseMotor();
            }
            else
            {
                _leftFlipper.UseSpring();
            }

            if (Input.GetAxis(_RIGHTFLIPPERHIT) != 0)
            {
                _rightFlipper.UseMotor();
                _topRightFlipper.UseMotor();
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

            // Toggling cursor visibility
            //if (Input.GetButtonUp("ToggleCursor"))
            //{
            //    cursor.PlayingUsingMouse = !cursor.PlayingUsingMouse;
            //}
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
                        _stateManager.GoToPauseState();
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
            SetHighlightedButtonOnScreenChange();
        }

        private void ExitConfirm()
        {
            _confirmation.Deactivate();
            _stateManager.ShowCurrentMenu(true);
            SetHighlightedButtonOnScreenChange();
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
            _stateManager.GoToPlayState();
        }

        public void ReturnToMainMenu(bool skipConfirm)
        {
            if (!skipConfirm)
            {
                Confirm(ConfirmationType.ReturnToMainMenu);
            }
            else
            {
                _stateManager.GoToMainMenuState();
                SetHighlightedButtonOnScreenChange();
            }
        }

        public void GoToSettings()
        {
            _stateManager.GoToSettingsMenuState();
            SetHighlightedButtonOnScreenChange();
        }

        public void SaveSettings(bool skipConfirm)
        {
            if (!skipConfirm)
            {
                Confirm(ConfirmationType.SaveSettings);
            }
            else
            {
                // TODO: Save settings

                _stateManager.GoToPauseState();
                SetHighlightedButtonOnScreenChange();
            }
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

        private void UpdateMouseControl()
        {
            if (_cursor.PlayingUsingMouse)
            {
                // Disables the use of the mouse cursor
                if (Input.GetAxis(_HOR_AXIS) != 0 ||
                    Input.GetAxis(_VERT_AXIS) != 0)
                {
                    _cursor.PlayingUsingMouse = false;
                    if (_canvasGR != null)
                    {
                        _canvasGR.enabled = false;
                    }
                }
                // Enables the use of the mouse cursor
                // (the cursor handles most of this by itself)
                else if (_canvasGR != null && !_canvasGR.enabled)
                {
                    _canvasGR.enabled = true;
                }
            }
        }

        private void SetHighlightedButtonOnScreenChange()
        {
            if (!_cursor.PlayingUsingMouse)
            {
                //cursor.ClearCursorHighlight();

                // Selects the first available button
                Button firstButton = FindObjectOfType<Button>();
                if (firstButton != null)
                {
                    Utils.SelectButton(firstButton);
                }

                //EventSystem.current.SetSelectedGameObject
                //    (EventSystem.current.firstSelectedGameObject);
            }
        }
    }
}
