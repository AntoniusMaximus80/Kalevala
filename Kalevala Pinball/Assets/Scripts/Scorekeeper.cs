using System.Collections;
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
            // VN : remember that some score modifiers are type dependent and go inside the switch statement.

            _totalScore += _score;

            // Refactored the string processing to happen in the viewscreen class.
            Viewscreen.FormatScore(_totalScore);
            Viewscreen.FormatScoreIncrement(_score);

            
            //_incrementUGUI.text = FormatIncrement(_score);
            //_incrementVisibleCountdown = _incrementVisible;
        }

       

        public void ResetScore()
        {
            _totalScore = 0;
            Viewscreen.FormatScore(_totalScore);
            
        }
    }
}