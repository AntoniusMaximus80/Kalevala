using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Kalevala
{
    public class Highscore
    {
        public string playerName;
        public int score;

        public override string ToString()
        {
            return playerName + ": " + score;
        }
    }
}
