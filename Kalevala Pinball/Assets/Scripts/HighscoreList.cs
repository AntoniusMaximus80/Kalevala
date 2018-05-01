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
        private const string _EMPTY_PLAYER_NAME = "Empty";
        private const int _DEFAULT_SCORE = 0;

        [SerializeField]
        private GameObject _scoreboard;

        [SerializeField]
        private Text _scoreSlotPrefab;

        [SerializeField, Range(3, 30)]
        private int _listSize = 10;

        [SerializeField, Range(0, 3)]
        private float _scoreSlotMoveDuration = 1f;

        [SerializeField]
        private Color _defaultScoreColor = Color.white;

        [SerializeField]
        private Color _currentScoreColor = Color.green;

        private Highscore[] _highscores;
        private List<Text> _scoreSlots;

        private string _currentPlayerName;
        private int _currentRank = -1;

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

            // Gets the ranking of the score
            // (-1 if it isn't high enough to be on the scoreboard)
            int highscoreIndex = CompareScore(score);

            // Saves the score if it is higher than a previous highscore
            if (highscoreIndex > -1)
            {
                isHighscore = true;

                // Adds the new highscore to the slot at the index
                // and moves lower scores down on the list
                AddScore(playerName, score, highscoreIndex);

                // Saves all highscores to a file
                GameManager.Instance.SaveGame("Saving new highscore");

                // Updates the scoreboard
                UpdateScoreboard();
            }

            return isHighscore;
        }

        public bool SaveHighscore()
        {
            if (_currentRank != -1)
            {
                // Saves all highscores to a file
                GameManager.Instance.SaveGame("Saving new highscore");
                return true;
            }

            return false;
        }

        public bool RevertHighscores()
        {
            // TODO

            return false;
        }

        private int CompareScore(int score)
        {
            int highscoreRank = -1;

            // Checks if the score is higher than an existing highscore
            for (int i = 0; i < _highscores.Length; i++)
            {
                // If the score is higher, it takes the
                // rank of the surpassed score 
                if (score > _highscores[i]._score)
                {
                    highscoreRank = i;
                    break;
                }
            }

            return highscoreRank;
        }

        private int CompareScoreWithNextRanks(int score)
        {
            int highscoreRank = _currentRank;

            // No need to check if the top rank has already been reached
            if (_currentRank != 0)
            {
                //int nextRank;

                //// Targeting the lowest rank on the scoreboard
                //if (_currentRank == -1)
                //{
                //    nextRank = _highscores.Length - 1;
                //}
                //// Targeting the next rank above the current one
                //else
                //{
                //    nextRank = _currentRank - 1;
                //}

                //// Checks if the score high enough to get the next rank
                //if (score > _highscores[nextRank]._score)
                //{
                //    highscoreRank = nextRank;
                //}

                int rankLimit;
                if (_currentRank == -1)
                {
                    rankLimit = _highscores.Length;
                }
                else
                {
                    rankLimit = _currentRank;
                }

                // Checks if the score is higher than an existing highscore
                for (int i = 0; i < rankLimit; i++)
                {
                    // If the score is higher, it takes the
                    // rank of the surpassed score 
                    if (score > _highscores[i]._score)
                    {
                        highscoreRank = i;
                        break;
                    }
                }
            }

            return highscoreRank;
        }

        /// <summary>
        /// Adds a new highscore to the slot at the index
        /// and moves lower scores down on the list.
        /// </summary>
        /// <param name="playerName">The scoring player's name</param>
        /// <param name="score">A new highscore</param>
        /// <param name="rank">The highscore's rank</param>
        private void AddScore(string playerName, int score, int rank)
        {
            for (int i = _highscores.Length - 1; i >= rank; i--)
            {
                // Moves a lower score down in the list
                if (i > rank)
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
                ("New highscore ({0}): {1}", (rank + 1), score);
            Debug.Log(debugMsg);
        }

        /// <summary>
        /// Edits a highscore in the slot at the index
        /// </summary>
        /// <param name="playerName">The scoring player's name</param>
        /// <param name="score">A highscore</param>
        /// <param name="index">The highscore's index</param>
        private void EditScore(string playerName, int score, int index)
        {
            _highscores[index]._playerName = playerName;
            _highscores[index]._score = score;
        }

        /// <summary>
        /// Swaps two highscores' slots.
        /// </summary>
        /// <param name="rank">A highscore's index</param>
        /// <param name="swapUp">Should the score swap places
        /// with the score above it</param>
        private void SwapScoreSlots(int rank, bool swapUp)
        {
            // Stops invalid swaps
            if (rank == -1 ||
                (rank == 0 && swapUp) ||
                (rank == _highscores.Length - 1 && !swapUp))
            {
                return;
            }

            int otherRank = rank + (swapUp ? -1 : 1);

            string tempName = _highscores[otherRank]._playerName;
            int tempRank = _highscores[otherRank]._score;
            _highscores[otherRank]._playerName = _highscores[rank]._playerName;
            _highscores[otherRank]._score = _highscores[rank]._score;
            _highscores[rank]._playerName = tempName;
            _highscores[rank]._score = tempRank;
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

            float elapsedTime = 0;
            float ratio = 0;

            while (ratio < 1.0)
            {
                if (_scoreSlotMoveDuration <= 0)
                {
                    ratio = 1;
                }
                else
                {
                    ratio = elapsedTime / _scoreSlotMoveDuration;
                }

                elapsedTime += Time.unscaledDeltaTime;
               
                newPos.y = Mathf.Lerp(startY, targetY, ratio);
                scoreSlotTransform.anchoredPosition = newPos;

                yield return null;
            }

            newPos.y = targetY;
            scoreSlotTransform.anchoredPosition = newPos;
        }

        public void UpdateCurrentRanking(int currentScore)
        {
            // Gets the ranking of the score
            int rank = CompareScoreWithNextRanks(currentScore);

            if (rank != _currentRank)
            {
                if (_currentRank == -1)
                {
                    // Adds the new highscore to the slot at the index
                    // and moves lower scores down on the list
                    AddScore(_currentPlayerName,
                        currentScore, rank);
                }
                else
                {
                    // FIXME: Sometimes a name gets duplicated and another one is lost
                    SwapScoreSlots(_currentRank, true);
                    _scoreSlots[_currentRank].color = _defaultScoreColor;
                }

                _currentRank = rank;
                _scoreSlots[_currentRank].color = _currentScoreColor;
            }
            else
            {
                EditScore(_currentPlayerName,
                    currentScore, _currentRank);
            }

            // Updates the scoreboard
            UpdateScoreboard();
        }

        public void UpdateScoreboard()
        {
            for (int i = 0; i < _scoreSlots.Count; i++)
            {
                _scoreSlots[i].text =
                    GetHighscoreText(_highscores[i], i + 1);
            }
        }

        public void FetchHighscoreData(GameData data)
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
                GameManager.Instance.SaveGame("Saving default scores");
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

        public void ResetCurrentRanking()
        {
            _currentPlayerName = GameManager.Instance.PlayerName;
            _currentRank = -1;

            foreach (Text scoreText in _scoreSlots)
            {
                scoreText.color = _defaultScoreColor;
            }
        }

        public void ResetList()
        {
            for (int i = 0; i < _highscores.Length; i++)
            {
                _highscores[i] = new Highscore(i);
                _highscores[i]._playerName = _EMPTY_PLAYER_NAME;
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

        private string GetHighscoreText(Highscore highscore, int placement)
        {
            string hsText = string.Format("{0,3} {1}", (placement + "."), highscore.ToString());

            // Adds a space in the beginning of any string
            // where the placement is in the single digits
            //if (placement < 10)
            //{
            //    hsText = " " + hsText;
            //}

            return hsText;
        }
    }
}
