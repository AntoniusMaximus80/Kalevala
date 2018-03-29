﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Kalevala
{
    public class Scorekeeper : MonoBehaviour
    {
        public enum ScoreType
        {
            PopBumper,
            Slingshot,
            Spinner,
            DropTarget,
            IlmarinenKOH,
            TeleportKOH,
            TuonelaRollover,
            Skillshot
        }

        public int _totalScore,
            _spruceBumper,
            _kanteleBumper,
            _spinner,
            _dropTarget,
            _ilmarinenKOH,
            _teleportKOH,
            _tuonelaRollover,
            _skillshot
           ;

        public TextMeshProUGUI _scoreUGUI,
            _incrementUGUI;

        public float _incrementVisible;

        private float _incrementVisibleCountdown;

        private Color _fade;

        private static Scorekeeper _instance;

        public static Scorekeeper Instance
        {
            get { return _instance; }
        }

        private void Awake()
        {
            if(Instance == null)
            {
                _instance = this;
            } else
            {
                Destroy(this.gameObject);
            }
        }

        private void Start()
        {
            ResetScore();
            _incrementVisibleCountdown = _incrementVisible;
            _fade = new Color(1f, 1f, 1f, 1f);
        }

        private void Update()
        {
            if (_incrementVisibleCountdown != 0f)
            {
                _incrementVisibleCountdown -=  Time.deltaTime;
                Mathf.Clamp01(_incrementVisibleCountdown);
                float ratio = _incrementVisibleCountdown / _incrementVisible;
                _fade.a = ratio;
                _incrementUGUI.color = _fade;
            }
        }

        public void AddScore(ScoreType scoreType)
        {
            int _score = 0;
            switch(scoreType)
            {
                case ScoreType.PopBumper:
                    _score += _spruceBumper;
                    break;
                case ScoreType.Slingshot:
                    _score += _kanteleBumper;
                    break;
                case ScoreType.Spinner:
                    _score += _spinner;
                    break;
                case ScoreType.DropTarget:
                    _score += _dropTarget;
                    break;
                case ScoreType.IlmarinenKOH:
                    _score += _ilmarinenKOH;
                    break;
                case ScoreType.TeleportKOH:
                    _score += _teleportKOH;
                    break;
                case ScoreType.TuonelaRollover:
                    _score += _tuonelaRollover * Rollover.ScoreMultiplier;
                    break;
                case ScoreType.Skillshot:
                    _score += _skillshot;
                    break;
                default:
                    Debug.LogError("ScoreType not recognized.");
                    break;
            }

            // To do: Apply possible score modifiers here.

            _totalScore += _score;
            _scoreUGUI.text = FormatScore();
            _incrementUGUI.text = FormatIncrement(_score);
            _incrementVisibleCountdown = _incrementVisible;
        }

        public string FormatScore()
        {
            string formattedScore = _totalScore.ToString("N0");
            return formattedScore;
        }

        public string FormatIncrement(int amount)
        {
            string formattedIncrement = "+" + amount.ToString("N0");
            return formattedIncrement;
        }

        public void ResetScore()
        {
            _totalScore = 0;
            _scoreUGUI.text = FormatScore();
        }
    }
}