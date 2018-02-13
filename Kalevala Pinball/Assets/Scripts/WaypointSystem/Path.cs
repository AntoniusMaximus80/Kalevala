using UnityEngine;
using System.Collections.Generic;
using System;

namespace Kalevala.WaypointSystem
{
    public enum PathType
    {
        Loop, // After reaching the last waypoint, the user moves to the first
        PingPong, // The user changes direction after reaching the last waypoint
        OneShot // After reaching the last waypoint, the user exits the path
    }

    public enum Direction
    {
        Forward,
        Backward
    }

    public class Path : MonoBehaviour
    {
        [SerializeField]
        private PathType _pathType;

        [SerializeField]
        private Color _pathColor = Color.red;

        [SerializeField]
        private bool _drawPath = true;

        private List<Waypoint> _waypoints;
        private List<Curve> _curves;
        private Pinball _pinball;

        // Dictionary which defines a color for each path type.
        //private readonly Dictionary<PathType, Color> _pathColors =
        //    new Dictionary<PathType, Color>()
        //    {
        //        { PathType.Loop, Color.yellow },
        //        { PathType.PingPong, Color.red }
        //    };

        public List<Waypoint> Waypoints
        {
            get
            {
                // Populates the waypoints if not done that yet and in editor every time
                if (_waypoints == null ||
                    _waypoints.Count == 0 ||
                    !Application.isPlaying)
                {
                    PopulateWaypoints();
                }
                return _waypoints;
            }
        }

        public Waypoint GetFirstWaypoint()
        {
            if (Waypoints.Count > 0)
            {
                return Waypoints[0];
            }
            else
            {
                return null;
            }
        }

        public Waypoint GetLastWaypoint()
        {
            if (Waypoints.Count > 0)
            {
                return Waypoints[Waypoints.Count - 1];
            }
            else
            {
                return null;
            }
        }

        public Waypoint GetClosestWaypoint(Vector3 position)
        {
            float smallestSqrMagnitude = float.PositiveInfinity;
            Waypoint closest = null;
            for (int i = 0; i < Waypoints.Count; i++)
            {
                Waypoint waypoint = Waypoints[i];
                Vector3 toWaypointVector = waypoint.Position - position;
                float currentSqrMagnitude = toWaypointVector.sqrMagnitude;
                if (currentSqrMagnitude < smallestSqrMagnitude)
                {
                    closest = waypoint;
                    smallestSqrMagnitude = currentSqrMagnitude;
                }
            }

            return closest;
        }

        public Waypoint GetNextWaypoint(Waypoint currentWaypoint,
            ref Direction direction)
        {
            Waypoint nextWaypoint = null;
            for (int i = 0; i < Waypoints.Count; i++)
            {
                if (Waypoints[i] == currentWaypoint)
                {
                    switch (_pathType)
                    {
                        case PathType.Loop:
                        {
                            nextWaypoint = GetNextWaypointLoop(i, direction);
                            break;
                        }
                        case PathType.PingPong:
                        {
                            nextWaypoint = GetNextWaypointPingPong(i, ref direction);
                            break;
                        }
                        case PathType.OneShot:
                        {
                            nextWaypoint = GetNextWaypointOneShot(i, direction);
                            break;
                        }
                    }
                    break;
                }
            }
            return nextWaypoint;
        }

        private Waypoint GetNextWaypointPingPong(int currentWaypointIndex,
            ref Direction direction)
        {
            Waypoint nextWaypoint = null;
            switch (direction)
            {
                case Direction.Forward:
                {
                    if (currentWaypointIndex < Waypoints.Count - 1)
                    {
                        nextWaypoint = Waypoints[currentWaypointIndex + 1];
                    }
                    else
                    {
                        nextWaypoint = Waypoints[currentWaypointIndex - 1];
                        direction = Direction.Backward;
                    }
                    break;
                }
                case Direction.Backward:
                {
                    if (currentWaypointIndex > 0)
                    {
                        nextWaypoint = Waypoints[currentWaypointIndex - 1];
                    }
                    else
                    {
                        nextWaypoint = Waypoints[1];
                        direction = Direction.Forward;
                    }
                    break;
                }
            }
            return nextWaypoint;
        }

        private Waypoint GetNextWaypointLoop(int currentWaypointIndex,
            Direction direction)
        {
            Waypoint nextWaypoint = direction == Direction.Forward
                ? Waypoints[++currentWaypointIndex % Waypoints.Count]
                : Waypoints[((--currentWaypointIndex >= 0) ? currentWaypointIndex : Waypoints.Count - 1) % Waypoints.Count];
            return nextWaypoint;
        }

        private Waypoint GetNextWaypointOneShot(int currentWaypointIndex,
            Direction direction)
        {
            Waypoint nextWaypoint = null;
            switch (direction)
            {
                case Direction.Forward:
                {
                    if (currentWaypointIndex < Waypoints.Count - 1)
                    {
                        nextWaypoint = Waypoints[currentWaypointIndex + 1];
                    }
                    break;
                }
                case Direction.Backward:
                {
                    if (currentWaypointIndex > 0)
                    {
                        nextWaypoint = Waypoints[currentWaypointIndex - 1];
                    }
                    break;
                }
            }
            return nextWaypoint;
        }

