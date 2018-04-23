using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Kalevala.Localization;
using L10n = Kalevala.Localization.Localization;

namespace Kalevala {
    public class InputManager: MonoBehaviour {

        private const string _LAUNCH = "Launch";
        private const string _LEFTFLIPPERHIT = "LeftFlipper";
        private const string _RIGHTFLIPPERHIT = "RightFlipper";
        private const string _HOR_AXIS = "Horizontal";
        private const string _VERT_AXIS = "Vertical";
        private const string _SUBMIT = "Submit";
        private const string _CANCEL = "Cancel";
        private const string _ERASE = "EraseCharacter";
        private const string _PAUSE = "Pause";
        private const string _SHOW_SCORES = "ShowHighscores";
        private const string _CONTROLLER_BUTTON_A = "joystick button 0";

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

        [SerializeField]
        private TextInput _playerNameTextInput;

        [SerializeField]
        private Text _playerName;

        private static Vector3 _nudgeVector = Vector3.zero;

        private EventSystem _eventSystem;
        private StateManager _stateManager;
        private ConfirmationDialog _confirmation;
        private HighscoreList _highscoreList;
        private MouseCursorController _cursor;
        private HeatMap _heatMap;
        private KanteleHeroPanel _kantelePanel;
        private CollectableSpawner _collSpawner;
        private bool _alwaysDisplayScoreboard;
        private bool _pauseMenuActive;
        private bool _textInputActive;
        private bool _KanteleHeroLeftTriggerDown;
        private bool _KanteleHeroRightTriggerDown;
        //private bool _gameOver;

        /// <summary>
        /// Is the submit button still held down from the previous screen
        /// </summary>
        private bool _submitHoldover;

        #region Settings

        /// <summary>
        /// The screen type to which should be returned
        /// after exiting the settings menu
        /// </summary>
        private ScreenStateType _settingsAccessorScreen;

        private LangCode _oldLangCode;
        private bool _oldEnableEventCam;

        #endregion Settings

        public static Vector3 NudgeVector
        {
            get
            {
                return _nudgeVector;
            }
        }

        private void Start()
        {
            _eventSystem = FindObjectOfType<EventSystem>();
            _stateManager = FindObjectOfType<StateManager>();
            if (_stateManager == null)
            {
                Debug.LogError("StateManager object not found in the scene.");
            }

            _confirmation = GetComponentInChildren<ConfirmationDialog>();
            _highscoreList = GameManager.Instance.HighscoreList;
            _cursor = FindObjectOfType<MouseCursorController>();
            _heatMap = FindObjectOfType<HeatMap>();
            _kantelePanel = FindObjectOfType<KanteleHeroPanel>();
            _collSpawner = FindObjectOfType<CollectableSpawner>();

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

            L10n.LanguageLoaded += OnLanguageLoaded;
            OnLanguageLoaded();
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

            // Deactivating text input when exiting the main menu
            if (_textInputActive &&
                _stateManager.CurrentScreenState.State != ScreenStateType.MainMenu)
            {
                DeactivateTextInput();
            }

            // Debug input uses various keys on the keyboard so
            // it cannot be active at the same time as text input 
            if (!_textInputActive)
            {
                DebugInput();
            }
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

            // Text input for the player's name
            if (_textInputActive)
            {
                TextInput();

                // Closing the text input
                if (_playerNameTextInput.CanBeClosed())
                {
                    if (Input.GetKeyDown(KeyCode.Return) ||
                        Input.GetKeyDown(_CONTROLLER_BUTTON_A) ||
                        Input.GetButtonUp(_CANCEL))
                    {
                        DeactivateTextInput();
                    }
                }
            }
            else
            {
                // Quitting the game
                if (Input.GetButtonUp(_CANCEL) ||
                    Input.GetButtonUp(_PAUSE))
                {
                    QuitGame(false);
                }
            }
        }

        private void TextInput()
        {
            _playerNameTextInput.CheckKeyboardInput();
            if (_playerNameTextInput.TextChanged)
            {
                SetPlayerName(_playerNameTextInput.GetText());
            }
        }

        private void SetPlayerName(string name)
        {
            GameManager.Instance.PlayerName = name;
            UpdatePlayerNameUIText();
        }

