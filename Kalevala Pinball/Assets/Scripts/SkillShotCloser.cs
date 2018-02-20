using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala {
    public class SkillShotCloser: MonoBehaviour {

        private SkillShotPoint _parent;

        private void Start()
        {
            _parent = GetComponentInParent<SkillShotPoint>();
        }


        /// <summary>
        /// Initiates closing the skillshot door from parent.
        /// </summary>
        /// <param name="other">Collider of the pinball</param>
        private void OnTriggerEnter( Collider other )
        {
            _parent.CloseSkillShot();
        }
    }
}
