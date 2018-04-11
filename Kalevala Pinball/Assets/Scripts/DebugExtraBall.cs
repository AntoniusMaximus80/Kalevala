using UnityEngine;

namespace Kalevala
{
    public class DebugExtraBall : DebugBallHitTool
    {
        [SerializeField]
        private Transform _extraBallPosition;

        protected override void Activate()
        {
            Debug.Log("[Debug] Extra ball awarded");
            PinballManager.Instance.ExtraBall(_extraBallPosition, Vector3.zero);
        }
    }
}
