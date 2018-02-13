using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Kalevala.WaypointSystem;

namespace Kalevala.Editor
{
    [UnityEditor.CustomEditor(typeof(Curve))]
    public class CurveInspector : UnityEditor.Editor
    {
        private Curve targetCurve;

        protected void OnEnable()
        {
            targetCurve = target as Curve;
            targetCurve.ResetPosition();

            EditorApplication.update += Update;
        }

        protected void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        private void Update()
        {
            targetCurve.Points[1] = targetCurve.midPoint;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ResetPositionButton();
            ResetCurveButton();
        }

        private void ResetPositionButton()
        {
            if (GUILayout.Button("Reset Position"))
            {
                targetCurve.ResetPosition();
            }
        }

        private void ResetCurveButton()
        {
            if (GUILayout.Button("Reset Curve"))
            {
                ResetCurve();
            }
        }

        private void ResetCurve()
        {
            targetCurve.midPoint = Vector3.Lerp(targetCurve.Points[0], targetCurve.Points[2], 0.5f);
            targetCurve.Points[1] = targetCurve.midPoint;

            // Makes changes appear instantly in the Scene view
            // (a stupid trick I wish wasn't necessary)
            Vector3 temp = targetCurve.transform.position;
            targetCurve.transform.position += Vector3.up;
            targetCurve.transform.position = temp;
        }
    }
}
