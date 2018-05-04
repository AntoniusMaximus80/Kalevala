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
            Skillshot,
            CollectableGrain,
            CollectableSalt,
            CollectableGold,
            KanteleBumper,
            KanteleLight
        }

        public int _totalScore,
            _spruceBumper,
            _slightshot,
            _spinner,
            _dropTarget,
            _ilmarinenKOH,
            _teleportKOH,
            _tuonelaRollover,
            _skillshot,
            _collectableGrain,
            _collectableSalt,
            _collectableGold,
            _kanteleBumper,
            _kanteleLight
           ;

        private HighscoreList _highscores;

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
            _highscores = GameManager.Instance.HighscoreList;
            ResetScore();
        }


        public void AddScore(ScoreType scoreType, GameObject scoreObject = null)
        {
            if(GameManager.Instance.Screen.State != ScreenStateType.Play)
            {
                return;
            }
            int _score = 0;
            string _message = "";
            switch(scoreType)
            {
                case ScoreType.PopBumper:
                    _score += _spruceBumper;
                    _message = "Spruce bumber hit for {0} points.";
                    break;
                case ScoreType.Slingshot:
                    _score += _slightshot;
                    _message = "Slingshot hit for {0} points.";
                    break;
                case ScoreType.Spinner:
                    _score += _spinner;
                    _message = "Spinner gave you {0} points.";
                    break;
                case ScoreType.DropTarget:
                    _score += _dropTarget;
                    _message = "{0} points from the drop target.";
                    break;
                case ScoreType.IlmarinenKOH:
                    _score += _ilmarinenKOH;
                    _message = "The workshop gave {0} points.";
                    break;
                case ScoreType.TeleportKOH:
                    _score += _teleportKOH;
                    _message = "{0} points from the teleporter.";
                    break;
                case ScoreType.TuonelaRollover:
                    _score += _tuonelaRollover * Rollover.ScoreMultiplier;
                    _message = "Tuonela rollovers gave {0} points.";
                    break;
                case ScoreType.Skillshot:
                    _score += _skillshot;
                    _message = "Skill shot made for {0} points.";
                    break;
                case ScoreType.CollectableGrain:
                    _score += _collectableGrain;
                    _message = "{0} points from grain.";
                    break;
                case ScoreType.CollectableSalt:
                    _score += _collectableSalt;
                    _message = "{0} points from salt.";
                    break;
                case ScoreType.CollectableGold:
                    _score += _collectableGold;
                    _message = "{0} points from gold.";
                    break;
                case ScoreType.KanteleBumper:
                    _score += _kanteleBumper;
                    _message = "Kantele bumber hit for {0} points.";
                    break;
                case ScoreType.KanteleLight:
                    _score += _kanteleLight;
                    _message = "Kantele light gave {0} points.";
                    break;
                default:
                    Debug.LogError("ScoreType not recognized.");
                    _message = "Something broke. No score.";
                    break;
            }

            //Check if it was an objective
            if(scoreObject != null)
            {
                ObjectiveManager.Instance.CheckObjective(scoreObject);
            }

            // To do: Apply possible score modifiers here.
            // VN : remember that some score modifiers are type dependent and go inside the switch statement.
            _score = _score * ComboManager.Instance.ScoreMultiplier;
           _totalScore += _score;
            // Refactored the string processing to happen in the viewscreen class.
            // Why was I calling two functions that are always used together separately?
            Viewscreen.FormatScore(_totalScore, _score, _message);
            //Viewscreen.FormatScoreIncrement();

            _highscores.UpdateCurrentRanking(_totalScore);
        }

       

        public void ResetScore()
        {
            _totalScore = 0;
            Viewscreen.ResetScore();
            
        }
    }
}