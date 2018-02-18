using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                    // Note:
                    // There must be a Resources folder under Assets and
                    // GameManager there for this to work. Not necessary if
                    // a GameManager object is present in a scene from the
                    // get-go.

                    instance =
                        Instantiate(Resources.Load<GameManager>("GameManager"));

                    //GameObject gmObj = Instantiate(Resources.Load("GameManager") as GameObject);
                    //instance = gmObj.GetComponent<GameManager>();
                }

                return instance;
            }
        }
        #endregion Statics

        [SerializeField]
        private bool debug_UnlockAll;

        [SerializeField]
        private bool debug_ResetData;

        [SerializeField]
        private LanguageStateBase.Language _defaultLanguage =
            LanguageStateBase.Language.English;

        [SerializeField]
        private ScreenStateBase.ScreenStateType _startUpScreen =
            ScreenStateBase.ScreenStateType.MainMenu;

        private LanguageStateBase _language;
        private LanguageStateBase _lang_english;
        private LanguageState_Finnish _lang_finnish;

        private ScreenStateBase _screenState;
        private ScreenState_MainMenu _screenMainMenu;
        private ScreenState_Game _screenGame;

        private GameModeStateBase _gameModeState;
        private GameModeState_Normal _gameModeNormal;

        private float _musicVolume;
        private float _effectVolume;

        public LanguageStateBase Language
        {
            get
            {
                return _language;
            }
        }

        public ScreenStateBase Screen
        {
            get
            {
                return _screenState;
            }
        }

        public GameModeStateBase GameMode
        {
            get
            {
                return _gameModeState;
            }
        }

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

        private void Update()
        {
            //InitScene();

            // DEBUGGING
            if (_defaultLanguage != _language.SelectedLanguage)
            {
                SetLanguage(_defaultLanguage);
            }
        }

        private void Init()
        {
            //LoadGame();

            DontDestroyOnLoad(gameObject);

            // Initializes states
            InitLanguages();
            InitScreens();
            InitGameModes();
            SetLanguage(_defaultLanguage);
            SetScreen(_startUpScreen);
            SetGameMode(GameModeStateBase.GameModeStateType.Normal);

            //sceneChanged = true;
            //InitScene();

            // Initializes volume
            //MusicVolume = 0.5f;
            //EffectVolume = 0.5f;

            // Creates a new MusicPlayer instance
            // if one does not already exist
            //MusicPlayer.Instance.Create();

            if (debug_UnlockAll)
            {
                //LatestCompletedLevel = 8;
            }

            if (debug_ResetData)
            {
                //Reset();
            }
        }

        private void InitLanguages()
        {
            _lang_english = new LanguageStateBase();
            _lang_finnish = new LanguageState_Finnish();
        }

        private void InitScreens()
        {
            _screenMainMenu = new ScreenState_MainMenu();
            _screenGame = new ScreenState_Game();
        }

        private void InitGameModes()
        {
            _gameModeNormal = new GameModeState_Normal();
        }

        private void SetLanguage(LanguageStateBase.Language language)
        {
            switch (language)
            {
                case LanguageStateBase.Language.Finnish:
                {
                    _language = _lang_finnish;
                    break;
                }
                default:
                {
                    _language = _lang_english;
                    break;
                }
            }

            Debug.Log("Selected language: " + _language.SelectedLanguage);
        }

        private void SetScreen(ScreenStateBase.ScreenStateType screen)
        {
            switch (screen)
            {
                case ScreenStateBase.ScreenStateType.MainMenu:
                {
                    _screenState = _screenMainMenu;
                    break;
                }
                default:
                {
                    _screenState = _screenGame;
                    break;
                }
            }
        }

        private void SetGameMode(GameModeStateBase.GameModeStateType gameMode)
        {
            switch (gameMode)
            {
                //case GameModeStateBase.GameModeStateType.Sauna:
                //{
                //    _gameModeState = ;
                //    break;
                //}
                default:
                {
                    _gameModeState = _gameModeNormal;
                    break;
                }
            }
        }

        //private void InitScene()
        //{
        //    if (sceneChanged)
        //    {
        //        sceneChanged = false;

        //        InitLevel();

        //        InitPlayerCharacters();

        //        InitCamera();

        //        InitCycles();

        //        //ResetInput();
        //        InitInput();

        //        //ResetFade();
        //        InitFade();

        //        MenuExited = true;

        //        levelStartUp = true;
        //    }
        //}

        //private void InitInput()
        //{
        //    if (input == null)
        //    {
        //        input = FindObjectOfType<PlayerInput>();
        //    }
        //}

        //private void ResetInput()
        //{
        //    if (input != null)
        //    {
        //        input = null;
        //    }
        //}

        //private void InitCamera()
        //{
        //    gameCamera = FindObjectOfType<CameraController>();

        //    if (gameCamera == null)
        //    {
        //        Debug.LogError("Could not find a CameraController " +
        //                       "object in the scene.");
        //    }
        //}

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

        //private void SaveGame()
        //{
        //    // Note: Saved data can be found in
        //    // regedit > Tietokone\HKEY_CURRENT_USER\Software\Unity\UnityEditor\TeamAF\not - broforce

        //    PlayerPrefs.SetInt("latestCompletedLevel", latestCompletedLevel);
        //    PlayerPrefs.SetFloat("musicVolume", musicVolume);
        //    PlayerPrefs.SetFloat("effectVolume", effectVolume);

        //    //Utils.PlayerPrefsSetBool(
        //    //    "alwaysShowBoxSelector", alwaysShowBoxSelector);
        //    //Utils.PlayerPrefsSetBool(
        //    //    "holdToActivateBoxSelector", holdToActivateBoxSelector);

        //    PlayerPrefs.Save();
        //    Debug.Log("--[ Game saved ]--");
        //    Debug.Log("latestCompletedLevel: " + latestCompletedLevel);
        //}

        //private void LoadGame()
        //{
        //    latestCompletedLevel = PlayerPrefs.GetInt("latestCompletedLevel", 0);
        //    musicVolume = PlayerPrefs.GetFloat("musicVolume", 0.5f);
        //    effectVolume = PlayerPrefs.GetFloat("effectVolume", 0.5f);

        //    //alwaysShowBoxSelector =
        //    //    Utils.PlayerPrefsGetBool("alwaysShowBoxSelector", false);
        //    //holdToActivateBoxSelector =
        //    //    Utils.PlayerPrefsGetBool("holdToActivateBoxSelector", false);

        //    Debug.Log("--[ Game loaded ]--");
        //    Debug.Log("latestCompletedLevel: " + latestCompletedLevel);
        //}

        //public void SaveSettings()
        //{
        //    PlayerPrefs.SetFloat("musicVolume", musicVolume);
        //    PlayerPrefs.SetFloat("effectVolume", effectVolume);

        //    //Utils.PlayerPrefsSetBool(
        //    //    "alwaysShowBoxSelector", alwaysShowBoxSelector);
        //    //Utils.PlayerPrefsSetBool(
        //    //    "holdToActivateBoxSelector", holdToActivateBoxSelector);

        //    PlayerPrefs.Save();
        //    Debug.Log("Settings saved");
        //}

        //public void Reset()
        //{
        //    CurrentLevel = 0;

        //    // Overwrites the existing save!
        //    LatestCompletedLevel = 0;
        //}

        //public void StartSceneChange(string sceneName)
        //{
        //    if (!changingScene)
        //    {
        //        nextScene = sceneName;
        //        changingScene = true;

        //        if (fade != null)
        //        {
        //            fade.StartFadeOut();
        //        }

        //        Debug.Log("Next scene: " + sceneName);
        //    }
        //}

        //public void LoadScene()
        //{
        //    LoadScene(nextScene);
        //}

        public void LoadScene(string sceneName)
        {
            // Stops all SFX
            //SFXPlayer.Instance.StopAllSFXPlayback();

            //changingScene = false;
            //sceneChanged = true;
            SceneManager.LoadScene(sceneName);
        }

        private bool menuExited = false;
        private float exitTime = 0f;
        private float exitDuration = 0.2f;

        /// <summary>
        /// Gets or sets whether a menu was just exited.
        /// Setting this true starts a short timer after whose
        /// completion the value returns to false.
        /// </summary>
        public bool MenuExited
        {
            get
            {
                if (menuExited &&
                    Time.time - exitTime >= exitDuration)
                {
                    menuExited = false;
                }

                return menuExited;
            }
            set
            {
                menuExited = value;

                if (value == true)
                {
                    exitTime = Time.time;
                }
            }
        }
    }
}
