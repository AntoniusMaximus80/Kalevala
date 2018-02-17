﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kalevala.WaypointSystem;

namespace Kalevala
{
    public class RampEntrance : MonoBehaviour
    {
        [SerializeField]
        private Path _path;

        [SerializeField]
        private bool _isPathStart = true;

        [SerializeField]
        private bool _dropBallAtEnd;

        [SerializeField]
        private Vector3 _rampDirection;

        [SerializeField]
        private float _distanceTolerance = 0.2f;

        [SerializeField]
        private float _directionAngleTolerance = 30;

        [SerializeField]
        private Color _gizmosColor = Color.blue;

        private Direction _direction;
        private Waypoint _startWaypoint;

        /* The ramp entrance knows the ramp it is a part of
         * When a pinball hits, the ramp is given to it and it disables its physics
         * The pinball moves itself on the ramp using its speed, the ramp's line and gravity
         * The pinball either changes direction midway or reaches the end of the ramp
         * When the pinball should exit the ramp, it forgets the ramp, enables its physics
           and adds force to itself based on the speed it had on the ramp
         */

        private void Start()
        {
            _rampDirection.Normalize();

            if (_isPathStart)
            {
                _direction = Direction.Forward;
                _startWaypoint = _path.GetFirstWaypoint();
            }
            else
            {
                _direction = Direction.Backward;
                _startWaypoint = _path.GetLastWaypoint();
            }
        }

        private void Update()
        {
            PutPinballOnRamp();
        }

        public bool IsPathStart
        {
            get
            {
                return _isPathStart;
            }
        }

        public Vector3 RampDirection
        {
            get
            {
                return _rampDirection;
            }
            set
            {
                _rampDirection = value;
            }
        }

        /// <summary>
        /// Makes a pinball that hits the ramp entrance
        /// and goes in the right direction enter the ramp.
        /// </summary>
        private void PutPinballOnRamp()
        {
            foreach (Pinball ball in PinballManager.Instance.Pinballs)
            {
                if (!ball.IsOnRamp &&
                    Hit(ball.transform.position) &&
                    SameDirections(ball.PhysicsVelocity))
                {
                    ball.EnterRamp(_path, _direction,
                        _startWaypoint, _dropBallAtEnd);
                    return;
                }
            }
        }

        public bool Hit(Vector3 objPosition)
        {
            bool result = false;

            if (Vector3.Distance(transform.position, objPosition) < _distanceTolerance)
            {
                result = true;
            }

            return result;
        }

        public bool SameDirections(Vector3 ballDirection)
        {
            bool result = false;

            if (_directionAngleTolerance >= 180 ||
                Vector3.Angle(ballDirection, _rampDirection)
                    <= _directionAngleTolerance)
            {
                result = true;
            }

            return result;
        }

        public Color GizmosColor
        {
            get
            {
                return _gizmosColor;
            }
            set
            {
                _gizmosColor = value;
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                if (_path.PathColor != _gizmosColor)
                {
                    if (_isPathStart)
                    {
                        _path.PathColor = _gizmosColor;
                    }
                    else
                    {
                        _gizmosColor = _path.PathColor;
                    }
                }
            }

            DrawStartPoint();
            DrawDirection();
        }

        private void DrawStartPoint()
        {
            Gizmos.color = _gizmosColor;
            Gizmos.DrawWireSphere(transform.position, _distanceTolerance);
        }

        private void DrawDirection()
        {
            Gizmos.color = _gizmosColor;
            Gizmos.DrawLine(transform.position,
                transform.position + _rampDirection);
        }
    }
}
