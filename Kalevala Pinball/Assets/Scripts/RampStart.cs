using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class RampStart : MonoBehaviour
    {
        public Vector3 startPoint;
        public Vector3 rampDirection;
        public float distanceTolerance = 0.2f;
        public float directionAngleTolerance = 30;

        public List<Pinball> pinballs;
        public Ramp ramp;

        /* The ramp start knows the ramp it is a part of
         * When a pinball hits, the ramp is given to it and it disables its physics
         * The pinball moves itself on the ramp using its speed, the ramp's line and gravity
         * The pinball either changes direction midway or reaches the end of the ramp
         * When the pinball should exit the ramp, it forgets the ramp, enables its physics
           and adds force to itself based on the speed it had on the ramp
         */

        private void Start()
        {
            rampDirection.Normalize();
        }

        private void Update()
        {
            foreach (Pinball ball in pinballs)
            {
                if (!ball.IsOnRamp)
                {
                    if (Hit(ball.transform.position))
                    {
                        if (SameDirections(ball.PhysicsVelocity))
                        {
                            ball.EnterRamp(ramp, rampDirection);
                        }
                    }
                }
            }
        }

        public bool Hit(Vector3 objPosition)
        {
            bool result = false;

            if (Vector3.Distance(transform.position + startPoint, objPosition) < distanceTolerance)
            {
                result = true;
            }

            return result;
        }

        public bool SameDirections(Vector3 ballDirection)
        {
            bool result = false;

            if (Vector3.Angle(ballDirection, rampDirection) <= directionAngleTolerance)
            {
                result = true;
            }

            return result;
        }

        private void OnDrawGizmos()
        {
            DrawStartPoint();
            DrawDirection();
        }

        private void DrawStartPoint()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position + startPoint, 0.8f);
        }

        private void DrawDirection()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position + startPoint, transform.position + startPoint + rampDirection * 8);
        }
    }
}
