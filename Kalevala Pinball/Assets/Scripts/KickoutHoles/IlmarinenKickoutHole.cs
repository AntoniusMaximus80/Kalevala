using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class IlmarinenKickoutHole: KickoutHole
    {
        [SerializeField]
        private Animator _bellows;
        private bool _doOnce = true;
        protected override void KickOutUpdate()
        {
            if(_doOnce)
            {
                _bellows.SetBool("Pumping", true);
                _ball.SetHeatBall(_timeToWait);
                Scorekeeper.Instance.AddScore(Scorekeeper.ScoreType.IlmarinenKOH);
                _doOnce = false;
            }
        }

        protected override void KickOut()
        {
            base.KickOut();
            _bellows.SetBool("Pumping", false);
            _doOnce = true;
        }
    }
}
