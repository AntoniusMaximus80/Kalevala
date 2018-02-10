using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class BallReturn : MonoBehaviour
    {
        public Pinball _pinball;
        private Vector3 _pinballStartPosition;

        // Use this for initialization
        void Start()
        {
            _pinballStartPosition = _pinball.transform.position;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            other.transform.position = _pinballStartPosition;

            Pinball pinball = other.GetComponent<Pinball>();
            if (pinball != null)
            {
                pinball.StopMotion();
            }
        }
    }
}
