using UnityEngine;
using Kalevala.Persistence;

namespace Kalevala
{
    public class Highscore
    {
        private int _id = -1;
        public string PlayerName;
        public int Score;

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
                PlayerName = PlayerName,
                Score = Score,
                ID = ID
            };
        }

        public void SetHighscoreData(HighscoreData data)
        {
            PlayerName = data.PlayerName;
            Score = data.Score;
            ID = data.ID;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1:N0}", PlayerName, Score);
            //return _playerName + ": " + _score;
        }
    }
}
