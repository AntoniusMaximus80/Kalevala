using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Kalevala.WaypointSystem;

namespace Kalevala.Editor
{
    [UnityEditor.CustomEditor(typeof(PinballManager))]
    public class PinballManagerInspector : UnityEditor.Editor
    {
        private PinballManager targetPM;
        private Transform handleTransform;
        private Quaternion handleQuaternion;

        protected void OnEnable()
        {
            targetPM = target as PinballManager;
            handleTransform = targetPM.transform;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            MoveLaunchPointToBallButton();
        }

        private void OnSceneGUI()
        {
            DisplayLaunchPointHandle();
        }

        private void MoveLaunchPointToBallButton()
        {
            if (GUILayout.Button("Move Launch Point To Ball"))
            {
                Pinball ball = FindObjectOfType<Pinball>();

                if (ball != null)
                {
                    targetPM.LaunchPoint = ball.transform.position;
                }
            }
        }

        private void DisplayLaunchPointHandle()
        {
            // Sets the handle's rotation based on
            // if Local or Global mode is active
            handleQuaternion = Tools.pivotRotation == PivotRotation.Local ?
                handleTransform.rotation : Quaternion.identity;

            // Transforms the position into world coordinates
            Vector3 point =
                handleTransform.TransformPoint(targetPM.LaunchPoint);

            EditorGUI.BeginChangeCheck();

            point = Handles.DoPositionHandle(point, handleQuaternion);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetPM, "Move Ball Launch Point");
                //EditorUtility.SetDirty(targetPM);
                targetPM.LaunchPoint =
                    handleTransform.InverseTransformPoint(point);
            }
        }
    }
}
