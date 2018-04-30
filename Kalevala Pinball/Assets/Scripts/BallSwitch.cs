using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class BallSwitch : Shot
    {
        [SerializeField]
        private Vector3 _switchDirection;

        [SerializeField]
        private float _distanceTolerance = 0.2f;

        [SerializeField]
        private float _directionAngleTolerance = 30f;

        [SerializeField]
        private float _restTime = 1f;

        [SerializeField]
        private Color _gizmosColor = Color.blue;

        private bool _resting;
        private float _elapsedRestTime;

        public Vector3 SwitchDirection
        {
            get
            {
                return _switchDirection;
            }
            set
            {
                _switchDirection = value;
            }
        }

        public override int RepeatActivations { get; set; }

        private void Update()
        {
            if (_resting)
            {
                UpdateRest();
            }
            else
            {
                CheckActivation();
            }
        }

        private void UpdateRest()
        {
            _elapsedRestTime += Time.deltaTime;
            if (_elapsedRestTime >= _restTime)
            {
                EndRest();
            }
        }

        private void StartRest()
        {
            _resting = true;
            _elapsedRestTime = 0;
        }

        private void EndRest()
        {
            _resting = false;
        }

        /// <summary>
        /// Checks whether a pinball hits the switch while
        /// going in the right direction.
        /// </summary>
        public override bool CheckActivation()
        {
            if ( !_resting )
            {
                foreach (Pinball ball in PinballManager.Instance.Pinballs)
                {
                    if (Hit(ball.transform.position) &&
                        SameDirections(ball.PhysicsVelocity))

                    {
                        //Debug.Log("[" + name + "] Pinball detected");
                        StartRest();
                        return true;
                    }
                }
            }

            return false;
        }

        public bool Hit(Vector3 objPosition)
        {
            bool result = false;

            if (Vector3.Distance(transform.position, objPosition)
                < _distanceTolerance)
            {
                result = true;
            }

            return result;
        }

        public bool SameDirections(Vector3 ballDirection)
        {
            bool result = false;

            if (_directionAngleTolerance >= 180 ||
                Vector3.Angle(ballDirection, _switchDirection)
                    <= _directionAngleTolerance)
            {
                result = true;
            }

            return result;
        }

        public override void ResetShot()
        {
            EndRest();
            RepeatActivations = 0;
        }

        private void OnDrawGizmos()
        {
            DrawPosition();
            DrawDirection();
        }

        private void DrawPosition()
        {
            Gizmos.color = _gizmosColor;

            if (RepeatActivations > 0)
            {
                Gizmos.DrawSphere(
                    transform.position + new Vector3(0, _distanceTolerance + 2f, 0),
                    1f);
            }

            if (_resting)
            {
                Gizmos.color = Color.black;
            }

            Gizmos.DrawWireSphere(transform.position, _distanceTolerance);
        }

        private void DrawDirection()
        {
            Gizmos.color = _gizmosColor;
            Gizmos.DrawLine(transform.position,
                transform.position + _switchDirection);
        }
    }
}
