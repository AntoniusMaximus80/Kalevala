using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Kalevala.Persistence;

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

        [SerializeField, Range(3, 30)]
        private int _listSize = 10;

        [SerializeField, Range(0, 3)]
        private float _scoreSlotMoveDuration = 1f;

        private Highscore[] _highscores;
        private List<Text> _scoreSlots;

        private void Start()
        {
            //InitScoreboard();
        }

        private void InitScoreboard()
        {
            _highscores = new Highscore[_listSize];
            _scoreSlots = new List<Text>(_listSize);

            if (_scoreboard != null)
            {
                ResetList();
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
                if (score > _highscores[i]._score)
                {
                    isHighscore = true;

                    // Adds the new highscore to the slot at the index
                    // and moves lower scores down on the list
                    AddScore(playerName, score, i);

                    // Saves all highscores to a file
                    GameManager.Instance.SaveGame();

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
                    _highscores[i]._playerName = _highscores[i - 1]._playerName;
                    _highscores[i]._score = _highscores[i - 1]._score;

                    //RectTransform belowScrSlotTr = _scoreSlots[i].GetComponent<RectTransform>();
                    //StartCoroutine(MoveScoreSlot(_scoreSlots[i - 1], belowScrSlotTr.anchoredPosition.y));
                }
                // Records the new highscore to the slot at the index
                else
                {
                    _highscores[i]._playerName = playerName;
                    _highscores[i]._score = score;
                }
            }

            string debugMsg = string.Format
                ("New highscore ({0}): {1}", (index + 1), score);
            Debug.Log(debugMsg);
        }

        /// <summary>
        /// A coroutine.
        /// Moves a highscore slot up or down towards the given Y-coordinate.
        /// The moving speed is determined by the serialized field
        /// _scoreSlotMoveDuration.
        /// </summary>
        /// <param name="scoreSlot">The text object that moves</param>
        /// <param name="targetY">The target Y-coordinate</param>
        /// <returns>Null</returns>
        private IEnumerator MoveScoreSlot(Text scoreSlot, float targetY)
        {
            RectTransform scoreSlotTransform =
                scoreSlot.GetComponent<RectTransform>();
            float startY = scoreSlotTransform.anchoredPosition.y;
            bool moveUp = (targetY > startY);
            Vector3 newPos = scoreSlotTransform.anchoredPosition;

            float startTime = Time.time;
            float ratio = 0;

            while ((moveUp && newPos.y < targetY) ||
                   (!moveUp && newPos.y > targetY))
            {
                ratio = (Time.time - startTime) / _scoreSlotMoveDuration;
                newPos.y = Mathf.Lerp(startY, targetY, ratio);
                scoreSlotTransform.anchoredPosition = newPos;
                yield return null;
            }

            newPos.y = targetY;
            scoreSlotTransform.anchoredPosition = newPos;
        }

        private void UpdateScoreboard()
        {
            for (int i = 0; i < _scoreSlots.Count; i++)
            {
                _scoreSlots[i].text = GetHighscoreText(_highscores[i], i + 1);
            }
        }

        public void FetchHighscoreData(ref GameData data)
        {
            foreach (Highscore highscore in _highscores)
            {
                data.HighscoreDataList.Add(highscore.GetHighscoreData());
            }
        }

        /// <summary>
        /// Loads highscores from data.
        /// </summary>
        /// <param name="data">Loaded game data</param>
        public void LoadHighscores(GameData data)
        {
            InitScoreboard();

            // No data to load;
            // saves default scores and returns
            if (data == null)
            {
                Debug.Log("Saving default scores");
                GameManager.Instance.SaveGame();
                return;
            }

            // Sets highscores taken from the loaded game data
            foreach (HighscoreData highscoreData in data.HighscoreDataList)
            {
                Highscore highscore = _highscores.
                    FirstOrDefault(hs => hs.ID == highscoreData.ID);

                if (highscore != null)
                {
                    highscore.SetHighscoreData(highscoreData);
                }
            }

            // TODO:
            // If the saved list has a different length than
            // the current list, either some scores get dropped
            // or empty slots are filled with default values

            UpdateScoreboard();
        }

        private void ResetList()
        {
            for (int i = 0; i < _highscores.Length; i++)
            {
                _highscores[i] = new Highscore(i);
                _highscores[i]._playerName = _DEFAULT_PLAYER_NAME;
                _highscores[i]._score = _DEFAULT_SCORE;
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
