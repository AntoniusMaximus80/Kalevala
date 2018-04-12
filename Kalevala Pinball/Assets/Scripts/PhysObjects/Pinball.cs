using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kalevala.WaypointSystem;
using System;

namespace Kalevala
{
    public class Pinball : MonoBehaviour
    {
        // DEBUGGING
        public float debug_rampSpeed = 15;
        public bool debug_useDebugRampSpeed;

        public bool debug_freeBalls;
        public bool debug_addImpulseForce;
        public bool debug_stopMotion;
        public bool debug_exitRamp;
        //public bool debug_stopPhysics;
        //public bool debug_startPhysics;

        public Vector3 debug_upTableVelocity;

        public float _speed; // public for debugging
        private float _radius;
        private bool _physicsEnabled = true;

        private Path _ramp;
        private bool _dropAtEnd;
        private bool _heatBall;
        private bool _coolDown;
        private Color _heatColor = new Color(0.7411765f, 0.2156863f, 0.03921569f);
        private float _colorChangeDuration;
        private float _elapsedColorChangeDuration;
        private float _coolDownTimer;

        public bool IsInKickoutHole
        {
            get;
            set;
        }

        //private PinballManager _pbm;
        private Rigidbody _rb;
        private SphereCollider _sphColl;

        public event Action<bool> ExitingRamp;

        private void Awake()
        {
            Init(false);

            // Trying to improve physics by overriding the defaults for the ball.
            _rb.maxAngularVelocity = 70;
            _rb.maxDepenetrationVelocity = _rb.maxDepenetrationVelocity * 5;
            _rb.solverIterations = 12;
            _rb.solverVelocityIterations = 5;
            _sphColl.contactOffset = 0.1f;
          
        }

        public void Init(bool physicsEnabled)
        {
            _radius = GetComponent<Collider>().bounds.size.x / 2;
            _rb = GetComponent<Rigidbody>();
            _sphColl = GetComponent<SphereCollider>();

            debug_upTableVelocity = new Vector3(0f, 10 * 0.1742402f, 10 * -10.31068f);

            RampMotion = GetComponent<RampMotion>();

            SetPhysicsEnabled(physicsEnabled);
        }

        /// <summary>
        /// Updates the pinball if the game is not paused.
        /// </summary>
        public void UpdatePinball()
        {
            //// Does not update if the game is paused
            //if (GameManager.Instance.Screen != ScreenStateType.Play)
            //{
            //    return;
            //}

            //UpdatePause();
            if (_heatBall)
            {
                _heatBall = HeatUpBall(_heatColor);
            } else if (_coolDown)
            {
                _coolDown = CoolDownBall(_heatColor);
            }
            _speed = Speed;

            if (IsOnRamp)
            {
                bool rampEnded = !RampMotion.MoveAlongRamp();

                if (rampEnded)
                {
                    //Debug.Log("exiting ramp, ramp ended");
                    ExitRamp();
                }
            }

            if (!InputManager.NudgeVector.Equals(Vector3.zero))
                AddImpulseForce(InputManager.NudgeVector);

            PinballManager.Instance.CheckIfBallIsLost(this, debug_freeBalls);

            HandleDebug();
        }

        //private void UpdatePause()
        //{
        //    bool inPlayScreen =
        //        GameManager.Instance.Screen == ScreenStateType.Play;

        //    if (_physicsEnabled != inPlayScreen)
        //    {
        //        // NOTE: If the ball is paused with this (in the
        //        // current state), its velocity resets to zero
        //        SetPhysicsEnabled(inPlayScreen);
        //    }
        //}

        public RampMotion RampMotion { get; private set; }

        /// <summary>
        /// Gets the pinball's speed.
        /// If its physics are enabled, the physics velocity is returned.
        /// Otherwise its speed on ramp is returned.
        /// </summary>
        public float Speed
        {
            get
            {
                if (_physicsEnabled)
                {
                    return _rb.velocity.magnitude;
                }
                else
                {
                    return RampMotion._speed;
                }
            }
        }

        /// <summary>
        /// Gets the pinball's RigidBody's velocity.
        /// </summary>
        public Vector3 PhysicsVelocity
        {
            get
            {
                return _rb.velocity;
            }
        }

        /// <summary>
        /// Gets the pinball's radius.
        /// </summary>
        public float Radius
        {
            get
            {
                if (_radius == 0)
                {
                    _radius = GetComponent<Collider>().bounds.size.x / 2;
                }

                return _radius;
            }
        }

        public void StopMotion()
        {
            if (_physicsEnabled)
            {
                _rb.velocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }
        }

        public void SetPhysicsEnabled(bool enable)
        {
            if (enable)
            {
                if (!_physicsEnabled)
                {
                    _physicsEnabled = true;
                    _rb.isKinematic = false;
                    _rb.useGravity = true;
                    _sphColl.enabled = true;
                }
            }
            else if (_physicsEnabled)
            {
                StopMotion();
                _physicsEnabled = false;
                _rb.isKinematic = true;
                _rb.useGravity = false;
                _sphColl.enabled = false;
            }
        }

