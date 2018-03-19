using UnityEngine;
using UnityEditor;
using Kalevala.WaypointSystem;

namespace Kalevala.Editor
{
    [UnityEditor.CustomEditor(typeof(RampEntrance))]
    public class RampEntranceInspector : UnityEditor.Editor
    {
        private RampEntrance targetRampEntrance;
        private Transform targetTransform;
        private Quaternion handleQuaternion;

        protected void OnEnable()
        {
            targetRampEntrance = target as RampEntrance;
            targetTransform = targetRampEntrance.transform;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            FixPositionButton();
        }

        private void FixPositionButton()
        {
            string whichWaypoint =
                (targetRampEntrance.IsPathStart ? "First" : "Last");
            string buttonName =
                string.Format("Fix Position ({0} Waypoint)", whichWaypoint);

            if (GUILayout.Button(buttonName))
            {
                // The path is a child of the path start RampEntrance
                Path path;

                Waypoint[] waypoints;

                if (targetRampEntrance.IsPathStart)
                {
                    path = targetRampEntrance.GetComponentInChildren<Path>();

                    waypoints =
                        path.GetComponentsInChildren<Waypoint>();

                    // Resets the path's position
                    path.transform.localPosition = Vector3.zero;

                    // Resets the path's first waypoint's position
                    waypoints[0].transform.localPosition = Vector3.zero;
                }
                else
                {
                    path = targetRampEntrance.transform.
                        parent.GetComponentInChildren<Path>();

                    waypoints =
                        path.GetComponentsInChildren<Waypoint>();

                    int waypointIndex = waypoints.Length - 1;
                    Waypoint waypoint = waypoints[waypointIndex];

                    // Resets the path's position
                    path.transform.localPosition = Vector3.zero;

                    // Sets the ramp entrance's position to
                    // be the same as the last waypoint's
                    targetRampEntrance.transform.position = waypoint.Position;
                }
            }
        }

        private void OnSceneGUI()
        {
            DisplayDirectionHandle();
        }

        private void DisplayDirectionHandle()
        {
            handleQuaternion = Tools.pivotRotation == PivotRotation.Local ?
                targetTransform.rotation : Quaternion.identity;

            Vector3 dirHandlePoint = ShowPoint(true);
        }

        private Vector3 ShowPoint(bool showHandle)
        {
            // Transforms the position into world coordinates
            Vector3 point = targetTransform.
                TransformPoint(targetRampEntrance.RampDirection);

            if (showHandle)
            {
                EditorGUI.BeginChangeCheck();
                point = Handles.DoPositionHandle(point, handleQuaternion);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(targetRampEntrance, "Move Point");
                    //EditorUtility.SetDirty(targetRampEntrance);
                    targetRampEntrance.RampDirection =
                        targetTransform.InverseTransformPoint(point);
                }
            }

            return point;
        }
    }
}
