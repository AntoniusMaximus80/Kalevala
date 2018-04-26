using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Kalevala
{
    public class Settings : MonoBehaviour
    {
        #region Statics
        private static Settings instance;

        public static Settings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<Settings>();
                    if (instance == null)
                    {
                        // Prints an error message.
                        // A Settings object must be in the scene.
                        Debug.LogError("A Settings object has not " +
                                       "been added to the scene.");
                    }
                }

                return instance;
            }
        }
        #endregion Statics

        private const string MusicVolumeKey = "musicVolume";
        private const string EffectVolumeKey = "effectVolume";

        [SerializeField]
        private float _defaultMusicVolume = 0.1f;

        [SerializeField]
        private float _defaultEffectVolume = 0.1f;

        [SerializeField]
        private bool _autoplayMusic = true;

        [SerializeField]
        private Slider _musicVolumeSlider;

        [SerializeField]
        private Slider _effectVolumeSlider;

        [SerializeField]
        private Toggle _enableEventCamToggle;

        //[SerializeField]
        private bool _enableEventCamera = true;

        private bool _settingsLoaded;
        private bool _audioInitialized = false;

        private float _musicVolume;
        private float _effectVolume;

        private InputManager _input;

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
        }

        private void Start()
        {
            _input = FindObjectOfType<InputManager>();
            InitAudio();
            InitUIObjects();
            //EnableEventCamera = _enableEventCamera;
        }

        private void InitAudio()
        {
            _audioInitialized = true;

            //MusicPlayer.Instance.SetVolume(_musicVolume);
            //SFXPlayer.Instance.SetVolume(_effectVolume);

            if (!_autoplayMusic)
            {
                MusicPlayer.Instance.Stop();
            }
        }

        private void InitUIObjects()
        {
            _musicVolumeSlider.value = MusicVolume;
            _effectVolumeSlider.value = EffectVolume;
        }

        public float MusicVolume
        {
            get
            {
                return _musicVolume;
            }
            set
            {
                _musicVolume = Mathf.Clamp(value, 0, 1);

                if (_audioInitialized)
                {
                    MusicPlayer.Instance.SetVolume(value);
                }
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
                _effectVolume = Mathf.Clamp(value, 0, 1);

                if (_audioInitialized)
                {
                    SFXPlayer.Instance.SetVolume(value);
                }
            }
        }

        public bool EnableEventCamera
        {
            get
            {
                return _enableEventCamera;
            }
            set
            {
                _enableEventCamera = value;

                if (!value)
                {
                    StateManager.HideLaunch();
                }
                else if (Launcher.Instance.BallOnLauncher)
                {
                    StateManager.ShowLaunch();
                }
            }
        }

        public void OnMusicVolumeValueChanged()
        {
            MusicVolume = _musicVolumeSlider.value;
        }

        public void OnEffectVolumeValueChanged()
        {
            EffectVolume = _effectVolumeSlider.value;
        }

        public void OnEnableEventCameraValueChanged()
        {
            EnableEventCamera = _enableEventCamToggle.isOn;
        }

        public void EraseHighscores(bool skipConfirmation)
        {
            if (!skipConfirmation)
            {
                _input.Confirm(ConfirmationType.EraseHighscores);
            }
            else
            {
                GameManager.Instance.EraseLocalHighscores();

                // Highlight the default menu button if the mouse is not used
                if (!_input.HighlightMenuDefaultButton())
                {
                    // Clears the menu button selection if the mouse is used
                    EventSystem.current.SetSelectedGameObject(null);
                }
            }
        }

        public void Save()
        {
            PlayerPrefs.SetFloat(MusicVolumeKey, _musicVolume);
            PlayerPrefs.SetFloat(EffectVolumeKey, _effectVolume);
            // PlayerPrefs.Save();
        }

        public void Load()
        {
            MusicVolume = PlayerPrefs.GetFloat
                (MusicVolumeKey, _defaultMusicVolume);
            EffectVolume = PlayerPrefs.GetFloat
                (EffectVolumeKey, _defaultEffectVolume);
        }
    }
}
