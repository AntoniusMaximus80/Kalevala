using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class Rollover : MonoBehaviour
    {

        public Transform wire;

        public PlayfieldLight _light;

        public int order;

        public float downPositionZ = -.5f;

        public float duration = .5f;

        private bool _down;

        private Vector3 _targetPosition = Vector3.zero;

        private float _elapsedTime;

        private static Rollover[] _instances = new Rollover[7];

        private static AudioSource _leftSound, _rightSound;

        private static int _count, _completedCount;


        public static bool IsComplete
        {
            get
            {
                return _count == 7;
            }
        }

        public static int ScoreMultiplier
        {
            get
            {
                return _count + _completedCount;
            }
        }

        // Use this for initialization
        void Awake()
        {

            Init();

            _instances[order] = this;

            if (_leftSound == null)
            {
                _leftSound = GameObject.Find("RolloverLeftAudio").GetComponent<AudioSource>();
                _rightSound = GameObject.Find("RolloverRightAudio").GetComponent<AudioSource>();
            }

        }

        internal static void Reset()
        {
            foreach (Rollover r in _instances)
            {
                r.Init();
            }
        }

        private void Init()
        {
            // Start having moved, might be a good idea to actually move the wires at start instead.
            _elapsedTime = duration;

            _down = false;

            // The light should start off.
            _light.TurnOff();

            _count = 0;
        }

        // Update is called once per frame
        void Update()
        {

            if (_elapsedTime > duration) return;

            _elapsedTime += Time.deltaTime;

            // Relative height (z) the wire should have in its current state.
            _targetPosition.z = _down ? downPositionZ : 0f;

            wire.localPosition = Vector3.Lerp(wire.localPosition, _targetPosition, _elapsedTime / duration);

        }

        private void OnTriggerEnter(Collider collider)
        {

            // Was already down.
            if (_down) return;

            _down = true;
            _light.TurnOn();
            _elapsedTime = duration - _elapsedTime;

            (transform.position.x < 0 ? _leftSound : _rightSound).Play();

            Scorekeeper.Instance.AddScore(Scorekeeper.ScoreType.TuonelaRollover);

            _count++;

            if (IsComplete)
            {
                PinballManager.Instance.ShootAgain = true;

                foreach (Rollover r in _instances)
                {
                    r.SetValue(false);
                }

                _count = 0;
                _completedCount++;

            }

        }

        public static void RollLeft()
        {
            bool[] current = new bool[7];
            foreach (Rollover r in _instances)
            {
                current[r.order] = r._down;
            }

            foreach (Rollover r in _instances)
            {
                r.SetValue(current[(r.order + 1) % 7]);
            }
        }

        public static void RollRight()
        {
            bool[] current = new bool[7];
            foreach (Rollover r in _instances)
            {
                current[r.order] = r._down;
            }

            foreach (Rollover r in _instances)
            {
                r.SetValue(current[(r.order == 0 ? 6 : r.order - 1)]);
            }
        }

        private void SetValue(bool newValue)
        {
            if (newValue != _down)
            {
                _elapsedTime = duration - _elapsedTime;
            }

            _down = newValue;

            if (_down)
            {
                _light.TurnOn();
            }
            else
            {
                _light.TurnOff();
            }
        }

    }
}