        public void AddImpulseForce(Vector3 force)
        {
            if (_physicsEnabled)
            {
                _rb.AddForce(force, ForceMode.Impulse);
                // Debug.Log(force);
                //Vector3 forcePosition = transform.position;
                //forcePosition.y += radius * 3f / 4f;
                //rb.AddForceAtPosition(force, forcePosition, ForceMode.Impulse);
            }
        }

        public bool IsOnRamp
        {
            get
            {
                return _ramp != null;
            }
        }

        public void SetHeatBall (float heatDuration)
        {
            _colorChangeDuration = heatDuration;
            _heatBall = true;
            _coolDown = true;
        }

        private bool HeatUpBall(Color heatcolor)
        {
            bool result = true;
            if (_elapsedColorChangeDuration < _colorChangeDuration) {
                _elapsedColorChangeDuration += Time.deltaTime;
                Material mat = GetComponent<Renderer>().material;
                Color color = heatcolor * (_elapsedColorChangeDuration / _colorChangeDuration);
                mat.SetColor("_EmissionColor", color);
            }
             else if(_elapsedColorChangeDuration >  _colorChangeDuration)
            {
                result = false;
                _coolDownTimer = 5f;
            }
            return result;
            
        }

        private bool CoolDownBall( Color heatcolor )
        {
            bool result = true;
            if(_coolDownTimer > 0)
            {
                _coolDownTimer -= Time.deltaTime;
            } else
            {
                _elapsedColorChangeDuration -= Time.deltaTime;
            }
            Material mat = GetComponent<Renderer>().material;
            Color color = heatcolor * (_elapsedColorChangeDuration / _colorChangeDuration);
            if(_elapsedColorChangeDuration <= 0)
            {
                result = false;
            }
            mat.SetColor("_EmissionColor", color);
            return result;
        } 

        public void EnterRamp(Path path, Direction direction, Waypoint startWP, bool dropAtEnd, KickoutHole kickoutHole)
        {
            float speedEnteringRamp = Speed;
            if(kickoutHole != null)
            {
                speedEnteringRamp = kickoutHole.KickForce;
            }
            else if (debug_useDebugRampSpeed)
            {
                speedEnteringRamp = debug_rampSpeed;
            }

            //Debug.Log("Ramp entered - speed: " + speedEnteringRamp);
            _ramp = path;
            _dropAtEnd = dropAtEnd;
            SetPhysicsEnabled(false);
            RampMotion.Activate(_ramp, direction, startWP, speedEnteringRamp, kickoutHole);
        }

        public void ExitRamp()
        {
            //Debug.Log("Ramp exited");
            float speedExitingRamp = Speed;
            RampMotion._speed = 0;
            if(!IsInKickoutHole)
            {
                SetPhysicsEnabled(true);
            }

            // Adds impulse force to the pinball to
            // keep the momentum it had on the ramp
            if (!_dropAtEnd)
            {
                AddImpulseForce(RampMotion.GetRampSegmentDirection() *
                    PinballManager.Instance.RampExitMomentumFactor *
                    speedExitingRamp);
            }
            if(ExitingRamp != null)
            {
                ExitingRamp(true);
            }
            _ramp = null;
        }

        private void AbortRamp()
        {
            if (IsOnRamp)
            {
                Debug.Log("ramp aborted");
                RampMotion.Deactivate();
                _dropAtEnd = true;
                ExitRamp();
            }
        }

        /// <summary>
        /// Returns the ball to the launcher regardless of where it is.
        /// </summary>
        public void ResetBall()
        {
            PinballManager.Instance.InstanceNextBall(this);
            StopMotion();
            AbortRamp();
            SetPhysicsEnabled(true);
        }

        //public bool SameDirections(Vector3 direction1, Vector3 direction2, float angleTolerance)
        //{
        //    bool result = false;

        //    if (Vector3.Angle(direction1, direction2) <= angleTolerance)
        //    {
        //        result = true;
        //    }

        //    return result;
        //}

        private void OnDrawGizmos()
        {
            DrawDirection();

            if (IsOnRamp)
            {
                DrawColoredWireSphere();
            }
        }

        private void DrawDirection()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.red;

                if (IsOnRamp)
                {
                    Gizmos.DrawLine(transform.position, transform.position + RampMotion.GetRampSegmentDirection() * _radius * 5);
                }
                else
                {
                    Gizmos.DrawLine(transform.position, transform.position + PhysicsVelocity.normalized * _radius * 5);
                }
            }
        }

        private void DrawLocalAxes()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.right * _radius * 3);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * _radius * 3);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * _radius * 3);
        }

        private void DrawColoredWireSphere()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _radius * 1.1f);
        }

        private void HandleDebug()
        {
            if (debug_stopMotion)
            {
                debug_stopMotion = false;
                StopMotion();
            }

            if (debug_exitRamp)
            {
                debug_exitRamp = false;
                AbortRamp();
            }

            //if (debug_stopPhysics)
            //{
            //    debug_stopPhysics = false;
            //    SetPhysicsEnabled(false);
            //}

            //if (debug_startPhysics)
            //{
            //    debug_startPhysics = false;
            //    SetPhysicsEnabled(true);
            //}

            if (debug_addImpulseForce)
            {
                debug_addImpulseForce = false;
                AddImpulseForce(debug_upTableVelocity);
            }
        }
    }
}
