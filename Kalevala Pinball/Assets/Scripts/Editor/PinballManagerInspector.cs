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
            DisplayPointHandle(targetPM.LaunchPoint, true);
            DisplayPointHandle(targetPM.LaunchArrivalPoint, false);
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

        /// <summary>
        /// Displays a handle at the given point.
        /// The point can be moved with the handle.
        /// </summary>
        /// <param name="changeLaunchPoint">True: launch point,
        /// False: launch arrival point</param>
        private void DisplayPointHandle(Vector3 point, bool changeLaunchPoint)
        {
            // Sets the handle's rotation based on
            // if Local or Global mode is active
            handleQuaternion = Tools.pivotRotation == PivotRotation.Local ?
                handleTransform.rotation : Quaternion.identity;

            // Transforms the position into world coordinates
            Vector3 worldPoint =
                handleTransform.TransformPoint(point);

            EditorGUI.BeginChangeCheck();

            worldPoint = Handles.DoPositionHandle(worldPoint, handleQuaternion);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(targetPM, "Move Point");
                //EditorUtility.SetDirty(targetPM);

                // An ugly way to change a variable point
                // but I couldn't think of anything else
                if (changeLaunchPoint)
                {
                    targetPM.LaunchPoint =
                        handleTransform.InverseTransformPoint(worldPoint);
                }
                else
                {
                    targetPM.LaunchArrivalPoint =
                        handleTransform.InverseTransformPoint(worldPoint);
                }
            }
        }
    }
}
