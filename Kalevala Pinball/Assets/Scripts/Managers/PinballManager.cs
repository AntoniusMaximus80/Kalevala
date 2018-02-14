using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class PinballManager : MonoBehaviour
    {
        #region Statics
        private static PinballManager instance;

        public static PinballManager Instance
        {
            get
            {

                // If no instance exists, all the values will be wrong and the code using this will not work anyway.
                // So there is no real point proofing this again people forgetting to set pinballmanager,
                // just remind them and stop execution.
                if (!instance) {
                    Debug.LogError("No pinballmanager instance set in the scene.");
                    Debug.Break();
                }


                //if (instance == null)
                //{
                //    instance = FindObjectOfType<PinballManager>();
                //}
                //if (instance == null)
                //{
                //    // Note:
                //    // There must be a Resources folder under Assets and
                //    // PinballManager there for this to work. Not necessary if
                //    // a PinballManager object is present in a scene from the
                //    // get-go.

                //    instance =
                //        Instantiate(Resources.Load<PinballManager>("PinballManager"));
                //}

                return instance;
            }
        }
        #endregion Statics

        public float _flipperMotorForce;
        public float _flipperMotorTargetVelocity;
        public float _springForce;

        [SerializeField]
        private float _rampGravity = -7;

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

        [SerializeField]
        private Transform _startingPosition;

        private Pinball[] _pinballs;

        private int _currentBallAmount, _activeBalls;
        private int _nudgesLeft;


        private bool noNudges;

        public bool Tilt { get; private set; }

        public Pinball[] Pinballs
        {
            get
            {
                if (_pinballs == null)
                {
                    _pinballs = FindObjectsOfType<Pinball>();
                    _activeBalls = _pinballs.Length;
                    Debug.Log("Initial balls : " + _activeBalls.ToString());
                }

                return _pinballs;
            }
        }

        public float PinballRadius
        {
            get
            {
                if (Pinballs != null && Pinballs.Length >= 1)
                {
                    return Pinballs[0].Radius;
                }
                else
                {
                    return 0;
                }
            }
        }

        public float RampGravity
        {
            get
            {
                return _rampGravity;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Init();
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Init()
        {
            _ballDrainTopRightCorner.y = _ballDrainBottomLeftCorner.y;

            Reset();

            if (_allowedNudgeAmount <= 0)
            {
                noNudges = true;
            }
        }

        
        public void Reset()
        {
            _currentBallAmount = _startingBallAmount;
            _nudgesLeft = _allowedNudgeAmount;
            Tilt = false;

            _pinballs = FindObjectsOfType<Pinball>();
            _activeBalls = _pinballs.Length;
            Debug.Log("Initial balls : " + _activeBalls.ToString());

            if(_startingPosition)
            {
                _ballLaunchPoint = _startingPosition.position;
            }
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

        public void InstanceNextBall(Pinball ball)
        {
            
                ball.transform.position = _ballLaunchPoint;
                ball.StopMotion();
                        
        }

        public void RemoveBall(Pinball pinball)
        {
            if(_activeBalls>1)
            {
                pinball.gameObject.SetActive(false);
                _activeBalls--;
            }
            else
            {
                if (!OutOfBalls())
                {
                    InstanceNextBall(pinball);
                    _currentBallAmount--;
                    Debug.Log("Balls left : " + _currentBallAmount.ToString());
                }
                else
                {
                    Debug.Log("Out of balls");
                }
            }
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