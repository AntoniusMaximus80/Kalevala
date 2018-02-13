using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class DebugBreak : MonoBehaviour
    {
        public float restingTime;
        public bool resting = false;
        private float elapsedRestingTime = 0;

        // Update is called once per frame
        void Update()
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

        private void OnTriggerEnter(Collider other)
        {
            if (!resting)
            {
                Pinball pinball = other.GetComponent<Pinball>();
                if (pinball != null)
                {
                    resting = true;
                    Debug.Break();
                }
            }
        }
    }
}
