using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class KanteleHeroPanel: MonoBehaviour
    {
        [SerializeField]
        private float _spawnInterval;
        [SerializeField]
        private float _lightMoveSpeed;
        public GameObject[] Waypoints;
        public GameObject _movingLight;
        // Use this for initialization
        void Start()
        {
            _movingLight.GetComponent<KanteleHeroLight>().Init(Waypoints, this, _lightMoveSpeed);
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Turns the left light on and 
        /// </summary>
        public void TurnLeftLightOn()
        {

        }
    }
}
