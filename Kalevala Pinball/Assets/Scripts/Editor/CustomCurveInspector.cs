using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Kalevala.WaypointSystem;

namespace Kalevala
{
    [CustomEditor(typeof(CustomCurve))]
    public class CustomCurveInspector : UnityEditor.Editor
    {
        private CustomCurve targetCurve;
        private Transform handleTransform;
        private Quaternion handleQuaternion;

        protected void OnEnable()
        {
            targetCurve = target as CustomCurve;
            handleTransform = targetCurve.transform;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ResetButton();
        }

        private void OnSceneGUI()
        {
            DisplayCurveHandle();
        }

        private void ResetButton()
        {
            if (GUILayout.Button("Reset"))
            {
                targetCurve.ResetPoints();

                // Makes changes appear instantly in the Scene view
                // (a stupid trick I wish wasn't necessary)
                Vector3 temp = targetCurve.transform.position;
                targetCurve.transform.position += Vector3.up;
                targetCurve.transform.position = temp;
            }
        }

        private void DisplayCurveHandle()
        {
            if (targetCurve.Points.Length < 3)
            {
                return;
            }

            // Sets the handle's rotation based on
            // if Local or Global mode is active
            handleQuaternion = Tools.pivotRotation == PivotRotation.Local ?
                handleTransform.rotation : Quaternion.identity;

            Vector3 p0 = ShowPoint(0, true);
            Vector3 p1 = ShowPoint(1, true);
            Vector3 p_end1 = Vector3.zero;
            Vector3 p_end0 = Vector3.zero;

            for (int i = 1; i < targetCurve.Points.Length; i++)
            {
                Vector3 p = ShowPoint(i, true);

                if (i == targetCurve.Points.Length - 2)
                {
                    p_end1 = p;
                }
                else if (i == targetCurve.Points.Length - 1)
                {
                    p_end0 = p;
                }
            }

            Handles.color = Color.gray;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p_end1, p_end0);

            Handles.color = Color.white;
            Vector3 lineStart = targetCurve.GetPoint(0f);
            for (int i = 0; i < targetCurve.LineSteps; i++)
            {
                Vector3 lineEnd = targetCurve.GetPoint
                    ((i + 1) / (float) (targetCurve.LineSteps + 1));
                Handles.DrawLine(lineStart, lineEnd);
                lineStart = lineEnd;
            }

            Handles.DrawLine(lineStart, targetCurve.GetPoint(1));
        }

        private Vector3 ShowPoint(int index, bool showHandle)
        {
            // Transforms the position into world coordinates
            Vector3 point = handleTransform.
                TransformPoint(targetCurve.Points[index]);

            if (showHandle)
            {
                EditorGUI.BeginChangeCheck();
                point = Handles.DoPositionHandle(point, handleQuaternion);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(targetCurve, "Move Point");
                    EditorUtility.SetDirty(targetCurve);
                    targetCurve.Points[index] =
                        handleTransform.InverseTransformPoint(point);
                }
            }

            return point;
        }
    }
}
