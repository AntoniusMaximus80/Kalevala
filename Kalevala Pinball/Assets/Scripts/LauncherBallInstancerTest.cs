using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kalevala
{
    public class LauncherBallInstancerTest : Launcher
    {
        [SerializeField]
        private float _launcherForceMultiplier2;

        [SerializeField]
        private GameObject ball;

        private PinballManager _manager;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _manager = FindObjectOfType<PinballManager>();
        }

        public bool LaunchTest()
        {
            bool success = ball != null && _manager.InstanceNextBall(ball);

            if (success)
            {
                Collider coll = ball.GetComponent<SphereCollider>();

                coll.GetComponent<Rigidbody>().AddForce(-Vector3.forward * _launcherForceMultiplier2, ForceMode.Impulse);
            }

            return success;
        }
    }
}
