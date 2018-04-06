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
                if (!instance)
                {
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
        private float _rampGravityMultiplier = 7;

        [SerializeField]
        private float _rampExitMomentumFactor = 0.75f;

        [SerializeField, Range(1, 10)]
        private int _startingBallAmount;

        [SerializeField]
        public int _allowedNudgeAmount;

        [SerializeField, Tooltip
            ("Use the wire sphere gizmo's position as the launch point.")]
        private bool debug_useDefaultLaunchPoint;

        [SerializeField]
        private Vector3 _ballLaunchPoint;

        [SerializeField]
        private Vector3 _ballDrainBottomLeftCorner;

        [SerializeField]
        private Vector3 _ballDrainTopRightCorner;

        [SerializeField]
        private Transform _startingPosition;

        [SerializeField, Tooltip("The prefab for creating extra balls.")]
        private Pinball _pinballPrefab;

        private StatusPanelManager _status;

        private List<Pinball> _pinballs;
        private List<Pinball> _autosavedPinballs;

        private int _currentBallAmount, _activeBalls;
        private int _nudgesLeft;

        [SerializeField] // Serialized for debugging purposes only
        private float _autosaveTimeRemaining;

        //[SerializeField]
        //private Vector3 _shootAgainLightPos; // Debugging purposes only

        private bool _shootAgain;
        private float _shootAgainTimeOut;

        private bool _noNudges;

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

        private void Start()
        {
            _status = GetComponentInChildren<StatusPanelManager>();
            _autosavedPinballs = new List<Pinball>();

            ResetGame();
        }

        private void Update()
        {
            UpdateShootAgain();
            UpdateAutosave();
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Init()
        {
            _ballDrainTopRightCorner.y = _ballDrainBottomLeftCorner.y;

            if (_allowedNudgeAmount <= 0)
            {
                _noNudges = true;
            }

            if (!debug_useDefaultLaunchPoint &&
                _startingPosition != null)
            {
                LaunchPoint = _startingPosition.position;
            }
        }

        public bool ShootAgain
        {
            get
            {
                return _shootAgainTimeOut > Time.time;
            }

            set
            {

                if (value)
                {
                    _shootAgainTimeOut = Mathf.Max(_shootAgainTimeOut, Time.time) + 60f;
                    Debug.Log("Shoot Again activated for one minute");
                }
                else
                {
                    _shootAgainTimeOut = -1f;
                }
            }
        }

        public float ShootAgainDuration
        {
            set
            {
                _shootAgainTimeOut = Mathf.Max(_shootAgainTimeOut, Time.time) + value;
                Debug.Log("Shoot Again activated for " + value + " seconds");
            }
        }

        public bool Autosave { get; private set; }

        public bool Tilt { get; private set; }

        public Vector3 LaunchPoint
        {
            get
            {
                return _ballLaunchPoint;
            }
            set
            {
                _ballLaunchPoint = value;
            }
        }

        public List<Pinball> Pinballs
        {
            get
            {
                if (_pinballs == null)
                {
                    _pinballs = new List<Pinball>(FindObjectsOfType<Pinball>());
                    _activeBalls = _pinballs.Count;
                    Debug.Log("Initial balls : " + _activeBalls.ToString());
                }

                return _pinballs;
            }
        }

        public float PinballRadius
        {
            get
            {
                if (Pinballs != null && Pinballs.Count >= 1)
                {
                    return Pinballs[0].Radius;
                }
                else
                {
                    return 0;
                }
            }
        }

        public float RampGravityMultiplier
        {
            get
            {
                return _rampGravityMultiplier;
            }
        }

        public float RampExitMomentumFactor
        {
            get
            {
                return _rampExitMomentumFactor;
            }
        }

        public void ResetGame()
        {
            _currentBallAmount = _startingBallAmount;
            _nudgesLeft = _allowedNudgeAmount;
            Tilt = false;
            Autosave = false;
            ShootAgain = false;
            _status.ResetStatus();

            _pinballs = new List<Pinball>(FindObjectsOfType<Pinball>());
            _autosavedPinballs.Clear();
            _activeBalls = _pinballs.Count;
            Debug.Log("Total balls : " + _currentBallAmount.ToString());
            Debug.Log("Initial balls : " + _activeBalls.ToString());

            ResetAllPinballs();
            //SetPinballPhysicsEnabled(false);
        }

        public void UpdatePinballs()
        {
            foreach (Pinball ball in _pinballs)
            {
                ball.UpdatePinball();
            }
        }

        private void UpdateShootAgain()
        {
            if (_shootAgain != _shootAgainTimeOut > Time.time)
            {
                _shootAgain = _shootAgainTimeOut > Time.time;
                _status.SwitchLight
                    (StatusPanelManager.PanelType.ShootAgain, _shootAgain);
            }
        }

        private void UpdateAutosave()
        {
            if (Autosave)
            {
                if (_autosavedPinballs.Count > 0 &&
                    Launcher.Instance.LaunchAreaIsEmpty)
                {
                    ReturnBallToLauncher
                        (_autosavedPinballs[0], true);
                    _autosavedPinballs.RemoveAt(0);
                }

                _autosaveTimeRemaining -= Time.deltaTime;

                // The autosave is deactivated and the queue is cleared
                if (_autosaveTimeRemaining <= 0)
                {
                    Autosave = false;
                    _autosavedPinballs.Clear();
                }
            }
        }

        /// <summary>
        /// Activates Autosave for the given duration.
        /// </summary>
        /// <param name="duration">The duration of Autosave in seconds</param>
        public void ActivateAutosave(float duration)
        {
            Autosave = true;
            _autosaveTimeRemaining = duration;

            Debug.Log("Autosave activated");
        }

        public void ResetPinball()
        {
            if (_pinballs.Count > 0)
            {
                InstanceNextBall(_pinballs[0]);
            }
        }

        public void ResetAllPinballs()
        {
            if (_pinballs.Count > 0)
            {
                // Removes all extra balls from play
                _activeBalls = 1;

                for (int i = 0; i < _pinballs.Count; i++)
                {
                    _pinballs[i].gameObject.SetActive(false);
                }

                _pinballs[0].gameObject.SetActive(true);
                InstanceNextBall(_pinballs[0]);
            }

            //foreach (Pinball ball in _pinballs)
            //{
            //    InstanceNextBall(ball);
            //}
        }

        public bool OutOfBalls
        {
            get
            {
                return _currentBallAmount <= 0;
            }
        }

        public bool CheckIfBallIsLost(Pinball ball, bool freeBalls)
        {
            if (PositionIsInDrain(ball.transform.position))
            {
                // If balls are free, a ball in drain is moved
                // back to the launcher without consuming "lives"
                if (freeBalls)
                {
                    InstanceNextBall(ball);
                }
                else
                {
                    RemoveBall(ball);
                }

                return true;
            }

            return false;
        }

        public bool PositionIsInDrain(Vector3 position)
        {
            //bool inDrainZ = (position.z < _ballDrainTopRightCorner.z);
            bool withinX = (position.x >= _ballDrainBottomLeftCorner.x &&
                            position.x < _ballDrainTopRightCorner.x);
            bool withinZ = (position.z >= _ballDrainBottomLeftCorner.z &&
                            position.z < _ballDrainTopRightCorner.z);

            return /*inDrainZ;*/ withinX && withinZ;
        }

        public void InstanceNextBall(Pinball ball)
        {
            ball.transform.position = _ballLaunchPoint;
            ball.StopMotion();
            //ball.SetPhysicsEnabled(false);
            Launcher.Instance.StartLaunch(ball);
        }

        /// <summary>
        /// Returns the ball next to the launcher.
        /// If Autosave or Shoot Again is active, the ball is not lost.
        /// </summary>
        /// <param name="ball">A pinball that went down the drain</param>
        /// <param name="lauchFreeBallOnly">Is the ball returned to the
        /// launcher only if it's free</param>
        /// <returns>Is the ball lost</returns>
        public bool ReturnBallToLauncher(Pinball ball, bool lauchFreeBallOnly)
        {
            // Returns the ball next to the launcher if it's
            // free or if also non-free balls can be launched
            if (Autosave)
            {
                // The ball is returned to the launcher if there's not
                // already a ball there. If there is, the ball is added
                // to a queue and will be returned to the launcher when
                // the previous one is launched.
                if (Launcher.Instance.LaunchAreaIsEmpty)
                {
                    InstanceNextBall(ball);
                }
                else
                {
                    _autosavedPinballs.Add(ball);
                }
            }
            else if (ShootAgain || !lauchFreeBallOnly)
            {
                InstanceNextBall(ball);
            }

            // The ball is not lost
            if (Autosave)
            {
                Debug.Log("Ball autosaved");
                return false;
            }
            // The ball is not lost but Shoot Again becomes unlit
            else if (ShootAgain)
            {
                Debug.Log("Shooting ball again");
                ShootAgain = false;
                return false;
            }
            // The ball is lost
            else
            {
                return true;
            }
        }

        public void RemoveBall(Pinball pinball)
        {
            Tilt = false;

            if (_activeBalls > 1)
            {
                // Returns the ball next to the launcher
                // only if Shoot Again is lit
                bool ballLost = ReturnBallToLauncher(pinball, true);

                // Removes the ball from play
                if (ballLost)
                {
                    pinball.gameObject.SetActive(false);
                    _activeBalls--;
                }
            }
            else
            {
                if (!OutOfBalls)
                {
                    // Returns the ball next to the launcher
                    // and gets whether the ball is lost
                    bool ballLost = ReturnBallToLauncher(pinball, false);

                    if (ballLost)
                    {
                        _currentBallAmount--;
                    }

                    if (OutOfBalls)
                    {
                        Debug.Log("Out of balls - game over");
                        GameManager.Instance.GameOver(true);
                    }
                    else
                    {
                        Debug.Log("Balls left : " +
                            _currentBallAmount.ToString());
                    }
                }
                else
                {
                    Debug.Log("Out of balls");
                }
            }
        }

        /// <summary>
        /// Removes a ball for debugging purposes.
        /// </summary>
        public void DebugLoseBall()
        {
            RemoveBall(_pinballs[0]);
        }

        /// <summary>
        /// The method to launch extra balls.
        /// </summary>
        /// <param name="location">Transform giving the position,
        /// you can use an empty gameobject linked to launching object.</param>
        /// <param name="impulse">You can give the ball a "kick" when launching. Use null or Vector3.zero if not needed.</param>
        public void ExtraBall(Transform location, Vector3 impulse)
        {
            // Use a deactivated ball if possible.
            Pinball ball = RecycleBall();

            // If not create a new ball from prefab.
            if (!ball)
            {
                ball = Instantiate<Pinball>(_pinballPrefab);
                _pinballs.Add(ball);
            }

            // If it was found activate it.
            else
            {
                ball.gameObject.SetActive(true);
            }

            // Update active ball counter and set ball location.
            _activeBalls++;
            ball.transform.position = location.position;

            // If given a valid impulse vector apply it.
            if (impulse != null && !impulse.Equals(Vector3.zero))
            {
                ball.AddImpulseForce(impulse);
            }
        }

        private Pinball RecycleBall()
        {
            foreach (Pinball ball in _pinballs)
            {
                if (!ball.gameObject.activeSelf) return ball;
            }

            return null;
        }

        /// <summary>
        /// Sets all pinballs' physics enabled or disabled.
        /// Used when a game starts or ends.
        /// </summary>
        /// <param name="enable">are pinball physics enabled</param>
        public void SetPinballPhysicsEnabled(bool enable)
        {
            foreach (Pinball ball in _pinballs)
            {
                ball.SetPhysicsEnabled(enable);
            }
        }

        public bool SpendNudge()
        {
            if (!_noNudges && _nudgesLeft > 0)
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

            // Debugging purposes only, remove when an actual lights are implemented
            // Not sure about the autosave light, but I removed the shoot again light as unnecessary
            // since Toni did the light long time ago and that broke the auto save light, oops!
            // Autosave light
            //if (Autosave)
            //{
            //    Gizmos.color = Color.green;
            //    Gizmos.DrawSphere(_LightPos + Vector3.right * 2.2f, 1);
            //}
        }

        private void DrawBallLaunchPoint()
        {
            if (debug_useDefaultLaunchPoint)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.blue;
            }

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