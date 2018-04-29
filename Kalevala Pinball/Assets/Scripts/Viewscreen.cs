using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using L10n = Kalevala.Localization.Localization;

namespace Kalevala
{
    public class Viewscreen : MonoBehaviour
    {
        private const string CurrentBallKey = "currentBall";
        private const string JackpotKey = "jackpot";
        private const string SampoModeRequirementKey = "sampoModeRequirement";
        private const string SampoModeActivationKey = "sampoModeActivated";

        public StateManager _stateManager;

        // references to the viewscreen texts.
        public TextMeshProUGUI
            _scoreUGUI,
            _incrementUGUI,
            _ballCounter,
            _smallScore,
            _gameMode;

        public float _incrementVisible = 5f;

        private static float _incrementVisibleCountdown, _displayModeCountdown = 2f;

        private Color _fade = new Color(1f, 1f, 1f, 1f);

        private static Viewscreen _instance;
        private bool _showMode, _displayMode;

        private static string[] _numbers; /* = { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven" };*/
        private string _score;
        private float _timeSinceIncrement;

        // Use this for initialization
        void Awake()
        {
            _instance = this;
            _incrementVisibleCountdown = _incrementVisible;
        }

        private void Start()
        {
            _numbers = new string[8];

            L10n.LanguageLoaded += OnLanguageLoaded;
            OnLanguageLoaded();
        }

        // Update is called once per frame
        void Update()
        {
            if (_incrementVisibleCountdown != 0f)
            {
                _incrementVisibleCountdown -= Time.deltaTime;
                Mathf.Clamp01(_incrementVisibleCountdown);
                float ratio = _incrementVisibleCountdown / _incrementVisible;
                _fade.a = ratio;
                _incrementUGUI.color = _fade;

                if (_incrementVisibleCountdown <= 0f)
                {
                    _instance._scoreUGUI.text = _score;
                    _instance._scoreUGUI.fontSize = 4;
                }
            }

            if(_showMode)
            {
                _displayModeCountdown -= Time.deltaTime;

                if(_displayModeCountdown<=0)
                {
                    // Make the switch two seconds by default. I am assuming this is something
                    // that we won't change a lot.
                    _displayModeCountdown = 2f;

                    _displayMode = _displayMode ? false : true;
                    _gameMode.gameObject.SetActive(_displayMode);
                    _scoreUGUI.gameObject.SetActive(!_displayMode);
                }
            }
        }

        public static void FormatScore(int score, int increase, string message)
        {
            _instance._score = score.ToString("N0");
            _instance._scoreUGUI.text = String.Format(message, increase);
            _instance._scoreUGUI.fontSize = 3;
            _instance._smallScore.text = score.ToString("N0");
            _instance._incrementUGUI.text = "+" + score.ToString("N0");
            _incrementVisibleCountdown = _instance._incrementVisible;

        }

        public static void WorkShopEntered(int entriesNeeded)
        {
            string translation = L10n.CurrentLanguage.GetTranslation(SampoModeRequirementKey);
            _instance._gameMode.text = string.Format(translation, _numbers[entriesNeeded]);
            //_instance._gameMode.text = _numbers[entriesNeeded]+" more to activate Sampo mode.";
            _instance._showMode = true;
        }

        public static void FormatScoreIncrement(int score, String message)
        {
            _instance._incrementUGUI.text = "+"+score.ToString("N0");
            _incrementVisibleCountdown = _instance._incrementVisible;
            _instance._gameMode.text = message;
            _instance._showMode = true;
        }

        public static void BallCount(int balls)
        {
            string translation = L10n.CurrentLanguage.GetTranslation(CurrentBallKey);
            _instance._ballCounter.text = string.Format(translation, balls.ToString("N0"));
            //_instance._ballCounter.text = "Balls left: " + balls.ToString("N0");
        }

        // Added a proper method for restting this. It was needed but did not exist, oops.
        internal static void ResetScore()
        {
            _instance._score ="0";
            _instance._scoreUGUI.text = _instance._score;
            _instance._scoreUGUI.fontSize = 4;
            _instance._smallScore.text = _instance._score;
            _instance._showMode = false;
            _incrementVisibleCountdown = 0;
            _displayModeCountdown = 2f;
            

        }

        public static void StartSampoMode()
        {
            string translation = L10n.CurrentLanguage.GetTranslation(SampoModeActivationKey);
            _instance._gameMode.text = translation;
            //_instance._gameMode.text = "Sampo mode activated.";
            _instance._showMode = true;
        }

        private void OnLanguageLoaded()
        {
            _numbers[0] = L10n.CurrentLanguage.GetTranslation("zero");
            _numbers[1] = L10n.CurrentLanguage.GetTranslation("one");
            _numbers[2] = L10n.CurrentLanguage.GetTranslation("two");
            _numbers[3] = L10n.CurrentLanguage.GetTranslation("three");
            _numbers[4] = L10n.CurrentLanguage.GetTranslation("four");
            _numbers[5] = L10n.CurrentLanguage.GetTranslation("five");
            _numbers[6] = L10n.CurrentLanguage.GetTranslation("six");
            _numbers[7] = L10n.CurrentLanguage.GetTranslation("seven");

            BallCount(PinballManager.Instance.CurrentBallNumber);
        }
    }
}