using UnityEngine;

namespace Kalevala {
    public class InputManager: MonoBehaviour {

        private const string _launch = "Launch";
        private const string _leftFlipperHit = "LeftFlipper";
        private const string _rightFlipperHit = "RightFlipper";
       
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

        private StateManager stateManager;
        private ConfirmationDialog confirmation;
        private MouseCursorController cursor;

        public static Vector3 NudgeVector
        {
            get
            {
                return _nudgeVector;
            }
        }

        private void Start()
        {
            stateManager = FindObjectOfType<StateManager>();
            if (stateManager == null)
            {
                Debug.LogError("StateManager object not found in the scene.");
            }

            confirmation = FindObjectOfType<ConfirmationDialog>();
            cursor = FindObjectOfType<MouseCursorController>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (confirmation.Active)
            {
                ConfirmationInput();
            }
            else
            {
                switch (stateManager.CurrentScreenState.State)
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
            // Debugging
            if (GameManager.Instance.debug_SkipMainMenu)
            {
                StartGame(true);
                return;
            }

            // Quitting the game
            if (Input.GetButtonUp("Cancel"))
            {
                QuitGame(false);
            }
        }

        private void PauseInput()
        {
            // Resuming the game
            if (Input.GetButtonUp("Cancel"))
            {
                stateManager.GoToPlayState();
                //Debug.Log("Game resumed");
            }
        }

        private void SettingsInput()
        {
            // Exiting the menu
            if (Input.GetButtonUp("Cancel"))
            {
                SaveSettings(false);
            }
        }

        private void GameOverInput()
        {
            // Returning to main menu
            if (Input.GetButtonUp("Cancel"))
            {
                ReturnToMainMenu(false);
            }
        }

        private void ConfirmationInput()
        {
            ConfirmationDialog.InputType confInput = confirmation.GetInput();

            // Decline just closes the confirmation dialog box
            // except in the case of settings menu where it
            // exits the menu without saving.
            // AltDecline has the normal effect for settings menu.
            if (confInput == ConfirmationDialog.InputType.Decline)
            {
                switch (confirmation.Type)
                {
                    case ConfirmationType.SaveSettings:
                    {
                        stateManager.GoToPauseState();
                        break;
                    }
                }
            }
            else if (confInput == ConfirmationDialog.InputType.Accept)
            {
                switch (confirmation.Type)
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

        private void GameInput()
        {
            if (Input.GetAxis(_launch) > 0)
            {
                _launcher.PoweringUp();
            }
            else if (Input.GetButtonUp(_launch))
            {
                _launcher.SwingAxe();
            }

            if (Input.GetButton(_leftFlipperHit))
            {
                _leftFlipper.UseMotor();
            }
            else
            {
                _leftFlipper.UseSpring();
            }

            if (Input.GetButton(_rightFlipperHit))
            {
                _rightFlipper.UseMotor();
                _topRightFlipper.UseMotor();
            }
            else
            {
                _rightFlipper.UseSpring();
                _topRightFlipper.UseSpring();
            }

            // Ugly nudge hack.
            _nudgeVector.x = 0;
            if (Input.GetButtonDown("NudgeLeft")) DoNudge(-1);
            if (Input.GetButtonDown("NudgeRight")) DoNudge(1);

            // Pausing the game
            if (Input.GetButtonUp("Cancel"))
            {
                stateManager.PerformTransition(ScreenStateType.Pause);
                //Debug.Log("Game paused");
            }

            // Resetting the ball
            if (Input.GetButtonDown("ResetBall"))
            {
                PinballManager.Instance.Pinballs[0].ResetBall();
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
            if (Input.GetButtonUp("ToggleCursor"))
            {
                cursor.PlayingUsingMouse = !cursor.PlayingUsingMouse;
            }
        }

        private void Confirm(ConfirmationType confType)
        {
            confirmation.Activate(confType);
            stateManager.ShowCurrentMenu(false);
        }

        private void ExitConfirm()
        {
            confirmation.Deactivate();
            stateManager.ShowCurrentMenu(true);
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
                stateManager.StartNewGame();
            }
        }

        public void ReturnToMainMenu(bool skipConfirm)
        {
            if (!skipConfirm)
            {
                Confirm(ConfirmationType.ReturnToMainMenu);
            }
            else
            {
                stateManager.GoToMainMenuState();
            }
        }

        public void SaveSettings(bool skipConfirm)
        {
            if (!skipConfirm)
            {
                // TODO: Accept, decline and cancel buttons

                Confirm(ConfirmationType.SaveSettings);
            }
            else
            {
                // TODO: Save settings

                stateManager.GoToPauseState();
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
    }
}
