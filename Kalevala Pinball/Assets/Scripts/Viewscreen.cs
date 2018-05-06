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

        public StrobeLight _bumberIndicator, _workshopIndicator, _sampoIndicator;

        private static float _incrementVisibleCountdown, _displayModeCountdown = 2f;

        private Color _fade = new Color(1f, 1f, 1f, 1f);

        private static Viewscreen _instance;
        private bool _showMode, _displayMode;

        private static string[] _numbers; /* = { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven" };*/
        private string _score;
        private static float _incrementTime;

        private static bool _launch, _kanteleHero, _sampo, _collect;
        private static int _workshopCountdown;

        public static float TimeSinceIncrement
        {
            get
            {
                return (Time.time - _incrementTime);
            }

        }

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

            if (_showMode)
            {
                _displayModeCountdown -= Time.deltaTime;

                if (_displayModeCountdown <= 0)
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
            _instance._scoreUGUI.fontSize = 2.7f;
            _instance._smallScore.text = score.ToString("N0");
            _instance._incrementUGUI.text = "+" + increase.ToString("N0");
            _incrementVisibleCountdown = _instance._incrementVisible;
            _incrementTime = Time.time;

        }

        #region game modes

        public static void WorkShopEntered(int entriesNeeded)
        {
            _workshopCountdown = entriesNeeded;

            //Since workshop reduces resources, this is triggered by the pinball manager.
            //UpdateModeInfo();

        }

        public static void StartSampoMode()
        {
            _sampo = true;
            _launch = false;
            _collect = false;
            UpdateModeInfo();
        }

        public static void EndSampoMode()
        {
            _sampo = false;
            _collect = false;
            UpdateModeInfo();
        }

        public static void StartCollect()
        {
            _collect = true;
            UpdateModeInfo();
        }

        public static void EndCollect()
        {
            _collect = false;
            UpdateModeInfo();
        }

        public static void StartKH()
        {
            _kanteleHero = true;
            _launch = false;
            UpdateModeInfo();

        }

        public static void EndKH()
        {
            _kanteleHero = false;

            UpdateModeInfo();
        }

        public static void StartLaunch()
        {
            _launch = true;
            UpdateModeInfo();
            
        }

        public static void EndLaunch()
        {
            _launch = false;
            UpdateModeInfo();
        }

        public static void UpdateModeInfo()
        {

            _instance._bumberIndicator.Switch(false);
            _instance._workshopIndicator.Switch(false);


            if (_launch)
            {
                DisplayModeInfo("Launch the ball to start.");
                return;
            }

            if(_kanteleHero)
            {
                DisplayModeInfo("Playing Kantele Hero.");
                return;
            }

            if (_collect)
            {
                DisplayModeInfo("Collect products!");
                return;
            }

            if (_sampo)
            {
                DisplayModeInfo("Spin Sampo spinner.");
                return;
            }
            
            if(PinballManager.Instance.Resources < PinballManager.Instance.MaxResources / 3)
            {
                DisplayModeInfo("Hit spruce bumbers for resources.");
                _instance._bumberIndicator.Switch(true);
            }
            else
            {
               
                DisplayModeInfo("Enter workshop to forge Sampo.");
                _instance._workshopIndicator.Switch(true);
            }
        }

        private static void DisplayModeInfo(string info)
        {
            _instance._gameMode.text = info;
            _instance._showMode = true;
        }


        #endregion

        public static void FormatScoreIncrement(int score, String message)
        {
            _instance._incrementUGUI.text = "+" + score.ToString("N0");
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

        public static void OutOfBalls()
        {
            //string translation = L10n.CurrentLanguage.GetTranslation(OutOfBallsKey);
            _instance._ballCounter.text = "Out of balls";
        }

        // Added a proper method for restting this. It was needed but did not exist, oops.
        internal static void ResetScore()
        {
            _instance._score = "0";
            _instance._scoreUGUI.text = _instance._score;
            _instance._scoreUGUI.fontSize = 4;
            _instance._smallScore.text = _instance._score;
            _instance._showMode = false;
            _incrementVisibleCountdown = 0;
            _displayModeCountdown = 2f;


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