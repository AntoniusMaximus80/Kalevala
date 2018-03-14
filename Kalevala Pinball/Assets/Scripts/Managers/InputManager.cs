﻿using UnityEngine;

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
        }

        // Update is called once per frame
        private void Update()
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
                case ScreenStateType.GameOver:
                {
                    GameOverInput();
                    break;
                }
            }
        }

        private void MainMenuInput()
        {
            // Starting the game
            if (Input.GetButtonUp("Submit"))
            {
                stateManager.GoToPlayState();
                Debug.Log("Game started");
            }
        }

        private void PauseInput()
        {
            // Resuming the game
            if (Input.GetButtonUp("Cancel") ||
                Input.GetButtonUp("Submit"))
            {
                stateManager.GoToPlayState();
                //Debug.Log("Game resumed");
            }
        }

        private void GameOverInput()
        {
            // Restarting the game
            if (Input.GetButtonUp("Submit"))
            {
                stateManager.GoToPlayState();
                //Debug.Log("Game resumed");
            }

            // Returning to main menu
            if (Input.GetButtonUp("Cancel"))
            {
                stateManager.GoToMainMenuState();
                //Debug.Log("Game resumed");
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
    }
}
