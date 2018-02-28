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

        [SerializeField]
        private bool debug_UnlockAll;

        [SerializeField]
        private bool debug_ResetData;

        [SerializeField]
        private LanguageStateType _defaultLanguage =
            LanguageStateType.English;

        private StateManager stateManager;
        private Playfield playfield;

        private IList<LanguageStateBase> _langStates = new List<LanguageStateBase>();
        public LanguageStateBase Language { get; set; }

        private float _musicVolume;
        private float _effectVolume;

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
            if (_defaultLanguage != Language.State)
            {
                SetLanguage(_defaultLanguage);
            }
        }

        private void Init()
        {
            //LoadGame();

            DontDestroyOnLoad(gameObject);

            // Initializes states
            stateManager = FindObjectOfType<StateManager>();
            if (stateManager == null)
            {
                Debug.LogError("StateManager object not found in the scene.");
            }

            playfield = FindObjectOfType<Playfield>();
            if (playfield == null)
            {
                Debug.LogError("Playfield object not found in the scene.");
            }

            // Initializes languages
            InitLanguages();

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
            LanguageStateBase lang_english = new LanguageStateBase();
            LanguageState_Finnish lang_finnish = new LanguageState_Finnish();
            _langStates.Add(lang_english);
            _langStates.Add(lang_finnish);

            SetLanguage(_defaultLanguage);
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

        public ScreenStateType Screen
        {
            get
            {
                return stateManager.CurrentScreenState.State;
            }
        }

        public GameModeStateType GameMode
        {
            get
            {
                return stateManager.CurrentGameModeState.State;
            }
        }

        public void GameOver(bool saveScore)
        {
            // TODO
        }

        public void ResetAll()
        {
            PinballManager.Instance.ResetGame();
            Scorekeeper.Instance.ResetScore();
            playfield.ResetPlayfield();
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
