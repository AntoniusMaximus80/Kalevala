using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kalevala.WaypointSystem;
using System;

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

        [SerializeField, Range(0f, 2f)]
        private float _rampEnterSpeedMultiplier = 1f;

        [SerializeField, Range(0.1f, 3f)]
        private float _rampGravityMultiplier = 1f;

        [SerializeField]
        private bool _useGlobalRampExitSpeedMult = true;

        [SerializeField]
        private Color _gizmosColor = Color.blue;

        private Direction _direction;
        private Waypoint _startWaypoint;

        [SerializeField]
        private KickoutHole _kickoutHole;

        private bool _active = true;
        private bool _isKickHole = false;
        public event Action BallEnteredRamp;

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
                //_kickoutHole = GetComponent<KickoutHole>();
                if(_kickoutHole != null)
                {
                    _isKickHole = true;
                }
                _direction = Direction.Forward;
                _startWaypoint = _path.GetFirstWaypoint();
            }
            else
            {
                //_kickoutHole = GetComponentInParent<KickoutHole>();
                _direction = Direction.Backward;
                _startWaypoint = _path.GetLastWaypoint();
            }
        }

        private void Update()
        {
            PutPinballOnRamp();
        }

        public bool Active
        {
            get
            {
                return _active;
            }
            set
            {
                _active = value;
            }
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
        /// Puts a pinball on the ramp if it hits the ramp
        /// entrance and goes in the right direction.
        /// </summary>
        private void PutPinballOnRamp()
        {
            foreach (Pinball ball in PinballManager.Instance.Pinballs)
            {
                if (Active && !ball.IsOnRamp && !ball.IsInKickoutHole &&
                    Hit(ball.transform.position) &&
                    SameDirections(ball.PhysicsVelocity) )

                {
                    if(BallEnteredRamp != null)
                    {
                        BallEnteredRamp();
                    }
                    bool available = true;
                    float kickforce = 0;
                    // Checks if either end of the ramp is attached to a kickouthole
                    if(_kickoutHole != null)
                    {
                        // Checks if the path is not starting path and has a reference to kickouthole component attached
                        if(!_isKickHole)
                        {
                            kickforce = _kickoutHole.KickForce;
                        }
                        // otherwise if the path is path start entrance and has a kickouthole component attached
                        else
                        {
                            available = _kickoutHole.BallIncoming(ball, this);
                        }
                    }
                    if(available)
                    {
                        ball.EnterRamp(_path, _direction, _startWaypoint,
                            _rampEnterSpeedMultiplier, _rampGravityMultiplier,
                            _dropBallAtEnd, _useGlobalRampExitSpeedMult, _kickoutHole);
                    }
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
            if (Active)
            {
                Gizmos.color = _gizmosColor;
                Gizmos.DrawWireSphere(transform.position, _distanceTolerance);
            }
        }

        private void DrawDirection()
        {
            if (Active)
            {
                Gizmos.color = _gizmosColor;
                Gizmos.DrawLine(transform.position,
                    transform.position + _rampDirection);
            }
        }
    }
}
