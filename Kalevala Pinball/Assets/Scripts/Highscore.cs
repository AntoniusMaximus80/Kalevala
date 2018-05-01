using UnityEngine;
using Kalevala.Persistence;

namespace Kalevala
{
    public class Highscore
    {
        private int _id = -1;
        public string _playerName;
        public int _score;

        public int ID
        {
            get { return _id; }
            private set { _id = value; }
        }

        public Highscore(int id)
        {
            ID = id;
        }

        public HighscoreData GetHighscoreData()
        {
            return new HighscoreData
            {
                PlayerName = _playerName,
                Score = _score,
                ID = ID
            };
        }

        public void SetHighscoreData(HighscoreData data)
        {
            _playerName = data.PlayerName;
            _score = data.Score;
            ID = data.ID;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1:N0}", _playerName, _score);
            //return _playerName + ": " + _score;
        }
    }
}
