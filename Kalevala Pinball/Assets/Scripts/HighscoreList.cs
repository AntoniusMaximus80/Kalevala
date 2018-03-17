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

        [SerializeField, Range(5, 30)]
        private int _listSize = 10;

        private Highscore[] _highscores;

        private void Start()
        {
            _highscores = new Highscore[_listSize];
            ResetList();
            LoadHighscores();
            UpdateScoreboard();
        }

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

        public void SaveHighscore(string playerName, int score)
        {
            for (int i = 0; i < _highscores.Length; i++)
            {
                if (score > _highscores[i].score)
                {
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
            Text[] scoreSlots = _scoreboard.GetComponentsInChildren<Text>();

            // TODO: Add or remove score slots in the scoreboard if needed

            for (int i = 0; i < scoreSlots.Length; i++)
            {
                scoreSlots[i].text = (i + 1) + ". " + _highscores[i].ToString();
            }
        }

        private void ResetList()
        {
            for (int i = 0; i < _highscores.Length; i++)
            {
                _highscores[i] = new Highscore();
                _highscores[i].playerName = _DEFAULT_PLAYER_NAME;
                _highscores[i].score = _DEFAULT_SCORE;
            }
        }
    }
}
