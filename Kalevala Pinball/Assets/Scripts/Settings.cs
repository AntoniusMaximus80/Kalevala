using UnityEngine;

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

        [SerializeField]
        private bool _autoplayMusic = true;

        [SerializeField]
        private bool _enableEventCamera = true;

        private bool _settingsLoaded;
        private bool _audioInitialized = false;

        private float _musicVolume;
        private float _effectVolume;

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
            InitAudio();
            EnableEventCamera = _enableEventCamera;
        }

        public void InitAudio()
        {
            _audioInitialized = true;

            MusicPlayer.Instance.SetVolume(_musicVolume);
            //SFXPlayer.Instance.SetVolume(_effectVolume);

            if (!_autoplayMusic)
            {
                MusicPlayer.Instance.Stop();
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
            }
        }

        public void SetMusicVolume(float value)
        {
            MusicVolume = value;
        }

        public void SetEffectVolume(float value)
        {
            EffectVolume = value;
        }

        public void SetEnableEventCamera(bool enable)
        {
            EnableEventCamera = enable;
        }

        public void Save()
        {
            //Debug.Log("musicVolume: " + _musicVolume);
            //Debug.Log("effectVolume: " + _effectVolume);
            PlayerPrefs.SetFloat("musicVolume", _musicVolume);
            PlayerPrefs.SetFloat("effectVolume", _effectVolume);
        }

        public void Load()
        {
            //Debug.Log("musicVolume: " + _musicVolume);
            //Debug.Log("effectVolume: " + _effectVolume);
            PlayerPrefs.SetFloat("musicVolume", _musicVolume);
            PlayerPrefs.SetFloat("effectVolume", _effectVolume);
        }
    }
}
