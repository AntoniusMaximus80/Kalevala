using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Kalevala
{
    public class Scorekeeper : MonoBehaviour
    {
        public int _score;
        public TextMeshProUGUI _scoreUGUI,
            _incrementUGUI;
        public float _incrementVisible;
        private float _incrementVisibleCountdown;
        private Color _fade;
        /*private float _testingDelay = 1f,
            _testingDelayCountup = 0f;*/

        private void Start()
        {
            _score = 0;
            _incrementVisibleCountdown = _incrementVisible;
            _fade = new Color(1f, 1f, 1f, 1f);
        }

        private void Update()
        {
            /*_testingDelayCountup += Time.deltaTime;
            if (_testingDelayCountup > _testingDelay)
            {
                _testingDelayCountup = 0f;
                AddScore(150000);
            }*/

            if (_incrementVisibleCountdown != 0f)
            {
                _incrementVisibleCountdown -=  Time.deltaTime;
                Mathf.Clamp01(_incrementVisibleCountdown);
                float ratio = _incrementVisibleCountdown / _incrementVisible;
                _fade.a = ratio;
                _incrementUGUI.color = _fade;
            }
        }

        public void AddScore(int amount)
        {
            _score += amount;
            _scoreUGUI.text = FormatScore();
            _incrementUGUI.text = FormatIncrement(amount);
            _incrementVisibleCountdown = _incrementVisible;
        }

        public string FormatScore()
        {
            string formattedScore = _score.ToString("N0");
            return formattedScore;
        }

        public string FormatIncrement(int amount)
        {
            string formattedIncrement = "+" + amount.ToString("N0");
            return formattedIncrement;
        }
    }
}