using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala.WaypointSystem
{
    public class PathMover : MonoBehaviour
    {
        //[SerializeField]
        //private Line line;

        //[SerializeField]
        //private Curve curve;

        //[SerializeField]
        //private Path path;

        [SerializeField]
        private EasingType easingType;

        [SerializeField]
        private float eventTime = 1;

        [SerializeField, Range(0, 1)]
        private float directionalRatio;

        private Vector3 prevWaypointPos;
        private bool reverse;
        private float elapsedTime;

        private void Start()
        {
            if (eventTime <= 0)
            {
                eventTime = 1f;
            }
        }

        public bool Move(Vector3 waypointPos)
        {
            // Don't use eventTime, use speed

            float ratio = elapsedTime / eventTime;
            directionalRatio = (reverse ? 1 - ratio : ratio);

            transform.position = Vector3.Lerp(prevWaypointPos, waypointPos, directionalRatio);

            elapsedTime += Time.deltaTime;
            if (elapsedTime >= eventTime)
            {
                prevWaypointPos = waypointPos;
                return true;

                //Debug.Log("FollowLine reset");
                //elapsedTime = 0;
                //reverse = !reverse;
            }

            return false;
        }

        //public void Move()
        //{
        //    // Don't use eventTime, use speed

        //    float ratio = elapsedTime / eventTime;
        //    dirRatio = (reverse ? 1 - ratio : ratio);

        //    if (path != false)
        //    {
        //        transform.position = Vector3.Lerp(line.start, line.end, dirRatio);
        //        //transform.position = Vector3.Lerp(line.start, line.end, Easing.EaseInOut(dirRatio, easingType, easingType));
        //    }
        //    if (line != false)
        //    {
        //        transform.position = Vector3.Lerp(line.start, line.end, dirRatio);
        //        //transform.position = Vector3.Lerp(line.start, line.end, Easing.EaseInOut(dirRatio, easingType, easingType));
        //    }
        //    else if (curve != false)
        //    {
        //        transform.position = curve.GetPoint(dirRatio);
        //        //transform.position = curve.GetPoint(Easing.EaseInOut(dirRatio, easingType, easingType));
        //        //transform.position = curve.GetPoint(Easing.Ease(dirRatio, (ratio < 0.5 ? 1 : -1), easingType));
        //    }

        //    elapsedTime += Time.deltaTime;
        //    if (elapsedTime >= eventTime)
        //    {
        //        Debug.Log("FollowLine reset");
        //        elapsedTime = 0;
        //        reverse = !reverse;
        //    }
        //}
    }
}
