using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kalevala.WaypointSystem;

namespace Kalevala
{
    public class RampMotion : MonoBehaviour
    {
        [SerializeField]
        private float _gravity = -1f;

        //[SerializeField]
        //private float _eventTime = 1;

        [SerializeField, Range(0, 1)]
        private float _directionalRatio;

        private bool _onRamp;

        public float _speed;
        private float _segmentTime;

        private Path _path;
        private Waypoint _startWaypoint;
        private Waypoint _prevWaypoint;
        private Direction _startDirection;
        private Direction _direction;
        private float _leftOverDistance;
        private float _elapsedTime;
        private bool _getNextWaypoint;

        public Waypoint CurrentWaypoint { get; private set; }

        private void Start()
        {
            //if (_eventTime <= 0)
            //{
            //    _eventTime = 1f;
            //}
        }

        public void Activate(Path path, Direction direction, Waypoint startWaypoint, float speed)
        {
            if (speed > 0)
            {
                _onRamp = true;
                _getNextWaypoint = true;

                _path = path;
                _startDirection = direction;
                _direction = direction;
                _startWaypoint = startWaypoint;
                _prevWaypoint = startWaypoint;
                CurrentWaypoint = startWaypoint;
                //_elapsedTime = 0;
                _speed = speed;
                _leftOverDistance = 0;

                //Debug.Log("Direction on path: " + direction);
            }
        }

        public void Deactivate()
        {
            _onRamp = false;
            _getNextWaypoint = false;
        }

        public bool MoveAlongRamp()
        {
            // Are we close enough to the current waypoint?
            //    If yes, get the next waypoint
            // Move towards the current waypoint

            UpdateCurrentWaypoint();

            if (_onRamp)
            {
                bool wpReached = MoveUsingSpeed(CurrentWaypoint.Position, false);

                if (wpReached && _onRamp)
                {
                    UpdateCurrentWaypoint();
                    MoveUsingSpeed(CurrentWaypoint.Position, true);
                }
            }

            return !_onRamp;
        }

        private void UpdateCurrentWaypoint()
        {
            Waypoint newWaypoint = GetWaypoint();
            CurrentWaypoint = (newWaypoint == null ? CurrentWaypoint : newWaypoint);
        }

        private Waypoint GetWaypoint()
        {
            Waypoint result = CurrentWaypoint;

            if (_getNextWaypoint)
            {
                _getNextWaypoint = false;

                result = _path.GetNextWaypoint(CurrentWaypoint, ref _direction);

                if (result == null)
                {
                    Deactivate();
                }
                else
                {
                    _prevWaypoint = CurrentWaypoint;
                    //_segmentTime = Vector3.Distance(CurrentWaypoint.Position, result.Position) / _speed;
                }
            }

            return result;
        }

        //private Waypoint GetWaypoint()
        //{
        //    Waypoint result = CurrentWaypoint;
        //    Vector3 toWaypointVector = CurrentWaypoint.Position - transform.position;
        //    float toWaypointSqr = toWaypointVector.sqrMagnitude;
        //    float sqrArriveDistance = _arriveDistance * _arriveDistance;

        //    if (toWaypointSqr <= sqrArriveDistance)
        //    {
        //        result = _path.GetNextWaypoint(CurrentWaypoint, ref _direction);
        //    }

        //    return result;
        //}

        public Vector3 GetRampSegmentDirection()
        {
            return (CurrentWaypoint.Position - _prevWaypoint.Position).normalized;
        }

        private bool MoveUsingSpeed(Vector3 waypointPos, bool leftOverOnly)
        {
            Vector3 position1 = transform.position;
            Vector3 position2 = waypointPos;

            if (leftOverOnly)
            {
                if (_leftOverDistance > 0)
                {
                    transform.position = Vector3.MoveTowards(position1, position2, _leftOverDistance);
                }
            }
            else
            {
                Vector3 direction = GetRampSegmentDirection();
                float incline = direction.y;
                //Debug.Log("incline: " + incline);

                if (_direction == _startDirection)
                {
                    _speed += incline * _gravity;

                    if (_speed < 0)
                    {
                        ChangeDirection();
                        position2 = CurrentWaypoint.Position;
                    }
                }
                else
                {
                    //Debug.Log("Speed change: " + incline * _gravity * 10);
                    //Debug.Log("_gravity: " + _gravity);
                    _speed += incline * _gravity;

                    float rampExitDistance = 2f;
                    if (Vector3.Distance(transform.position, _startWaypoint.Position) < rampExitDistance)
                    {
                        //Debug.Log("Returned to the start of the ramp");
                        Deactivate();
                        return true;
                    }
                }

                transform.position = Vector3.MoveTowards(position1, position2, Time.deltaTime * _speed);
                _leftOverDistance = Time.deltaTime * _speed - Vector3.Distance(position1, transform.position);
            }

            float segmentFinishDistance = 0.1f;
            bool segmentFinished = Vector3.Distance(position1, position2) < segmentFinishDistance;
            if (segmentFinished)
            {
                _elapsedTime = 0;
                _getNextWaypoint = true;
                return true;
            }

            return false;
        }

        private bool MoveUsingTime(Vector3 waypointPos)
        {
            float ratio = _elapsedTime / _segmentTime;
            //float ratio = _elapsedTime / _eventTime;
            _directionalRatio = ratio;

            Vector3 position1 = _prevWaypoint.Position;
            Vector3 position2 = waypointPos;

            switch (_direction)
            {
                //case Direction.Forward:
                //{
                //    _directionalRatio = ratio;
                //    break;
                //}
                case Direction.Backward:
                {
                    _directionalRatio = 1 - ratio;
                    position1 = waypointPos;
                    position2 = _prevWaypoint.Position;
                    break;
                }
            }

            transform.position = Vector3.Lerp(position1, position2, _directionalRatio);

            _elapsedTime += Time.deltaTime;

            bool segmentFinished = _elapsedTime >= _segmentTime;
            if (segmentFinished)
            {
                _elapsedTime = 0;
                _getNextWaypoint = true;
                return true;
            }

            return false;
        }

        private void ChangeDirection()
        {
            //Debug.Log("Direction changed");

            _speed = -1f * _speed;
            _direction = (_direction == Direction.Forward ? Direction.Backward : Direction.Forward);

            Waypoint temp = _prevWaypoint;
            _prevWaypoint = CurrentWaypoint;
            CurrentWaypoint = temp;
        }
    }
}
