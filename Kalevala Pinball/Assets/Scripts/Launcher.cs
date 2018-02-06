using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kalevala
{
    public class Launcher: MonoBehaviour
    {

        [SerializeField]
        private float _launcherForceMultiplier;
        [SerializeField]
        private float _timeToMaxForce;
        private float _launcherForce;

        public void PoweringUp()
        {
            _launcherForce += Mathf.Clamp01(Time.deltaTime / _timeToMaxForce);
        }

        public void Launch()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 1f);

            foreach(Collider coll in colliders)
            {
                coll.GetComponent<Rigidbody>().AddForce(-Vector3.forward * _launcherForce * _launcherForceMultiplier, ForceMode.Impulse);
            }
            _launcherForce = 0;
        }
    }
}
