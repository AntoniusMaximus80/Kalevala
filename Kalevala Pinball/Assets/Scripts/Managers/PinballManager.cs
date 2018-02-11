using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class PinballManager : MonoBehaviour
    {
        public float _flipperMotorForce;
        public float _flipperMotorTargetVelocity;
        public float _springForce;

        [SerializeField, Range(1, 10)]
        private int _startingBallAmount;

        [SerializeField]
        public int _allowedNudgeAmount;

        [SerializeField]
        private Vector3 _ballLaunchPoint;

        [SerializeField]
        private Vector3 _ballDrainBottomLeftCorner;

        [SerializeField]
        private Vector3 _ballDrainTopRightCorner;

        private int _currentBallAmount;
        private int _nudgesLeft;

        private bool noNudges;

        public bool Tilt { get; private set; }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _ballDrainTopRightCorner.y = _ballDrainBottomLeftCorner.y;

            Reset();

            if (_allowedNudgeAmount <= 0)
            {
                noNudges = true;
            }
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        private void Update()
        {
            // ...
        }

        public void Reset()
        {
            _currentBallAmount = _startingBallAmount;
            _nudgesLeft = _allowedNudgeAmount;
            Tilt = false;
        }

        public bool OutOfBalls()
        {
            return _currentBallAmount <= 0;
        }

        public bool LoseBall()
        {
            if (!OutOfBalls())
            {
                _currentBallAmount--;
            }

            return OutOfBalls();
        }

        public bool PositionIsInDrain(Vector3 position)
        {
            bool inDrainZ = (position.z > _ballDrainTopRightCorner.z);
            //bool withinX = (position.x >= _ballDrainBottomLeftCorner.x &&
            //                position.x < _ballDrainTopRightCorner.x);
            //bool withinZ = (position.z >= _ballDrainBottomLeftCorner.z &&
            //                position.z < _ballDrainTopRightCorner.z);

            return inDrainZ; //withinX && withinZ;
        }

        public bool InstanceNextBall(Pinball ball)
        {
            if (!OutOfBalls())
            {
                ball.transform.position = _ballLaunchPoint;
                ball.StopMotion();
                return true;
            }

            return false;
        }

        public bool Nudge()
        {
            if (!noNudges && _nudgesLeft > 0)
            {
                _nudgesLeft--;

                if (_nudgesLeft == 0)
                {
                    Tilt = true;
                }

                return true;
            }

            return false;
        }

        private void OnDrawGizmos()
        {
            DrawBallLaunchPoint();
            DrawBallDrainAreaBorders();
        }

        private void DrawBallLaunchPoint()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_ballLaunchPoint, 1.5f);
        }

        private void DrawBallDrainAreaBorders()
        {
            Gizmos.color = Color.red;

            float y = _ballDrainBottomLeftCorner.y;

            Vector3 corner1 = _ballDrainBottomLeftCorner;
            Vector3 corner2 = new Vector3(
                _ballDrainBottomLeftCorner.x, y, _ballDrainTopRightCorner.z);
            Vector3 corner3 = _ballDrainTopRightCorner;
            Vector3 corner4 = new Vector3(
                _ballDrainTopRightCorner.x, y, _ballDrainBottomLeftCorner.z);

            Gizmos.DrawLine(corner1, corner2);
            Gizmos.DrawLine(corner2, corner3);
            Gizmos.DrawLine(corner3, corner4);
            Gizmos.DrawLine(corner4, corner1);
        }
    }
}