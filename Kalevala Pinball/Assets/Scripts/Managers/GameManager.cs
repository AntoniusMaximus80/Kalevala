using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Kalevala.Persistence;

namespace Kalevala
{
    public class GameManager : MonoBehaviour
    {
        #region Statics
        private static GameManager instance;

        public static GameManager Instance
        {
            get
            {
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

                return instance;
            }
        }
        #endregion Statics

        // DEBUGGING
        public bool debug_SkipMainMenu;

        [SerializeField]
        private LanguageStateType _defaultLanguage =
            LanguageStateType.English;

        private StateManager _stateManager;
        private Playfield _playfield;
        private CollectableSpawner _collectableSpawner;
        private CameraController _cameraCtrl;
        private HighscoreList _highscoreList;
        private SaveSystem _saveSystem;

        private IList<LanguageStateBase> _langStates =
            new List<LanguageStateBase>();

        public LanguageStateBase Language { get; set; }

        public string _playerName = "Player";

        private float _musicVolume;
        private float _effectVolume;

        public event Action ResourcesChanged;
        private int _resources;

        public float MusicVolume
        {
            get
            {
                return _musicVolume;
            }
            set
            {
                _musicVolume = value;
                MusicPlayer.Instance.SetVolume(value);
            }
        }

        public float EffectVolume
        {
            get
            {
                return _effectVolume;
            }
            set
            {
                _effectVolume = value;
                SFXPlayer.Instance.SetVolume(value);
            }
        }

        public int Resources
        {
            get
            {
                return _resources;
            }
            private set
            {
                Debug.Log(_resources);
                _resources = (int) Mathf.Clamp(_resources + value,0,45f);
                if(ResourcesChanged != null)
                {
                    ResourcesChanged();
                }
            }
        }

        public HighscoreList HighscoreList
        {
            get
            {
                return _highscoreList;
            }
        }

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

        private void Start()
        {
            InitAudio();
        }

        private void Update()
        {
            //InitScene();

            // DEBUGGING
            if (_defaultLanguage != Language.State)
            {
                SetLanguage(_defaultLanguage);
            }
        }

        private void Init()
        {
            // Initializes the highscore list
            _highscoreList = FindObjectOfType<HighscoreList>();
            if (_highscoreList == null)
            {
                Debug.LogError("HighscoreList object not found in the scene.");
            }

            // Initializes the save system and loads data
            _saveSystem = new SaveSystem(new JSONPersistence(SavePath));
            LoadGame();

            // Initializes states
            _stateManager = FindObjectOfType<StateManager>();
            if (_stateManager == null)
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

            // Initializes languages
            InitLanguages();

            DontDestroyOnLoad(gameObject);
        }

        private void InitLanguages()
        {
            LanguageStateBase lang_english = new LanguageStateBase();
            LanguageState_Finnish lang_finnish = new LanguageState_Finnish();
            _langStates.Add(lang_english);
            _langStates.Add(lang_finnish);

            SetLanguage(_defaultLanguage);
        }

        private void InitAudio()
        {
            // Creates a new MusicPlayer instance
            // if one does not already exist
            //MusicPlayer.Instance.Create();

            MusicPlayer.Instance.SetVolume(MusicVolume);
        }

        public void SetLanguage(LanguageStateType language)
        {
            Language = GetLanguage(language);
            Debug.Log("Selected language: " + Language.State);
        }

        private LanguageStateBase GetLanguage(LanguageStateType stateType)
        {
            // Returns the first object from the state list whose State property's
            // value equals to stateType. If no object was found, null is returned.

            foreach (LanguageStateBase state in _langStates)
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

        public void GameOver(bool saveScore)
        {
            // NOTE: Called by ScreenState_Play
            //_stateManager.EndGame(saveScore);

            // Saves the score if it's high enough
            // and the player actually finished
            // the game instead of giving up
            if (saveScore)
            {
                //bool isHighscore =
                _highscoreList.CompareScoreAndSave
                    (_playerName, Scorekeeper.Instance._totalScore);

                //if (isHighscore)
                //{

                //}
            }
        }

        /// <summary>
        /// Resets the playfield and the current score for a clean start.
        /// </summary>
        public void ResetPlay()
        {
            PinballManager.Instance.ResetGame();
            _playfield.ResetPlayfield();
            Scorekeeper.Instance.ResetScore();
            _collectableSpawner.ResetCollectables();
        }

        /// <summary>
        /// Erases local highscores and saves the game.
        /// </summary>
        public void EraseLocalHighscores()
        {
            _highscoreList.ResetList();
            _highscoreList.UpdateScoreboard();
            SaveGame("Saving reset highscores");
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

            // Loads audio volumes but doesn't set them to the audio
            // players because they have not been initialized yet
            _musicVolume = PlayerPrefs.GetFloat("musicVolume", 0.25f);
            _effectVolume = PlayerPrefs.GetFloat("effectVolume", 0.25f);

            Debug.Log("--[ Game loaded ]--");
        }

        public void SaveSettings()
        {
            // Note: Saved data can be found in
            // regedit > Tietokone\HKEY_CURRENT_USER\Software\Unity\UnityEditor\TeamAF\not - broforce

            Debug.Log("musicVolume: " + _musicVolume);
            Debug.Log("effectVolume: " + _effectVolume);
            PlayerPrefs.SetFloat("musicVolume", MusicVolume);
            PlayerPrefs.SetFloat("effectVolume", EffectVolume);

            PlayerPrefs.Save();
            Debug.Log("Settings saved");
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

        public void ChangeResources(int amount)
        {
            Resources = amount;
        }
    }
}
