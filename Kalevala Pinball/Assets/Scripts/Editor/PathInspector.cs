﻿using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Kalevala.WaypointSystem;

namespace Kalevala.Editor
{
    [UnityEditor.CustomEditor(typeof(Path))]
    public class PathInspector : UnityEditor.Editor
    {
        private Path targetPath;

        protected void OnEnable()
        {
            targetPath = target as Path;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            AddWaypointButton();
        }

        private void AddWaypointButton()
        {
            if (GUILayout.Button("Add Waypoint"))
            {
                int waypointCount = targetPath.transform.childCount;

                Waypoint[] waypoints =
                    targetPath.GetComponentsInChildren<Waypoint>();

                Waypoint prevWaypoint =
                    (waypoints.Length > 0 ?
                        waypoints[waypoints.Length - 1] : null);

                string waypointName =
                    string.Format("Waypoint{0}", (waypointCount + 1).ToString("D3"));

                // Creates the new waypoint
                GameObject waypoint = new GameObject(waypointName);
                waypoint.AddComponent<Waypoint>();
                waypoint.transform.SetParent(targetPath.transform);

                // Sets the waypoint's position and rotation
                waypoint.transform.position =
                    (prevWaypoint != null ?
                        prevWaypoint.Position : targetPath.transform.position);
                waypoint.transform.rotation = new Quaternion(0, 0, 0, 0);
            }
        }
    }
}