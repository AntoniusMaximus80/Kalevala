using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class Pinball : MonoBehaviour
    {
        public bool debug_addImpulseForce;
        public bool debug_stopMotion;
        public bool debug_exitRamp;
        //public bool debug_stopPhysics;
        //public bool debug_startPhysics;

        public Vector3 debug_upTableVelocity;
        public float debug_vectorComparisonTolerance = 100;

        public float speed;
        public Vector3 rampVelocity;
        public Vector3 oldVelocity;

        private float radius;
        private bool physicsEnabled = true;

        private Ramp ramp;

        private Rigidbody rb;
        private SphereCollider sphColl;

        private void Start()
        {
            radius = GetComponent<Collider>().bounds.size.x / 2;
            rb = GetComponent<Rigidbody>();
            sphColl = GetComponent<SphereCollider>();

            debug_upTableVelocity = new Vector3(0f, 10 * 0.1742402f, 10 * -10.31068f);
        }

        private void Update()
        {
            speed = Speed;

            if (physicsEnabled)
            {
                oldVelocity = PhysicsVelocity;
            }

            if (debug_stopMotion)
            {
                debug_stopMotion = false;
                StopMotion();
            }

            if (debug_exitRamp)
            {
                debug_exitRamp = false;
                if (IsOnRamp)
                {
                    ExitRamp();
                }
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

        public float Speed
        {
            get
            {
                if (physicsEnabled)
                {
                    return rb.velocity.magnitude;
                }
                else
                {
                    return rampVelocity.magnitude;
                }
            }
        }

        public Vector3 PhysicsVelocity
        {
            get
            {
                return rb.velocity;
            }
        }

        public void StopMotion()
        {
            if (physicsEnabled)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        public void SetPhysicsEnabled(bool enable)
        {
            if (enable)
            {
                if (!physicsEnabled)
                {
                    physicsEnabled = true;
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    sphColl.enabled = true;
                }
            }
            else if (physicsEnabled)
            {
                StopMotion();
                physicsEnabled = false;
                rb.isKinematic = true;
                rb.useGravity = false;
                sphColl.enabled = false;
            }
        }

        public void AddImpulseForce(Vector3 force)
        {
            if (physicsEnabled)
            {
                rb.AddForce(force, ForceMode.Impulse);

                //Vector3 forcePosition = transform.position;
                //forcePosition.y += radius * 3f / 4f;
                //rb.AddForceAtPosition(force, forcePosition, ForceMode.Impulse);
            }
        }

        public bool IsOnRamp
        {
            get
            {
                return ramp != null;
            }
        }

        public void EnterRamp(Ramp ramp, Vector3 debug_rampDirection)
        {
            Debug.Log("Ramp entered");
            float speedEnteringRamp = Speed;
            this.ramp = ramp;
            SetPhysicsEnabled(false);

            // TODO: Moving on a rail
            // Direction from line, speed from speedEnteringRamp

            // Testing
            rampVelocity = debug_rampDirection * speedEnteringRamp;
            ExitRamp();
        }

        public void ExitRamp()
        {
            Debug.Log("Ramp exited");
            float speedExitingRamp = Speed;
            ramp = null;
            SetPhysicsEnabled(true);

            // TODO: Getting the right direction for impulse

            // Testing
            AddImpulseForce(rampVelocity);

            rampVelocity = Vector3.zero;
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
                //if (SameDirections(PhysicsVelocity, debug_directionVector, debug_vectorComparisonTolerance))
                //{
                //    Gizmos.color = Color.green;
                //}
                //else
                //{
                //    Gizmos.color = Color.red;
                //}

                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position + PhysicsVelocity.normalized * radius * 10);
            }
        }

        private void DrawLocalAxes()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.right * radius * 3);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * radius * 3);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * radius * 3);
        }

        private void DrawColoredWireSphere()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, radius * 1.1f);
        }
    }
}
