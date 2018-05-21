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
                SFXPlayer.Instance.Play(Sound.SuoKOH);
                Scorekeeper.Instance.AddScore(Scorekeeper.ScoreType.SwampKOH);
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
