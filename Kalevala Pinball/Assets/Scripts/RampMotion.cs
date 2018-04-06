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

        public float _speed; // public for debugging

        private bool _onRamp;
        private Path _path;
        private Waypoint _startWaypoint;
        private Waypoint _prevWaypoint;
        private Direction _startDirection;
        private Direction _direction;
        private float _leftoverDistance;
        private bool _getNextWaypoint;
        private bool _kickOut;
        private KickoutHole _kickoutHole;
        public Waypoint CurrentWaypoint { get; private set; }

        public void Activate(Path path, Direction direction, Waypoint startWaypoint, float speed, KickoutHole kickouthole)
        {
            if (speed > 0)
            {
                _onRamp = true;
                _getNextWaypoint = true;
                _kickOut = kickouthole != null;
                _path = path;
                _startDirection = direction;
                _direction = direction;
                _startWaypoint = startWaypoint;
                _prevWaypoint = startWaypoint;
                CurrentWaypoint = startWaypoint;
                _speed = speed;
                _leftoverDistance = 0;
                _kickoutHole = kickouthole;
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
            // Return whether the ball is still on the ramp

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

            return _onRamp;
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
            Vector3 result;

            if ( !_kickOut )
            {
                result = CurrentWaypoint.Position - _prevWaypoint.Position;
            }
            else
            {
                result = _kickoutHole.KickDirection;
            }

            return result.normalized;
        }

        private bool MoveUsingSpeed(Vector3 waypointPos, bool leftOverOnly)
        {
            float movingDistance;

            Vector3 startPosition = transform.position;
            Vector3 targetPosition = waypointPos;

            Vector3 direction = GetRampSegmentDirection();

            if (leftOverOnly)
            {
                movingDistance = _leftoverDistance;
            }
            else
            {
                movingDistance = _speed * Time.deltaTime;
                _speed = GetSpeedAffectedByGravity(direction);
            }

            // The direction on the ramp has not changed
            if (_direction == _startDirection)
            {
                // Changes direction on the ramp
                if (_speed < 0)
                {
                    ChangeDirection();
                    targetPosition = CurrentWaypoint.Position;
                }
            }
            // The direction on the ramp has changed
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
            _leftoverDistance = movingDistance -
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

        private float GetSpeedAffectedByGravity(Vector3 direction)
        {
            float result = _speed;

            if (!_kickOut)
            {
                // TODO: Fix

                float gravityRatioY = Physics.gravity.y /
                    (Physics.gravity.y + Physics.gravity.z);
                float gravityRatioZ = Physics.gravity.z /
                    (Physics.gravity.y + Physics.gravity.z);
                //float gravityRatioZ2 = Physics.gravity.z / Physics.gravity.y;

                //float inclineRatioY = direction.y /
                //    (direction.y + direction.z);
                //float inclineRatioZ = direction.z /
                //    (direction.y + direction.z);

                float incline = direction.y * gravityRatioY + direction.z * gravityRatioZ;
                //float incline = direction.y;
                //Debug.Log("incline: " + incline);

                //Physics.gravity;


                //Debug.Log("Speed change: " + incline * _gravity * 10);
                //Debug.Log("_gravity: " + _gravity);
                result -= incline * PinballManager.Instance.RampGravityMultiplier;
            }

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
    }
}