        private void PopulateWaypoints()
        {
            int childCount = transform.childCount;
            _waypoints = new List<Waypoint>(childCount);
            _curves = new List<Curve>();
            for (int i = 0; i < childCount; i++)
            {
                Transform childTransform = transform.GetChild(i);

                Waypoint waypoint = childTransform.GetComponent<Waypoint>();
                if (waypoint != null)
                {
                    _waypoints.Add(waypoint);
                }
                else
                {
                    Curve curve = childTransform.GetComponent<Curve>();
                    if (curve != null)
                    {
                        curve.pathChildIndex = i;
                        _curves.Add(curve);
                    }
                }
            }
        }

        public float Length
        {
            get
            {
                float length = 0;

                // Length for a path without curves
                for (int i = 1; i < Waypoints.Count; i++)
                {
                    length += Vector3.Distance(Waypoints[i + 1].Position, Waypoints[i].Position);
                }

                // TODO: Length for a path with curves

                return length;
            }
        }

        public float GetSegmentLength(Waypoint waypoint, bool start)
        {
            Waypoint startWaypoint = waypoint;
            Waypoint endWaypoint = null;
            int otherWaypointIndex = Waypoints.IndexOf(waypoint) + (start ? 1 : -1);

            if (otherWaypointIndex < 0 || otherWaypointIndex >= Waypoints.Count)
            {
                return 0;
            }

            if (start)
            {
                endWaypoint = Waypoints[otherWaypointIndex];
            }
            else
            {
                startWaypoint = Waypoints[otherWaypointIndex];
                endWaypoint = waypoint;
            }

            // Length for a straight segment
            float length = Vector3.Distance(startWaypoint.Position,
                endWaypoint.Position);

            // TODO: Length for a curved segment

            return length;
        }

        public Color PathColor
        {
            get
            {
                return _pathColor;
            }
            set
            {
                _pathColor = value;
            }
        }

        private Curve CurveInIndex(int index)
        {
            for (int i = 0; i < _curves.Count; i++)
            {
                if (_curves[i].pathChildIndex == index)
                {
                    return _curves[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Draws lines between waypoints
        /// </summary>
        protected void OnDrawGizmos()
        {
            if (_drawPath)
            {
                if (_pinball == null)
                {
                    _pinball = FindObjectOfType<Pinball>();
                }

                float radius = _pinball.Radius;

                DrawPath(radius);

                if (radius > 0)
                {
                    DrawWaypoints(radius);
                }
            }
        }

        private void DrawPath(float pinballRadius)
        {
            Gizmos.color = _pathColor;
            //Gizmos.color = _pathColors[_pathType];

            if (Waypoints.Count > 1)
            {
                for (int i = 1; i < Waypoints.Count; i++)
                {
                    Curve curve = CurveInIndex(i);

                    if (curve == null)
                    {
                        // Draws a line from previous waypoint to current
                        Gizmos.DrawLine(Waypoints[i - 1].Position, Waypoints[i].Position);
                    }
                    else
                    {
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
                    }
                }
                if (_pathType == PathType.Loop)
                {
                    // From last waypoint to first 
                    Gizmos.DrawLine(Waypoints[Waypoints.Count - 1].Position,
                        Waypoints[0].Position);
                }
            }
        }

        private void DrawWaypoints(float pinballRadius)
        {
            Gizmos.color = _pathColor;
            //Gizmos.color = _pathColors[_pathType];

            if (Waypoints.Count > 1)
            {
                for (int i = 1; i < Waypoints.Count; i++)
                {
                    DrawPinballSizeMarker(Waypoints[i].Position, pinballRadius);
                }
            }
        }

        private void DrawPinballSizeMarker(Vector3 position, float radius)
        {
            // Draw lines on x-axis
            Gizmos.DrawLine(position + Vector3.right * radius, position + Vector3.right * radius / 2);
            Gizmos.DrawLine(position - Vector3.right * radius, position - Vector3.right * radius / 2);

            // Draw lines on y-axis
            Gizmos.DrawLine(position + Vector3.up * radius, position + Vector3.up * radius / 2);
            Gizmos.DrawLine(position - Vector3.up * radius, position - Vector3.up * radius / 2);

            // Draw lines on z-axis
            Gizmos.DrawLine(position + Vector3.forward * radius, position + Vector3.forward * radius / 2);
            Gizmos.DrawLine(position - Vector3.forward * radius, position - Vector3.forward * radius / 2);

            //Vector3 direction = (Waypoints[i].Position - Waypoints[i - 1].Position).normalized;

            //Vector3 horizVector = direction;
            //horizVector.y = 0;
            //horizVector.Normalize();

            //Vector3 vertVector = direction;
            //horizVector.x = 0;
            //vertVector.Normalize();

            //// Draw horizontal line
            //Gizmos.DrawLine(Waypoints[i].Position + horizVector * radius, Waypoints[i].Position - horizVector * radius);

            //// Draw vertical line
            //Gizmos.DrawLine(Waypoints[i].Position + vertVector * radius, Waypoints[i].Position - vertVector * radius);
        }
    }
}
