using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Kalevala.WaypointSystem;

namespace Kalevala
{
    [CustomEditor(typeof(Curve))]
    public class CurveInspector : UnityEditor.Editor
    {
        private Curve targetCurve;
        private Transform handleTransform;
        private Quaternion handleQuaternion;

        protected void OnEnable()
        {
            targetCurve = target as Curve;
            handleTransform = targetCurve.transform;

            //EditorApplication.update += Update;
        }

        //protected void OnDisable()
        //{
        //    EditorApplication.update -= Update;
        //}

        //private void Update()
        //{
        //    // ...
        //}

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            FixEndsButton();
            ResetButton();
            CreateWaypointsButton();
        }

        private void OnSceneGUI()
        {
            DisplayHandles();
        }

        private void CreateWaypointsButton()
        {
            if (targetCurve.StartWaypoint != null)
            {
                if (GUILayout.Button("Create Waypoints"))
                {
                    if (targetCurve.WaypointsCreated)
                    {
                        Debug.LogWarning("Be aware that waypoints created " +
                            "from this curve may already exist.");
                    }

                    // The parent path
                    Path path = targetCurve.transform.
                    parent.parent.GetComponentInChildren<Path>();

                    Vector3[] waypointPositions =
                        new Vector3[targetCurve.LineSteps - 1];

                    // Adds points to the waypoint position array
                    // (the start and end points are not included
                    // because they already exist as waypoints)
                    for (int i = 1; i < targetCurve.LineSteps; i++)
                    {
                        waypointPositions[i - 1] = targetCurve.GetPoint
                            (i / (float) targetCurve.LineSteps);
                    }

                    // Inserts waypoints on the curve
                    path.InsertWaypoints(targetCurve.StartWaypoint,
                        waypointPositions, targetCurve.name);

                    targetCurve.WaypointsCreated = true;
                }
            }
        }

        private void FixEndsButton()
        {
            if (GUILayout.Button("Fix Ends"))
            {
                targetCurve.FixEnds();
            }
        }

        private void ResetButton()
        {
            if (GUILayout.Button("Reset"))
            {
                //targetCurve.ResetWhenPossible = true;
                targetCurve.ResetPoints();

                // Makes changes appear instantly in the Scene view
                // (a stupid trick I wish wasn't necessary)
                Vector3 temp = targetCurve.transform.position;
                targetCurve.transform.position += Vector3.up;
                targetCurve.transform.position = temp;
            }
        }

        private void DisplayHandles()
        {
            handleQuaternion = Tools.pivotRotation == PivotRotation.Local ?
                handleTransform.rotation : Quaternion.identity;

            Vector3 p0 = ShowPoint(0);
            Vector3 p1 = ShowPoint(1);
            Vector3 p2 = ShowPoint(2);

            Handles.color = Color.gray;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p1, p2);

            Handles.color = Color.white;
            Vector3 lineStart = targetCurve.GetPoint(0f);
            for (int i = 1; i <= targetCurve.LineSteps; i++)
            {
                Vector3 lineEnd = targetCurve.GetPoint(i / (float) targetCurve.LineSteps);
                Handles.DrawLine(lineStart, lineEnd);
                lineStart = lineEnd;
            }
        }

        private Vector3 ShowPoint(int index)
        {
            Vector3 point = handleTransform.TransformPoint(targetCurve.Points[index]);

            if (index == 1)
            {
                EditorGUI.BeginChangeCheck();
                point = Handles.DoPositionHandle(point, handleQuaternion);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(targetCurve, "Move Point");
                    EditorUtility.SetDirty(targetCurve);
                    targetCurve.Points[index] = handleTransform.InverseTransformPoint(point);
                }
            }

            return point;
        }
    }
}
