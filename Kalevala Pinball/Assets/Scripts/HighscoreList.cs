using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kalevala
{
    public class HighscoreList : MonoBehaviour
    {
        private const string _DEFAULT_PLAYER_NAME = "Creativium";
        private const int _DEFAULT_SCORE = 0;

        [SerializeField]
        private GameObject _scoreboard;

        [SerializeField]
        private Text _scoreSlotPrefab;

        [SerializeField, Range(5, 30)]
        private int _listSize = 10;

        private Highscore[] _highscores;
        private List<Text> _scoreSlots;

        private void Start()
        {
            _highscores = new Highscore[_listSize];
            _scoreSlots = new List<Text>(_listSize);

            if (_scoreboard != null)
            {
                ResetList();
                LoadHighscores();
                UpdateScoreboard();
            }
            else
            {
                Debug.LogError("Scoreboard is not set.");
            }
        }

        private string GetHighscoreText(Highscore highscore, int placement)
        {
            return placement + ". " + highscore.ToString();
        }

        /// <summary>
        /// Is the scoreboard visible.
        /// </summary>
        public bool Visible
        {
            get
            {
                return _scoreboard.activeSelf;
            }
            set
            {
                _scoreboard.SetActive(value);
            }
        }

        /// <summary>
        /// Compares the given score to the highscores and if it is
        /// higher than at least one of them, it is added to the list.
        /// </summary>
        /// <param name="playerName">The scoring player's name</param>
        /// <param name="score">The score being compared</param>
        /// <returns>Is the score a new highscore</returns>
        public bool CompareScoreAndSave(string playerName, int score)
        {
            bool isHighscore = false;

            for (int i = 0; i < _highscores.Length; i++)
            {
                // Checks if the score is higher than a
                // previous highscore and if so, saves it
                if (score > _highscores[i].score)
                {
                    isHighscore = true;

                    // Adds the new highscore to the slot at the index
                    // and moves lower scores down on the list
                    AddScore(playerName, score, i);

                    // Saves all highscores to a file
                    SaveHighscores();

                    // Updates the scoreboard
                    UpdateScoreboard();

                    break;
                }
            }

            return isHighscore;
        }

        /// <summary>
        /// Adds a new highscore to the slot at the index
        /// and moves lower scores down on the list.
        /// </summary>
        /// <param name="playerName">The scoring player's name</param>
        /// <param name="score">A new highscore</param>
        /// <param name="index">The highscore's index</param>
        private void AddScore(string playerName, int score, int index)
        {
            for (int i = _highscores.Length - 1; i >= index; i--)
            {
                // Moves a lower score down in the list
                if (i > index)
                {
                    _highscores[i].playerName = _highscores[i - 1].playerName;
                    _highscores[i].score = _highscores[i - 1].score;
                }
                // Records the new highscore to the slot at the index
                else
                {
                    _highscores[i].playerName = playerName;
                    _highscores[i].score = score;
                }
            }

            string debugMsg = string.Format
                ("New highscore ({0}): {1}", (index + 1), score);
            Debug.Log(debugMsg);
        }

        private void UpdateScoreboard()
        {
            for (int i = 0; i < _scoreSlots.Count; i++)
            {
                _scoreSlots[i].text = GetHighscoreText(_highscores[i], i + 1);
            }
        }

        public void SaveHighscores()
        {
            // TODO
        }

        public void LoadHighscores()
        {
            // TODO

            // If the saved list has a different length than
            // the current list, either some scores get dropped
            // or empty slots are filled with default values
        }

        private void ResetList()
        {
            for (int i = 0; i < _highscores.Length; i++)
            {
                _highscores[i] = new Highscore();
                _highscores[i].playerName = _DEFAULT_PLAYER_NAME;
                _highscores[i].score = _DEFAULT_SCORE;
            }

            if (_scoreSlotPrefab != null)
            {
                // Gets every score slot text found in the
                // scoreboard and puts them in an array
                Text[] scoreSlotArray =
                    _scoreboard.GetComponentsInChildren<Text>();

                // Destroys all existing score slots
                _scoreSlots.Clear();
                for (int i = scoreSlotArray.Length - 1; i >= 0; i--)
                {
                    Destroy(scoreSlotArray[i].gameObject);
                }

                // Creates as many score slots as the highscore
                // list needs and adds them to the score slot list
                for (int scoreSlotIndex = 0;
                     scoreSlotIndex < _listSize;
                     scoreSlotIndex++)
                {
                    Text newScoreSlot =
                        Instantiate(_scoreSlotPrefab, _scoreboard.transform);

                    _scoreSlots.Add(newScoreSlot);

                    newScoreSlot.name = string.Format
                        ("Highscore{0}", (scoreSlotIndex + 1).ToString("D2"));
                }
            }
        }
    }
}