        private void UpdatePlayerNameUIText()
        {
            if (_playerName != null)
            {
                string startActiveMarker = "> ";
                string endActiveMarker = " <";

                string text;

                if (_textInputActive)
                {
                    // Uses markers around the text input field to
                    // make it clear that text input is active
                    text = startActiveMarker +
                           GameManager.Instance.PlayerName +
                           endActiveMarker;
                }
                else
                {
                    text = GameManager.Instance.PlayerName;
                }

                _playerName.text = text;
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
                Settings.Instance.MusicVolume =
                    Settings.Instance.MusicVolume + 0.01f;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                Settings.Instance.MusicVolume =
                    Settings.Instance.MusicVolume - 0.01f;
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
            if(Input.GetKeyDown(KeyCode.L))
            {
                _kantelePanel.ActivatePanel();
            }
            LaunchInput();
            if(_kantelePanel != null && _kantelePanel.PanelActive)
            {
                KantelePanelInput();
            }
            else
            {
                FlipperInput();
            }
            
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
            if (!_submitHoldover)
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

        private void KantelePanelInput()
        {
            if(Input.GetAxis(_LEFTFLIPPERHIT) > 0 && !_KanteleHeroLeftTriggerDown)
            {
                _kantelePanel.LeftTriggerPressed();
                _KanteleHeroLeftTriggerDown = true;
            } else if (Input.GetAxis(_LEFTFLIPPERHIT) == 0 && _KanteleHeroLeftTriggerDown)
            {
                _KanteleHeroLeftTriggerDown = false;
            }

            if(Input.GetAxis(_RIGHTFLIPPERHIT) > 0 && !_KanteleHeroRightTriggerDown)
            {
                _kantelePanel.RightTriggerPressed();
                _KanteleHeroRightTriggerDown = true;
            }
            else if (Input.GetAxis(_RIGHTFLIPPERHIT) == 0 && _KanteleHeroRightTriggerDown)
            {
                _KanteleHeroRightTriggerDown = false;
            }
        }

        private void DoNudge(int direction)
        {
            if (PinballManager.Instance.SpendNudge())
            {
                // TODO : This really needs a sound effect
                _nudgeVector.x = direction * _nudgeStrength;

                // Camera shake
                Vector3 shakeDir =
                    (direction < 0 ? Vector3.left : Vector3.right);
                GameManager.Instance.ShakeCamera(shakeDir, 0, 0.3f, 0.1f);
            }
        }

        private void DebugInput()
        {
            // Resetting the ball
            if (Input.GetButtonDown("ResetBall"))
            {
                PinballManager.Instance.DebugResetBall();
            }

            // Losing and resetting the ball
            if (Input.GetButtonDown("LoseBall"))
            {
                PinballManager.Instance.DebugLoseBall();
            }

            // Activating 'Shoot Again'
            if (Input.GetKeyDown(KeyCode.Y))
            {
                // One minute of Shoot Again
                PinballManager.Instance.ActivateShootAgain(60);
            }

            // Starting multiball mode
            if (Input.GetKeyDown(KeyCode.M))
            {
                PinballManager.ActivateWorkshopExtraBalls();
            }

            // Activating 15 seconds of autosave
            if (Input.GetKeyDown(KeyCode.U))
            {
                PinballManager.Instance.ActivateAutosave(15);
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

            // Camera shake
            if (Input.GetKeyUp(KeyCode.S))
            {
                GameManager.Instance.ShakeCamera
                    (Vector3.forward, 0, 0.8f, 0.2f);
            }

            // Spawning one collectable
            if (Input.GetKeyUp(KeyCode.Z))
            {
                _collSpawner.SpawnCollectable
                    (Collectable.CollectableType.Gold);
            }

            // Spawning all collectables
            if (Input.GetKeyUp(KeyCode.X))
            {
                _collSpawner.SpawnCollectables
                    (Collectable.CollectableType.Salt, 5, 0.5f);
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
                        ExitSettings(false);
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

            if (_textInputActive)
            {
                DeactivateTextInput();
            }
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
                _submitHoldover = true;
                _stateManager.StartNewGame();
            }
        }

        public void ResumeGame()
        {
            _submitHoldover = true;
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
            // Stores the screen from where settings were accessed
            _settingsAccessorScreen = _stateManager.CurrentScreenState.State;

            // Stores old settings
            _oldLangCode = L10n.CurrentLanguage.LanguageCode;
            _oldEnableEventCam = Settings.Instance.EnableEventCamera;

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
                ExitSettings(true);
            }
        }

        private void ExitSettings(bool saveSettings)
        {
            // Saves the settings
            if (saveSettings)
            {
                GameManager.Instance.SaveSettings();
            }
            // Reverts any changes to the settings
            else
            {
                L10n.LoadLanguage(_oldLangCode);
                Settings.Instance.EnableEventCamera = _oldEnableEventCam;
            }
            
            _stateManager.PerformTransition(_settingsAccessorScreen);
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
                _highscoreList.ResetList();
            }
        }

        public void ActivateTextInput()
        {
            _playerNameTextInput.Activate();
            _textInputActive = true;
            _playerName.color =
                new Color(105f / 255f, 243f / 255f, 1, 1); // #69F3FFFF
            _eventSystem.sendNavigationEvents = false;

            if (GameManager.Instance.DefaultNameUsed)
            {
                SetPlayerName("");
            }
            else
            {
                UpdatePlayerNameUIText();
            }

            // Clears the menu button selection
            EventSystem.current.SetSelectedGameObject(null);
        }

        public void DeactivateTextInput()
        {
            if (GameManager.Instance.PlayerName.Length == 0)
            {
                GameManager.Instance.SetPlayerNameToDefault();
            }

            _playerNameTextInput.Deactivate();
            _textInputActive = false;
            _playerName.color = Color.white;
            _eventSystem.sendNavigationEvents = true;
            UpdatePlayerNameUIText();

            // Clears the menu button selection
            EventSystem.current.SetSelectedGameObject(null);
            HighlightMenuDefaultButton();
        }

        public void ToggleTextInput()
        {
            if (_textInputActive)
            {
                DeactivateTextInput();
            }
            else
            {
                ActivateTextInput();
            }
        }

        private void CheckIfSubmitReleased()
        {
            if (_submitHoldover && !Input.GetButton(_SUBMIT))
            {
                _submitHoldover = false;
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
            // Text input does not count
            if (_textInputActive)
            {
                return false;
            }

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

        private void OnLanguageLoaded()
        {
            _playerName.text = GameManager.Instance.PlayerName;
        }
    }
}
