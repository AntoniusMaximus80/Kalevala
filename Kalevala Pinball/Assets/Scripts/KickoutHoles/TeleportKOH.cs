using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class TeleportKOH: KickoutHole
    {
        [SerializeField]
        private Vector3 _teleportEndPosition;
        [SerializeField]
        private Transform _endRampEntrance;
        private bool _doOnce = true;

        protected override void KickOutUpdate()
        {
            if(_doOnce)
            {
                _ball.transform.position = _endRampEntrance.position;
                Scorekeeper.Instance.AddScore(Scorekeeper.ScoreType.TeleportKOH);
                _doOnce = false;
            }
        }

        protected override void KickOut()
        {
            base.KickOut();
            _doOnce = true;
        }

        protected override void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Vector3 startpos = _teleportEndPosition;
            Vector3 endpos = Vector3.zero;
            float gravity = 0;
            for(int i = 0; i < gizmoLenght; i++)
            {
                endpos = startpos + KickDirection + Vector3.forward * gravity;
                Gizmos.DrawLine(startpos, endpos);
                startpos = endpos;
                gravity -= 0.017f;
            }
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_teleportEndPosition, 1f);
        }
    }
}
