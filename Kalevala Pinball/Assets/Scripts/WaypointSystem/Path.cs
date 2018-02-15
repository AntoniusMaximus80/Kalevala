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
                //else
                //{
                //    Curve curve = childTransform.GetComponent<Curve>();
                //    if (curve != null)
                //    {
                //        curve.PathChildIndex = i;
                //        _curves.Add(curve);
                //    }
                //}
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

        //private Curve CurveInIndex(int index)
        //{
        //    for (int i = 0; i < _curves.Count; i++)
        //    {
        //        if (_curves[i].PathChildIndex == index)
        //        {
        //            return _curves[i];
        //        }
        //    }

        //    return null;
        //}

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
                    // Draws a line from previous waypoint to current
                    Gizmos.DrawLine(Waypoints[i - 1].Position, Waypoints[i].Position);
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
            Gizmos.DrawLine(position + Vector3.right * radius, position + 3 * Vector3.right * radius / 4);
            Gizmos.DrawLine(position - Vector3.right * radius, position - 3 * Vector3.right * radius / 4);

            // Draw lines on y-axis
            Gizmos.DrawLine(position + Vector3.up * radius, position + 3 * Vector3.up * radius / 4);
            Gizmos.DrawLine(position - Vector3.up * radius, position - 3 * Vector3.up * radius / 4);

            // Draw lines on z-axis
            Gizmos.DrawLine(position + Vector3.forward * radius, position + 3 * Vector3.forward * radius / 4);
            Gizmos.DrawLine(position - Vector3.forward * radius, position - 3 * Vector3.forward * radius / 4);
        }




        public Waypoint InsertWaypoints(Waypoint targetWaypoint,
            Vector3[] waypointPositions, string curveName)
        {
            bool curve = (curveName.Length > 0);

            // Default insert
            bool defaultInsert = false;
            if (waypointPositions.Length < 1)
            {
                defaultInsert = true;
                waypointPositions = new Vector3[1];
            }

            int targetWPIndex = GetTargetWaypointIndex(targetWaypoint);
            int waypointsAfterCount = _waypoints.Count - targetWPIndex - 1;

            // Checks if the target waypoint is valid and returns if not
            if (targetWPIndex == -1 || waypointsAfterCount < 1)
            {
                Debug.LogError("Cannot insert a new waypoint after the " +
                               "last one. To do this, use the Path's " +
                               "Add Waypoint button.");
                return null;
            }

            // List of waypoints with the new ones included
            Waypoint[] alteredWaypoints = new Waypoint[_waypoints.Count + waypointPositions.Length];

            // A new, inserted waypoint
            Waypoint newWaypoint = null;

            for (int i = 0; i < alteredWaypoints.Length; i++)
            {
                // The name of the current waypoint
                string waypointName =
                    string.Format("Waypoint{0}", (i + 1).ToString("D3"));

                // If a waypoint comes before the target
                // waypoint, it is only added to the new list
                if (i <= targetWPIndex)
                {
                    alteredWaypoints[i] = _waypoints[i];
                }
                else
                {
                    // Creates a new waypoint
                    if (i > targetWPIndex && i <= targetWPIndex + waypointPositions.Length)
                    {
                        if (curve)
                        {
                            waypointName += string.Format(" ({0})", curveName);
                        }

                        GameObject waypoint = new GameObject(waypointName);
                        newWaypoint = waypoint.AddComponent<Waypoint>();
                        waypoint.transform.SetParent(transform);
                        newWaypoint.IsPartOfCurve = curve;
                        newWaypoint.CurveName = curveName;

                        // Sets the new waypoint's position
                        if (!defaultInsert)
                        {
                            waypoint.transform.position = waypointPositions[i - targetWPIndex - 1];
                        }
                        // Default position
                        else
                        {
                            waypoint.transform.position = Vector3.Lerp(
                                targetWaypoint.Position, _waypoints[i].Position, 0.5f);
                        }

                        waypoint.transform.rotation = new Quaternion(0, 0, 0, 0);

                        alteredWaypoints[i] = newWaypoint;
                    }
                    // Renames following waypoints to keep them in order
                    else
                    {
                        alteredWaypoints[i] = _waypoints[i - waypointPositions.Length];

                        if (alteredWaypoints[i].IsPartOfCurve)
                        {
                            waypointName += string.Format(" ({0})",
                                alteredWaypoints[i].CurveName);
                        }
                        alteredWaypoints[i].gameObject.name = waypointName;
                    }

                    // Reorganizes the waypoint list in editor
                    alteredWaypoints[i].transform.SetSiblingIndex(i);
                }
            }

            return newWaypoint;
        }

        private int GetTargetWaypointIndex(Waypoint targetWaypoint)
        {
            int targetWPIndex = -1;

            for (int i = 0; i < _waypoints.Count; i++)
            {
                if (_waypoints[i] == targetWaypoint)
                {
                    targetWPIndex = i;
                    break;
                }
            }

            return targetWPIndex;
        }
    }
}
