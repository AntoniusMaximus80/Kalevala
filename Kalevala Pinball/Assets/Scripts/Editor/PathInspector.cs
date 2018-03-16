using UnityEngine;
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
            FixWaypointNamesButton();
        }

        private void AddWaypointButton()
        {
            if (GUILayout.Button("Add Waypoint"))
            {
                targetPath.AddWaypoint();
            }
        }

        private void FixWaypointNamesButton()
        {
            if (GUILayout.Button("Fix Waypoint Names"))
            {
                targetPath.UpdateWaypointNames();
            }
        }
    }
}
