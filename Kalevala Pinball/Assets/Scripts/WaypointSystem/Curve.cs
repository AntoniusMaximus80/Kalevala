using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kalevala.WaypointSystem;

namespace Kalevala
{
    public class Curve : MonoBehaviour
    {
        public Waypoint[] waypoints;
        public Vector3 midPoint;

        public int pathChildIndex;
        public int lineSteps = 10;

        private Vector3[] points;
        public Vector3[] Points
        {
            get
            {
                if (points == null)
                {
                    points = new Vector3[3];
                }

                return points;
            }
            set
            {
                points = value;
            }
        }

        private void Start()
        {
            ResetPosition();
        }

        public void ResetPosition()
        {
            // TODO: (Editor) Fix not being updated immediately if a connected waypoint has been moved

            transform.position = waypoints[0].Position;

            Points = new Vector3[3];
            Points[0] = Vector3.zero;
            Points[2] = waypoints[1].Position - waypoints[0].Position;
            Points[1] = midPoint;
        }

        public Vector3 GetPoint(float t)
        {
            return transform.TransformPoint(
                MathHelp.GetCurvePoint(Points[0], Points[1], Points[2], t));
        }
    }
}
