using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Kalevala.WaypointSystem;

namespace Kalevala.Editor
{
    [UnityEditor.CustomEditor(typeof(Waypoint))]
    public class WaypointInspector : UnityEditor.Editor
    {
        private Waypoint targetWaypoint;

        protected void OnEnable()
        {
            targetWaypoint = target as Waypoint;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            InsertWaypointButton();
            //InsertCurveButton();
        }

        private void InsertWaypointButton()
        {
            if (GUILayout.Button("Insert Waypoint Between This And Next"))
            {
                // The new, inserted waypoint
                Waypoint newWaypoint = null;

                // The parent path
                Path parentPath = targetWaypoint.GetComponentInParent<Path>();

                // The list of existing waypoints
                Waypoint[] waypoints =
                    parentPath.GetComponentsInChildren<Waypoint>();

                int targetWPIndex = GetTargetWaypointIndex(waypoints);
                int waypointsAfterCount = waypoints.Length - targetWPIndex - 1;

                // Checks if the target waypoint is valid and returns if not
                if (targetWPIndex == -1 || waypointsAfterCount < 1)
                {
                    Debug.LogError("Cannot insert a new waypoint after the " +
                                   "last one. To do this, use the Path's " +
                                   "Add Waypoint button.");
                    return;
                }
                
                // List of waypoints with the new one included
                Waypoint[] alteredWaypoints = new Waypoint[waypoints.Length + 1];

                for (int i = 0; i < alteredWaypoints.Length; i++)
                {
                    // The name of the current waypoint
                    string waypointName =
                        string.Format("Waypoint{0}", (i + 1).ToString("D3"));

                    // If a waypoint comes before the target
                    // waypoint, it is only added to the new list
                    if (i < targetWPIndex + 1)
                    {
                        alteredWaypoints[i] = waypoints[i];
                    }
                    else
                    {
                        // Creates the new waypoint
                        if (i == targetWPIndex + 1)
                        {
                            GameObject waypoint = new GameObject(waypointName);
                            newWaypoint = waypoint.AddComponent<Waypoint>();
                            waypoint.transform.SetParent(parentPath.transform);

                            // The new waypoint's position is between
                            // the target waypoint and the next waypoint
                            waypoint.transform.position = Vector3.Lerp(
                                targetWaypoint.Position, waypoints[i].Position, 0.5f);
                            waypoint.transform.rotation = new Quaternion(0, 0, 0, 0);

                            alteredWaypoints[i] = newWaypoint;
                        }
                        // Renames following waypoints to keep them in order
                        else if (i > targetWPIndex + 1)
                        {
                            alteredWaypoints[i] = waypoints[i - 1];
                            alteredWaypoints[i].gameObject.name = waypointName;
                        }

                        // Reorganizes the waypoint list in editor
                        alteredWaypoints[i].transform.SetSiblingIndex(i);
                    }
                }

                // Selects the new waypoint in editor
                Selection.activeGameObject = newWaypoint.gameObject;
            }
        }

        private void InsertBetweenWaypoints(bool insertCurve)
        {
            // The parent path
            Path parentPath = targetWaypoint.GetComponentInParent<Path>();

            // The list of existing waypoints
            Waypoint[] waypoints =
                parentPath.GetComponentsInChildren<Waypoint>();

            int targetWPIndex = GetTargetWaypointIndex(waypoints);
            int waypointsAfterCount = waypoints.Length - targetWPIndex - 1;

            // Checks if the target waypoint is valid and returns if not
            if (targetWPIndex == -1 || waypointsAfterCount < 1)
            {
                Debug.LogError("Cannot insert a new waypoint after the " +
                               "last one. To do this, use the Path's " +
                               "Add Waypoint button.");
                return;
            }

            if (insertCurve)
            {
                // TODO
            }
            else
            {
                // TODO
            }
        }

        private int GetTargetWaypointIndex(Waypoint[] waypoints)
        {
            int targetWPIndex = -1;

            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] == targetWaypoint)
                {
                    targetWPIndex = i;
                    break;
                }
            }

            return targetWPIndex;
        }

        //private void InsertCurveButton()
        //{
        //    if (GUILayout.Button("Insert Curve"))
        //    {
        //        int waypointCount = targetPath.transform.childCount;

        //        Waypoint[] waypoints =
        //            targetPath.GetComponentsInChildren<Waypoint>();

        //        Waypoint prevWaypoint =
        //            (waypoints.Length > 0 ?
        //                waypoints[waypoints.Length - 1] : null);

        //        string waypointName =
        //            string.Format("Waypoint{0}", (waypointCount + 1).ToString("D3"));

        //        // Creates the new waypoint
        //        GameObject waypoint = new GameObject(waypointName);
        //        waypoint.AddComponent<Waypoint>();
        //        waypoint.transform.SetParent(targetPath.transform);

        //        // Sets the waypoint's position and rotation
        //        waypoint.transform.position =
        //            (prevWaypoint != null ?
        //                prevWaypoint.Position : targetPath.transform.position);
        //        waypoint.transform.rotation = new Quaternion(0, 0, 0, 0);
        //    }
        //}
    }
}
