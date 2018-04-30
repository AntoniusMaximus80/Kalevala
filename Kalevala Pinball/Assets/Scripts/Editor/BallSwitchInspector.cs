using UnityEngine;
using UnityEditor;
using Kalevala.WaypointSystem;

namespace Kalevala.Editor
{
    [UnityEditor.CustomEditor(typeof(BallSwitch))]
    public class BallSwitchInspector : UnityEditor.Editor
    {
        private BallSwitch targetBallSwitch;
        private Transform targetTransform;
        private Quaternion handleQuaternion;

        protected void OnEnable()
        {
            targetBallSwitch = target as BallSwitch;
            targetTransform = targetBallSwitch.transform;
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
                TransformPoint(targetBallSwitch.SwitchDirection);

            if (showHandle)
            {
                EditorGUI.BeginChangeCheck();
                point = Handles.DoPositionHandle(point, handleQuaternion);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(targetBallSwitch, "Move Point");
                    //EditorUtility.SetDirty(targetRampEntrance);
                    targetBallSwitch.SwitchDirection =
                        targetTransform.InverseTransformPoint(point);
                }
            }

            return point;
        }
    }
}
