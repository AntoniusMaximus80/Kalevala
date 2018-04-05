using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Kalevala
{
    public class Viewscreen : MonoBehaviour
    {
        public StateManager _stateManager;

        // references to the viewscreen texts.
        public TextMeshProUGUI
            _scoreUGUI,
            _incrementUGUI,
            _ballCounter,
            _smallScore,
            _gameMode;

        private Viewscreen _instance;

        
        // Use this for initialization
        void Awake()
        {
            _instance = this;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public static void FormatScore(int score)
        {

        }

        public static void FormatScoreIncrement(int score)
        {

        }
    }
}