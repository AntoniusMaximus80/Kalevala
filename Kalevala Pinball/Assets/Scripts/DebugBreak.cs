using UnityEngine;

namespace Kalevala
{
    public class DebugBreak : DebugBallHitTool
    {
        protected override void Activate()
        {
            Debug.Log("[Debug] Paused");
            Debug.Break();
        }
    }
}
