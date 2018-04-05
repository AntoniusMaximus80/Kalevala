using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Kalevala
{
    public class Viewscreen : MonoBehaviour
    {
        public StateManager _stateManager;

        // references to the viewscreen texts.
        public TextMeshProUGUI
            _scoreUGUI,
            _incrementUGUI,
            _ballCounter,
            _smallScore,
            _gameMode;

        public float _incrementVisible = 5f;

        private static float _incrementVisibleCountdown;

        private Color _fade = new Color(1f, 1f, 1f, 1f);

        private static Viewscreen _instance;

        
        // Use this for initialization
        void Awake()
        {
            _instance = this;
            _incrementVisibleCountdown = _incrementVisible;
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
            }
        }

        public static void FormatScore(int score)
        {
            _instance._scoreUGUI.text = score.ToString("N0");
            _instance._smallScore.text = score.ToString("N0");
        }

        public static void FormatScoreIncrement(int score)
        {
            _instance._incrementUGUI.text = score.ToString("N0");
            _incrementVisibleCountdown = _instance._incrementVisible;
        }

        public static void BallCount(int balls)
        {
            _instance._ballCounter.text = "Balls left: " + balls.ToString("N0");
        }
    }
}