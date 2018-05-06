using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Kalevala.Persistence;
using Kalevala.Localization;
using L10n = Kalevala.Localization.Localization;

namespace Kalevala
{
    public class GameManager : MonoBehaviour
    {
        #region Statics
        private static GameManager instance;

        private static int _timeModulo;

        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameManager>();

                    if (instance == null)
                    {
                        // Prints an error message.
                        // A GameManager object must be in the scene.
                        Debug.LogError("A GameManager object has not " +
                                       "been added to the scene.");

                        // Note:
                        // There must be a Resources folder under Assets and
                        // GameManager there for this to work. Not necessary if
                        // a GameManager object is present in a scene from the
                        // get-go.

                        //instance =
                        //    Instantiate(Resources.Load<GameManager>("GameManager"));

                        //GameObject gmObj = Instantiate(Resources.Load("GameManager") as GameObject);
                        //instance = gmObj.GetComponent<GameManager>();
                    }
                }

                return instance;
            }
        }
            public static int TimeModulo
        {
            get
            {
                return _timeModulo;
            }

            set
            {
                
                _timeModulo = value;
            }
        }
    
        #endregion Statics

        private const string PlayerKey = "player";

        /// <summary>
        /// Preferences key for the saved language setting
        /// </summary>
        private const string LanguageKey = "Language";

        // DEBUGGING
        public bool debug_SkipMainMenu;

        //[SerializeField]
        //private LanguageStateType _defaultLanguage =
        //    LanguageStateType.English;

        [SerializeField]
        private LangCode _defaultLanguage = LangCode.EN;

        private StateManager _stateManager;
        private Playfield _playfield;
        private CollectableSpawner _collectableSpawner;
        private CameraController _cameraCtrl;
        private HighscoreList _highscoreList;
        private SaveSystem _saveSystem;

        //private IList<LanguageStateBase> _langStates =
        //    new List<LanguageStateBase>();

        //public LanguageStateBase Language { get; set; }

        private string _playerName;

        public HighscoreList HighscoreList
        {
            get
            {
                return _highscoreList;
            }
        }

        public string PlayerName
        {
            get
            {
                return _playerName;
            }
            set
            {
                if (DefaultNameUsed)
                {
                    DefaultNameUsed = false;
                }

                _playerName = value;
            }
        }

        public bool DefaultNameUsed { get; private set; }

        public string SavePath
        {
            get
            {
                return Path.Combine(Application.persistentDataPath,
                    "saveData");
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Init();
        }

        //Neither currently does anythin but since they existed Unity was still calling them. So commented out for now.

        //private void Start()
        //{
        //    // TODO: If _autoplayMusic is off, call InitAudio
        //    // when starting playing music

        //    //Settings.Instance.InitAudio(_autoplayMusic);
        //}

        private void Update()
        {
            //InitScene();

            // DEBUGGING
            //if (_defaultLanguage != Language.State)
            //{
            //    SetLanguage(_defaultLanguage);
            //}

            TimeModulo = Mathf.CeilToInt(Time.time * 10) % 10;
        }

        private void Init()
        {
            // Initializes the highscore list
            _highscoreList = FindObjectOfType<HighscoreList>();
            if (_highscoreList == null)
            {
                Debug.LogError("HighscoreList object not found in the scene.");
            }

            // Initializes localization
            InitLocalization();

            // Initializes the save system and loads data
            _saveSystem = new SaveSystem(new JSONPersistence(SavePath));
            LoadGame();

            // Initializes states
            _stateManager = FindObjectOfType<StateManager>();
            if (_stateManager != null)
            {
                _stateManager.Init();
            }
            else
            {
                Debug.LogError("StateManager object not found in the scene.");
            }

            _playfield = FindObjectOfType<Playfield>();
            if (_playfield == null)
            {
                Debug.LogError("Playfield object not found in the scene.");
            }

            _collectableSpawner = FindObjectOfType<CollectableSpawner>();
            _cameraCtrl = FindObjectOfType<CameraController>();

            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Initializes localization.
        /// </summary>
        private void InitLocalization()
        {
            LangCode currentLang = _defaultLanguage;
                //(LangCode) PlayerPrefs.GetInt(LanguageKey, (int) _defaultLanguage);
            L10n.LoadLanguage(currentLang);
            L10n.LanguageLoaded += OnLanguageLoaded;
        }

        /// <summary>
        /// Called when a LanguageLoaded event is fired.
        /// </summary>
        private void OnLanguageLoaded()
        {
            if (DefaultNameUsed)
            {
                SetPlayerNameToDefault();
            }
        }

        public void SetLanguage(LangCode languageCode)
        {
            L10n.LoadLanguage(languageCode);
            Debug.Log("Selected language: " + languageCode);
        }

        public StateManager StateManager
        {
            get
            {
                return _stateManager;
            }
        }

        public ScreenStateBase Screen
        {
            get
            {
                return _stateManager.CurrentScreenState;
            }
        }

        public GameModeStateBase GameMode
        {
            get
            {
                return _stateManager.CurrentGameModeState;
            }
        }

        public void SaveOrRevertHighscores(bool saveScores)
        {
            // Saves the score if it's high enough
            // and the player actually finished
            // the game instead of giving up
            if (saveScores)
            {
                _highscoreList.SaveHighscores();
                //_highscoreList.CompareScoreAndSave
                //    (_playerName, Scorekeeper.Instance._totalScore);
            }
            else
            {
                _highscoreList.RevertHighscores();
            }
        }

        /// <summary>
        /// Resets the playfield and the current score for a clean start.
        /// </summary>
        public void ResetAll()
        {
            PinballManager.Instance.ResetGame();
            Scorekeeper.Instance.ResetScore();
            _highscoreList.InitPlay();
            _playfield.ResetPlayfield();
            _collectableSpawner.ResetCollectables();
        }

        /// <summary>
        /// Erases local highscores and saves the game.
        /// </summary>
        public void EraseLocalHighscores()
        {
            _highscoreList.ResetList();
            SaveGame("Saving reset highscores");
        }

        public void SetPlayerNameToDefault()
        {
            PlayerName = L10n.CurrentLanguage.GetTranslation(PlayerKey);
            DefaultNameUsed = true;
            Debug.Log("Setting player name to default");
        }

        //private void InitFade()
        //{
        //    fade = FindObjectOfType<FadeToColor>();

        //    if (fade == null)
        //    {
        //        Debug.LogError("Could not find a FadeToColor " +
        //                       "object in the scene.");
        //    }
        //}

        //private void ResetFade()
        //{
        //    if (fade != null)
        //    {
        //        fade = null;
        //    }
        //}

        //public bool ClearFade
        //{
        //    get
        //    {
        //        return (fade == null || fade.FadedIn);
        //    }
        //}

        /// <summary>
        /// Gets highscore data and stores it to a data object.
        /// </summary>
        /// <param name="saveMessage">A printed message</param>
        public void SaveGame(string saveMessage = "Saving game")
        {
            // Default save message
            if (saveMessage.Length == 0)
            {
                saveMessage = "Saving game";
            }

            // Prints the save message
            Debug.Log("--[ " + saveMessage + " ]--");

            GameData data = new GameData();

            // Gets highscores and stores them to the data
            HighscoreList.FetchHighscoreData(data);

            _saveSystem.Save(data);

            // Prints a save success message 
            Debug.Log("--[ Saved ]--");
        }

        public void LoadGame()
        {
            Debug.Log("--[ Loading game ]--");

            GameData data = _saveSystem.Load();

            // Loads highscores and sets them to the scoreboard
            HighscoreList.LoadHighscores(data);

            LoadPlayerName();

            Settings.Instance.Load();

            Debug.Log("--[ Game loaded ]--");
        }

        public void SaveSettings()
        {
            // Note: Saved data can be found in
            // regedit > Tietokone\HKEY_CURRENT_USER\Software\Unity\UnityEditor\TeamAF\not - broforce

            // Saves the currently selected language
            PlayerPrefs.SetInt(LanguageKey,
                (int) L10n.CurrentLanguage.LanguageCode);

            Settings.Instance.Save();

            PlayerPrefs.Save();

            Debug.Log("Settings saved");
        }

        /// <summary>
        /// Saves the player's name if it's not the default name.
        /// </summary>
        public void SavePlayerName()
        {
            if (DefaultNameUsed)
            {
                PlayerPrefs.SetString(PlayerKey, "");
            }
            else
            {
                PlayerPrefs.SetString(PlayerKey, PlayerName);
            }
        }

        public void LoadPlayerName()
        {
            string savedPlayerName = PlayerPrefs.GetString(PlayerKey, "");

            // If no name is saved, the default name is used
            if (savedPlayerName.Length == 0)
            {
                SetPlayerNameToDefault();
            }
            else
            {
                PlayerName = savedPlayerName;
            }
        }

        public void LoadScene(string sceneName)
        {
            // Stops all SFX
            //SFXPlayer.Instance.StopAllSFXPlayback();

            //changingScene = false;
            //sceneChanged = true;
            SceneManager.LoadScene(sceneName);
        }

        public void SetCameraMode(CameraController.CameraType camType)
        {
            _cameraCtrl.SetCurrentCamera(camType);
        }

        public void SetCameraFocus(CameraController.CameraPosition camPos)
        {
            if ( !_stateManager.GameIsPaused(true) )
            {
                _cameraCtrl.MoveCurrentCamTo(camPos, false);
            }
        }

        public void ShakeCamera(Vector3 direction,
            float randomDirAngle, float force, float duration)
        {
            if (!_stateManager.GameIsPaused(true))
            {
                _cameraCtrl.Shake(direction, randomDirAngle, force, duration);
            }
        }
    }
}
