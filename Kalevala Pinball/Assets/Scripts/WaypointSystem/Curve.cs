using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kalevala.WaypointSystem;

namespace Kalevala
{
    public class Curve : MonoBehaviour
    {
        [SerializeField]
        private Waypoint startWaypoint;

        [SerializeField]
        private Waypoint endWaypoint;

        [SerializeField]
        private Vector3[] points;

        [SerializeField, Range(1, 300)]
        private int lineSteps = 10;

        private Path parentPath;
        private Waypoint[] waypoints;

        //private bool resetWhenPossible = true;
        private bool waypointsCreated = false;

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

        public Waypoint StartWaypoint
        {
            get
            {
                return startWaypoint;
            }
        }

        public Waypoint EndWaypoint
        {
            get
            {
                return endWaypoint;
            }
        }

        public int LineSteps
        {
            get
            {
                return lineSteps;
            }
        }

        public Vector3 MidPoint { get; set; }

        public int PathChildIndex { get; set; }

        //public bool ResetWhenPossible
        //{
        //    get
        //    {
        //        return resetWhenPossible;
        //    }
        //    set
        //    {
        //        resetWhenPossible = value;
        //    }
        //}

        public bool WaypointsCreated
        {
            get
            {
                return waypointsCreated;
            }
            set
            {
                waypointsCreated = value;
            }
        }

        private void Start()
        {
            if (startWaypoint != null && endWaypoint != null)
            {
                parentPath = startWaypoint.transform.parent.GetComponent<Path>();

                if (parentPath == null)
                {
                    Debug.LogError("Parent path not found.");
                }
            }
            else
            {
                Debug.LogError("Start and/or end waypoint is not set.");
            }
        }

        public void ResetPoints()
        {
            if (true) //ResetWhenPossible)
            {
                Points = new Vector3[3];

                FixEnds();

                if (startWaypoint != null && endWaypoint != null)
                {
                    Points[1] = (endWaypoint.Position - startWaypoint.Position) * 0.5f;
                    MidPoint = Points[1];

                    //ResetWhenPossible = false;
                }

                waypointsCreated = false;
            }
            //else
            //{
            //    FixEnds();
            //}
        }

        public void FixEnds()
        {
            if (startWaypoint != null)
            {
                transform.position = startWaypoint.Position;
                Points[0] = Vector3.zero;

                if (endWaypoint != null)
                {
                    Points[2] = endWaypoint.Position - startWaypoint.Position;
                }
            }
        }

        public Vector3 GetPoint(float t)
        {
            return transform.TransformPoint(
                MathHelp.GetCurvePoint(Points[0], Points[1], Points[2], t));
        }

        /*
        if (curve.lineSteps <= 0)
        {
            curve.lineSteps = 10;
        }

        // Draws a curve from its start waypoint to its end waypoint
        for (int j = 0; j < curve.lineSteps; j++)
        {
            float step = j / (float) curve.lineSteps;
            float nextStep = (j + 1) / (float) curve.lineSteps;
            Gizmos.DrawLine(curve.GetPoint(step), curve.GetPoint(nextStep));
            DrawPinballSizeMarker(curve.GetPoint(step), pinballRadius);
        }
        */
    }
}
