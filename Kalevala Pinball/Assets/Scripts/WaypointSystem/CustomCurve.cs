using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kalevala.WaypointSystem;

namespace Kalevala
{
    public class CustomCurve : MonoBehaviour
    {
        [SerializeField]
        private Vector3[] _points;

        [SerializeField, Range(5, 50)]
        private int _lineSteps = 10;

        public Vector3 GetPoint(float t)
        {
            if (Points.Length == 4)
            {
                return transform.TransformPoint(
                Utils.GetCurvePoint(Points[0], Points[1], Points[2], Points[3], t));
            }
            else if (Points.Length == 5)
            {
                return transform.TransformPoint(
                    Utils.GetCurvePoint(Points[0], Points[1], Points[2], Points[3], Points[4], t));
            }

            return transform.TransformPoint(
                Utils.GetCurvePoint(Points[0], Points[1], Points[2], t));
        }

        public Vector3[] Points
        {
            get
            {
                if (_points == null)
                {
                    _points = new Vector3[4];
                }

                return _points;
            }
            set
            {
                _points = value;
            }
        }

        public int LineSteps
        {
            get
            {
                return _lineSteps;
            }
        }

        public void ResetPoints()
        {
            int length = (Points != null ? Points.Length : 4);
            if (length < 3)
            {
                length = 4;
            }

            Points = new Vector3[length];

            for (int i = 0; i < Points.Length; i++)
            {
                Points[i] = new Vector3(i + 1, 0, 0);
            }
        }
    }
}
