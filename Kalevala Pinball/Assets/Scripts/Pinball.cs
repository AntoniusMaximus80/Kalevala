using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kalevala.WaypointSystem;

namespace Kalevala
{
    public class Pinball : MonoBehaviour
    {
        public float debug_rampSpeed = 15;
        public bool debug_useDebugRampSpeed;

        public bool debug_autoReinstance;
        public bool debug_addImpulseForce;
        public bool debug_stopMotion;
        public bool debug_exitRamp;
        //public bool debug_stopPhysics;
        //public bool debug_startPhysics;

        public Vector3 debug_upTableVelocity;

        public float _speed;
        private float _radius;
        private bool _physicsEnabled = true;

        private Path _ramp;
        private bool _dropAtEnd;

        private PinballManager _pbm;
        private Rigidbody _rb;
        private SphereCollider _sphColl;

        private void Start()
        {
            _radius = GetComponent<Collider>().bounds.size.x / 2;
            _rb = GetComponent<Rigidbody>();
            _sphColl = GetComponent<SphereCollider>();

            debug_upTableVelocity = new Vector3(0f, 10 * 0.1742402f, 10 * -10.31068f);

            RampMotion = GetComponent<RampMotion>();
            _pbm = FindObjectOfType<PinballManager>();
            RampMotion.PinballManager = _pbm;
        }

        private void Update()
        {
            _speed = Speed;

            if (IsOnRamp)
            {
                bool rampEnded = RampMotion.MoveAlongRamp();

                if (rampEnded)
                {
                    ExitRamp();
                }
            }

            HandleDebug();
        }

        public RampMotion RampMotion { get; private set; }

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

        public Vector3 PhysicsVelocity
        {
            get
            {
                return _rb.velocity;
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

        public void EnterRamp(Path path, Direction direction, Waypoint startWP, bool dropAtEnd)
        {
            float speedEnteringRamp = Speed;

            if (debug_useDebugRampSpeed)
            {
                speedEnteringRamp = debug_rampSpeed;
            }

            //Debug.Log("Ramp entered - speed: " + speedEnteringRamp);
            _ramp = path;
            _dropAtEnd = dropAtEnd;
            SetPhysicsEnabled(false);
            RampMotion.Activate(_ramp, direction, startWP, speedEnteringRamp);
        }

        public void ExitRamp()
        {
            //Debug.Log("Ramp exited");
            float speedExitingRamp = Speed;
            SetPhysicsEnabled(true);

            // Testing
            if (!_dropAtEnd)
            {
                AddImpulseForce(RampMotion.GetRampSegmentDirection() * (3 * speedExitingRamp / 4));
            }

            _ramp = null;
        }

        private void ExitRamp_Debug()
        {
            if (IsOnRamp)
            {
                RampMotion.Deactivate();
                _dropAtEnd = true;
                ExitRamp();
            }
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
            //DrawDirection2();
            //DrawLocalAxes();

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
                    Gizmos.DrawLine(transform.position, transform.position + RampMotion.GetRampSegmentDirection() * _radius * 10);
                }
                else
                {
                    Gizmos.DrawLine(transform.position, transform.position + PhysicsVelocity.normalized * _radius * 10);
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
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                _pbm.InstanceNextBall(this);
                ExitRamp_Debug();
            }

            if (debug_autoReinstance &&
                _pbm.PositionIsInDrain(transform.position))
            {
                //Debug.Log("Ball is in drain");
                _pbm.InstanceNextBall(this);
            }

            if (debug_stopMotion)
            {
                debug_stopMotion = false;
                StopMotion();
            }

            if (debug_exitRamp)
            {
                debug_exitRamp = false;
                ExitRamp_Debug();
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
