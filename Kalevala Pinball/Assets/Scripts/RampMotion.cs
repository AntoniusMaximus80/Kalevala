using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kalevala.WaypointSystem;

namespace Kalevala
{
    public class RampMotion : MonoBehaviour
    {
        //[SerializeField]
        //private float _eventTime = 1;

        //[SerializeField, Range(0, 1)]
        //private float _directionalRatio;

        public float _speed;

        private bool _onRamp;
        private Path _path;
        private Waypoint _startWaypoint;
        private Waypoint _prevWaypoint;
        private Direction _startDirection;
        private Direction _direction;
        private float _leftoverDistance;
        //private float _segmentTime;
        //private float _elapsedTime;
        private bool _getNextWaypoint;

        public Waypoint CurrentWaypoint { get; private set; }

        //public PinballManager PinballManager { get; set; }

        //private void Start()
        //{
        //    if (_eventTime <= 0)
        //    {
        //        _eventTime = 1f;
        //    }
        //}

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
                _leftoverDistance = 0;

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
            // Did we reach the next waypoint but didn't move as far as we could?
            //    If yes, get the next waypoint and move again using the leftover movement
            //    Repeat until there's no leftover movement

            bool wpReached = false;
            int repeats = 0;

            do
            {
                Waypoint newWaypoint = GetWaypoint();
                CurrentWaypoint = (newWaypoint == null ?
                    CurrentWaypoint : newWaypoint);

                if (_onRamp)
                {
                    if (repeats == 0)
                    {
                        wpReached = MoveUsingSpeed(CurrentWaypoint.Position, false);
                    }
                    else if (_leftoverDistance > 0)
                    {
                        wpReached = MoveUsingSpeed(CurrentWaypoint.Position, true);
                    }
                    else
                    {
                        wpReached = false;
                    }
                }

                //Debug.Log(string.Format("Repeat: {0}; dist left: {1}", repeats, _leftoverDistance));
                repeats++;
            }
            while (_onRamp && wpReached);

            return !_onRamp;
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
            float movingDistance;

            Vector3 startPosition = transform.position;
            Vector3 targetPosition = waypointPos;

            Vector3 direction = GetRampSegmentDirection();
            float incline = direction.y;
            //Debug.Log("incline: " + incline);

            if (leftOverOnly)
            {
                movingDistance = _leftoverDistance;
            }
            else
            {
                movingDistance = Time.deltaTime * _speed;
                _speed = GetSpeedAffectedByGravity(incline);
            }

            if (_direction == _startDirection)
            {
                if (_speed < 0)
                {
                    ChangeDirection();
                    targetPosition = CurrentWaypoint.Position;
                }
            }
            else
            {
                // The maximum distance from the start of
                // the ramp where the ball can exit it
                float rampExitDistance = 2f;

                // Checks if the ball returned to the start of
                // the ramp, and if so, deactivates the ramp motion 
                if (Vector3.Distance(transform.position, _startWaypoint.Position) < rampExitDistance)
                {
                    //Debug.Log("Returned to the start of the ramp");
                    Deactivate();
                    return true;
                }
            }

            // Moves the pinball
            transform.position = Vector3.MoveTowards(
                startPosition, targetPosition, movingDistance);

            // Updates the leftover distance
            _leftoverDistance = 
                movingDistance -
                Vector3.Distance(startPosition, transform.position);

            // Checks if the segment is finished
            float segmentFinishDistance = 0.1f;
            bool segmentFinished =
                Vector3.Distance(transform.position, targetPosition)
                < segmentFinishDistance;
            if (segmentFinished)
            {
                // Enables getting the next waypoint
                _getNextWaypoint = true;
                return true;
            }

            return false;
        }

        private float GetSpeedAffectedByGravity(float incline)
        {
            float result = _speed;

            //Debug.Log("Speed change: " + incline * _gravity * 10);
            //Debug.Log("_gravity: " + _gravity);
            result += incline * PinballManager.Instance.RampGravity;

            return result;
        }

        private void ChangeDirection()
        {
            //Debug.Log("Direction changed");

            _speed = -1f * _speed;
            _direction = (_direction == Direction.Forward ?
                Direction.Backward : Direction.Forward);

            Waypoint temp = _prevWaypoint;
            _prevWaypoint = CurrentWaypoint;
            CurrentWaypoint = temp;
        }

        //private bool MoveUsingTime(Vector3 waypointPos)
        //{
        //    float ratio = _elapsedTime / _segmentTime;
        //    //float ratio = _elapsedTime / _eventTime;
        //    _directionalRatio = ratio;

        //    Vector3 position1 = _prevWaypoint.Position;
        //    Vector3 position2 = waypointPos;

        //    switch (_direction)
        //    {
        //        //case Direction.Forward:
        //        //{
        //        //    _directionalRatio = ratio;
        //        //    break;
        //        //}
        //        case Direction.Backward:
        //        {
        //            _directionalRatio = 1 - ratio;
        //            position1 = waypointPos;
        //            position2 = _prevWaypoint.Position;
        //            break;
        //        }
        //    }

        //    transform.position = Vector3.Lerp(position1, position2, _directionalRatio);

        //    _elapsedTime += Time.deltaTime;

        //    bool segmentFinished = _elapsedTime >= _segmentTime;
        //    if (segmentFinished)
        //    {
        //        _elapsedTime = 0;
        //        _getNextWaypoint = true;
        //        return true;
        //    }

        //    return false;
        //}
    }
}
