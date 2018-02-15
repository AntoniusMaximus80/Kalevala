﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public static class MathHelp
    {
        public static Vector3 GetCurvePoint(Vector3 p0, Vector3 p1, Vector3 p2,
                                            float t)
        {
            // Bézier curve:
            // B(t) = (1 - t) * [(1 - t) * p0 + t * p1] + t * [(1 - t) * p1 + t * p2]
            // 0 <= t <= 1

            return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);
        }
    }
}