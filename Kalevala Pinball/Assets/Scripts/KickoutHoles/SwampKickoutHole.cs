using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala {
    public class SwampKickoutHole: KickoutHole {

        private bool _doOnce = true;

        protected override void KickOutUpdate()
        {
            if(_doOnce)
            {
                SFXPlayer.Instance.Play(1);
                Scorekeeper.Instance.AddScore(Scorekeeper.ScoreType.IlmarinenKOH);
                _doOnce = false;
            }
        }

        protected override void KickOut()
        {
            base.KickOut();
            _doOnce = true;
        }

    }
}
