using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    [RequireComponent(typeof(Collider))]
    public abstract class DebugBallHitTool : MonoBehaviour
    {
        public float restingTime;
        public bool resting = false;
        private float elapsedRestingTime = 0;

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        protected virtual void Update()
        {
            if (resting)
            {
                elapsedRestingTime += Time.deltaTime;

                if (elapsedRestingTime > restingTime)
                {
                    elapsedRestingTime = 0;
                    resting = false;
                }
            }
        }

        protected abstract void Activate();

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!resting)
            {
                Pinball pinball = other.GetComponent<Pinball>();
                if (pinball != null)
                {
                    resting = true;
                    Activate();
                }
            }
        }
    }
}
