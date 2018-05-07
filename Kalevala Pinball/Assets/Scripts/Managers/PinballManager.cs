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
                if (!instance)
                {
                    instance = FindObjectOfType<PinballManager>();

                    // If no instance exists, all the values will be wrong and the code using this will not work anyway.
                    // So there is no real point proofing this again people forgetting to set pinballmanager,
                    // just remind them and stop execution.
                    if (!instance)
                    {
                        Debug.LogError("No pinballmanager instance set in the scene.");
                        Debug.Break();
                    }
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

        internal static void ActivateWorkshopExtraBalls()
        {
            Instance.ActivateMultiball();
            //Instance.StartCoroutine(Instance.WorkShopExtraBallRoutine());
        }        

        #endregion Statics

        public float _flipperMotorForce;
        public float _flipperMotorTargetVelocity;
        public float _springForce;
        public float GameOverDelay = 1f;

        [SerializeField, Tooltip
            ("Use the wire sphere gizmo's position as the launch point.")]
        private bool debug_useDefaultLaunchPoint;

        [SerializeField, Tooltip
            ("Balls are never lost.")]
        private bool debug_freeBalls;

        [SerializeField]
        private float _rampGravityMultiplier = 7;

        [SerializeField]
        private float _rampExitSpeedMultiplier = 0.75f;

        [SerializeField, Range(1, 10)]
        private int _startingBallCount;

        [SerializeField, Range(1, 5)]
        private int _workshopExtraBalls;

        [SerializeField]
        private Transform _workshopLocation;

        [SerializeField]
        public int _allowedNudgeAmount;

        [SerializeField]
        private Vector3 _ballLaunchPoint;

        [SerializeField]
        private Vector3 _ballLaunchArrivalPoint;

        [SerializeField]
        private float _ballLaunchArrivalTime;

        [SerializeField]
        private Vector3 _ballDrainBottomLeftCorner;

        [SerializeField]
        private Vector3 _ballDrainTopRightCorner;

        [SerializeField]
        private Transform _startingPosition;

        [SerializeField, Tooltip("The prefab for creating extra balls.")]
        private Pinball _pinballPrefab;

        [SerializeField]
        private ExtraBallSpawner _extraBallSpawner;

        private StatusPanelManager _status;
        private List<Pinball> _pinballs;

        private int _currentBallCount, _activeBalls;
        private int _nudgesLeft;
        private float _elapsedBallLaunchArrivalTime;

        [SerializeField] // Serialized for debugging purposes only
        private float _shootAgainTimeRemaining;

        [SerializeField] // Serialized for debugging purposes only
        private float _autosaveTimeRemaining;

        //[SerializeField]
        //private Vector3 _shootAgainLightPos; // Debugging purposes only

        //private bool _shootAgain;
        //private float _shootAgainTimeOut;

        private bool _tilt;
        private bool _noNudges;

        public event Action ResourcesChanged;
        private int _resources;
        [SerializeField]
        private int _maxResources;

        public int Resources
        {
            get
            {
                return _resources;
            }
            private set
            {
                if(_resources + value >= MaxResources)
                {
                    //Add score for excessive recourses
                }
                _resources = (int)Mathf.Clamp(_resources + value, 0, MaxResources);

                // Make Viewscreen update when resources are changed, so the text will change if the threshold is crossed.
                // Not using the event since I would need to check the resource change anyway, so event gives no advantage.
                Viewscreen.UpdateModeInfo();

                if(ResourcesChanged != null)
                {
                    ResourcesChanged();
                }
            }
        }

        public float MaxResources
        {
            get { return _maxResources; }
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

        private void Start()
        {
            _status = GetComponentInChildren<StatusPanelManager>();

            ResetGame();
        }

        private void Update()
        {
            // Debugging purposes only
            if(Input.GetKey(KeyCode.A))
            {
                Resources = 45;
            }

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

            //InitGravity();
        }

        ///// <summary>
        ///// Initializes gravity.
        ///// </summary>
        //private void InitGravity()
        //{
        //    //float totalGravity = Physics.gravity.y + Physics.gravity.z;

        //    //float gravityRatioY = Physics.gravity.y / totalGravity;
        //    //float gravityRatioZ = Physics.gravity.z / totalGravity;

        //    Gravity = Physics.gravity.normalized;
        //}

        //public Vector3 Gravity { get; private set; }

        public int CurrentBallNumber
        {
            get
            {
                return _startingBallCount - _currentBallCount + 1;
            }
        }

        public bool ShootAgain { get; private set; }
        //{
            //get
            //{
            //    return _shootAgainTimeOut > Time.time;
            //}

            //set
            //{

            //    if (value)
            //    {
            //        _shootAgainTimeOut = Mathf.Max(_shootAgainTimeOut, Time.time) + 60f;
            //        Debug.Log("Shoot Again activated for one minute");
            //    }
            //    else
            //    {
            //        _shootAgainTimeOut = -1f;
            //    }
            //}
        //}

        //public float ShootAgainDuration
        //{
        //    set
        //    {
        //        _shootAgainTimeOut = Mathf.Max(_shootAgainTimeOut, Time.time) + value;
        //        Debug.Log("Shoot Again activated for " + value + " seconds");
        //    }
        //}

        public bool Autosave { get; private set; }

        public bool Tilt
        {
            get
            {
                return _tilt;
            }
            set
            {
                if (value == false)
                {
                    _nudgesLeft = _allowedNudgeAmount;
                }

                _tilt = value;
            }
        }

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

        public Vector3 LaunchArrivalPoint
        {
            get
            {
                return _ballLaunchArrivalPoint;
            }
            set
            {
                _ballLaunchArrivalPoint = value;
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

        public float GlobalRampGravityMultiplier
        {
            get
            {
                return _rampGravityMultiplier;
            }
        }

        public float GlobalRampExitMomentumFactor
        {
            get
            {
                return _rampExitSpeedMultiplier;
            }
        }

        public void ResetGame()
        {
            _currentBallCount = _startingBallCount;
            Tilt = false;
            Autosave = false;
            ShootAgain = false;
            _extraBallSpawner.Deactivate();
            _status.ResetStatus();

            _pinballs = new List<Pinball>(FindObjectsOfType<Pinball>());
            _activeBalls = _pinballs.Count;
            Debug.Log("Total balls : " + _currentBallCount.ToString());
            Debug.Log("Initial balls : " + _activeBalls.ToString());

            ResetAllPinballs();
            //SetPinballPhysicsEnabled(false);

            Viewscreen.BallCount(CurrentBallNumber);
        }

        public void UpdatePinballs()
        {
            // Creates a backup list because multiball
            // mode breaks this otherwise
            List<Pinball> pbList = new List<Pinball>(_pinballs);

            foreach (Pinball ball in pbList)
            {
                if (ball.gameObject.activeSelf)
                {
                    ball.UpdatePinball();
                }
            }
        }

        /// <summary>
        /// Activates Shoot again for the given duration.
        /// </summary>
        /// <param name="duration">The duration of
        /// Shoot again in seconds</param>
        public void ActivateShootAgain(float duration)
        {
            ShootAgain = true;

            _shootAgainTimeRemaining = duration;

            // Adds time so any existing time still counts
            //_shootAgainTimeRemaining += duration;

            _status.SwitchLight
                (StatusPanelManager.PanelType.ShootAgain, true);

            Debug.Log("Shoot Again activated for " + duration + " seconds");
        }

        private void UpdateShootAgain()
        {
            if (ShootAgain)
            {
                _shootAgainTimeRemaining -= Time.deltaTime;

                // The shoot again is deactivated
                if (_shootAgainTimeRemaining <= 0)
                {
                    ShootAgain = false;
                    _shootAgainTimeRemaining = 0;
                    _status.SwitchLight
                        (StatusPanelManager.PanelType.ShootAgain, false);
                }
            }

            //if (_shootAgain != _shootAgainTimeOut > Time.time)
            //{
            //    _shootAgain = _shootAgainTimeOut > Time.time;
            //    _status.SwitchLight
            //        (StatusPanelManager.PanelType.ShootAgain, _shootAgain);
            //}
        }

        /// <summary>
        /// Activates Autosave for the given duration.
        /// </summary>
        /// <param name="duration">The duration of Autosave in seconds</param>
        public void ActivateAutosave(float duration)
        {
            Autosave = true;
            _autosaveTimeRemaining = duration;

            Debug.Log("Autosave activated for " + duration + " seconds");
        }

        private void UpdateAutosave()
        {
            if (Autosave)
            {
                //if (_autosavedPinballs.Count > 0 &&
                //    Launcher.Instance.LaunchAreaIsEmpty)
                //{
                //    ReturnBallToLauncher
                //        (_autosavedPinballs[0], true);
                //    _autosavedPinballs.RemoveAt(0);
                //}

                _autosaveTimeRemaining -= Time.deltaTime;

                // The autosave is deactivated
                if (_autosaveTimeRemaining <= 0)
                {
                    Autosave = false;
                    _autosaveTimeRemaining = 0;
                    Debug.Log("Autosave ended");
                }
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
                return _currentBallCount <= 0;
            }
        }

        public bool CheckIfBallIsLost(Pinball pinball)
        {
            if (PositionIsInDrain(pinball.transform.position))
            {
                RemoveOrSaveBall(pinball);

                // If balls are free, a ball in drain is moved
                // back to the launcher without consuming "lives"
                //if (freeBalls)
                //{
                //    InstanceNextBall(pinball);
                //}
                //else
                //{
                //    RemoveBall(pinball);
                //}

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

        /// <summary>
        /// Shoot again, autosave or remove ball?
        /// </summary>
        /// <param name="pinball">A drained pinball</param>
        /// <returns>Is the ball removed</returns>
        public bool RemoveOrSaveBall(Pinball pinball)
        {
            bool ballRemoved = false;

            // Multiball
            if (_activeBalls > 1)
            {
                AdjustActiveBallCounter(false);
                pinball.gameObject.SetActive(false);

                // Shoot again does not matter; if it's active, it stays active

                // Spawn a new ball
                if (Autosave)
                {
                    _extraBallSpawner.Activate(1, true);
                    ballRemoved = false;
                }
                // Just remove the ball
                else
                {
                    ballRemoved = true;

                    if (_activeBalls == 1)
                    {
                        Debug.Log("All extra balls are drained");
                    }
                }
            }
            // Only one ball
            else
            {
                Tilt = false;

                // Autosave can't be active without multiball

                // Save the ball and put it next to the launcher
                if (ShootAgain || debug_freeBalls)
                {
                    SFXPlayer.Instance.Play(Sound.BallLost);
                    SFXPlayer.Instance.Play(Sound.BallSaved);
                    InstanceNextBall(pinball);
                    ShootAgain = false;
                    ballRemoved = false;
                }
                // Remove the ball and put it next to the launcher
                else
                {
                    SFXPlayer.Instance.Play(Sound.BallLost);
                    RemoveBall(pinball);
                    ballRemoved = true;
                }
            }

            return ballRemoved;
        }

        public void InstanceNextBall(Pinball pinball)
        {
            pinball.SetPhysicsEnabled(false);

            if ( !Launcher.Instance.BallOnLauncher )
            {
                StartCoroutine(BallLaunchArrivalRoutine(pinball));
            }
            else
            {
                pinball.transform.position = _ballLaunchPoint;
                Debug.LogWarning("A ball may already be in the launch area");
            }

            pinball.SetPhysicsEnabled(true);
            Launcher.Instance.StartLaunch(pinball);
        }

        private IEnumerator BallLaunchArrivalRoutine(Pinball pinball)
        {
            float ratio = 0;
            _elapsedBallLaunchArrivalTime = 0;

            Vector3 startPosition = _ballLaunchArrivalPoint;

            while (ratio < 1f)
            {
                _elapsedBallLaunchArrivalTime += Time.deltaTime;

                ratio = _elapsedBallLaunchArrivalTime /
                    _ballLaunchArrivalTime;

                pinball.transform.position =
                    Vector3.Lerp(startPosition, _ballLaunchPoint, ratio);

                yield return 0;
            }

            pinball.transform.position = _ballLaunchPoint;
        }

        public void InstanceBallToWorkshopKOH(Pinball pinball)
        {
            pinball.transform.position = _workshopLocation.position;
            pinball.StopMotion();
            //pinball.SetPhysicsEnabled(false);
        }

        private void RemoveBall(Pinball pinball)
        {
            if (!OutOfBalls)
            {
                // Returns the ball next to the launcher
                // and gets whether the ball is lost
                InstanceNextBall(pinball);

                _currentBallCount--;

                if (_currentBallCount == 1)
                {
                    if (GameManager.Instance.GameMode.State ==
                        GameModeStateType.Sampo)
                    {
                        GameManager.Instance.GameMode.
                            Owner.PerformTransition(GameModeStateType.Normal);

                        // TODO: Reset collectable chances
                    }
                }
                else if (OutOfBalls)
                {
                    Viewscreen.OutOfBalls();
                    Debug.Log("Out of balls - game over");
                    //GameManager.Instance.SaveOrRevertHighscores(true);

                    // The game is ended by ScreenState_Play
                }
                else
                {
                    Viewscreen.BallCount(CurrentBallNumber);
                    Debug.Log("Balls left: " +
                        _currentBallCount.ToString());
                }
            }
            else
            {
                Debug.Log("Out of balls");
            }
        }

        /// <summary>
        /// Removes a ball for debugging purposes.
        /// The first active ball is removed.
        /// </summary>
        public void DebugLoseBall()
        {
            foreach (Pinball ball in _pinballs)
            {
                if (ball.gameObject.activeSelf)
                {
                    RemoveBall(ball);
                    return;
                }
            }
        }

        /// <summary>
        /// Resets a ball for debugging purposes.
        /// The first active ball is reset.
        /// </summary>
        public void DebugResetBall()
        {
            Tilt = false;

            foreach (Pinball ball in _pinballs)
            {
                if (ball.gameObject.activeSelf)
                {
                    ball.ResetBall();
                    return;
                }
            }
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
                ball = CreateNewBall();
            }
            // If it was found activate it.
            else
            {
                ball.gameObject.SetActive(true);
            }

            // Update active ball counter and set ball location.
            AdjustActiveBallCounter(true);
            ball.transform.position = location.position;

            // If given a valid impulse vector apply it.
            if (impulse != null && !impulse.Equals(Vector3.zero))
            {
                ball.AddImpulseForce(impulse);
            }
        }

        public void AdjustActiveBallCounter(bool increment)
        {
            _activeBalls += (increment ? 1 : -1);
            //Debug.Log("Active balls: " + _activeBalls);
        }

        public Pinball CreateNewBall()
        {
            Pinball ball = Instantiate(_pinballPrefab);
            ball.Init(true);
            _pinballs.Add(ball);
            return ball;
        }

        private void ActivateMultiball()
        {
            Debug.Log("Multiball mode activated");

            // Activates autosave for 15 seconds
            ActivateAutosave(15);

            // Activates extra ball spawner
            _extraBallSpawner.Activate(_workshopExtraBalls, false);
        }

        private IEnumerator WorkShopExtraBallRoutine()
        {
            //int addCount = 0;

            // Activates autosave for 15 seconds
            ActivateAutosave(15);

            while (_activeBalls - 1 < _workshopExtraBalls)
            {
                ExtraBall(_workshopLocation, Vector3.zero);
                //addCount++;

                yield return new WaitForSeconds(2f);
            }
        }

        public Pinball RecycleBall()
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
            if (!Launcher.Instance.BallOnLauncher &&
                !_noNudges && _nudgesLeft > 0)
            {
                _nudgesLeft--;

                if (_nudgesLeft == 0)
                {
                    Tilt = true;
                    Debug.Log("Tilt!");
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

        public void ChangeResources( int amount )
        {
            Resources = amount;
        }
    }
}