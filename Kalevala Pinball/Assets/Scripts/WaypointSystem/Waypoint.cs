using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala.WaypointSystem
{
    public class Waypoint : MonoBehaviour
    {
        public bool IsPartOfCurve { get; set; }

        public string CurveName { get; set; }

        public Vector3 Position
        {
            get
            {
                return transform.position;
            }
        }
    }
}
