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
        [SerializeField]
        private bool _useDebugs;
        private float _launcherForce;


        /// <summary>
        /// Increases the power to launch ball, calmps the force between 0 and 1.
        /// </summary>
        public void PoweringUp()
        {
            _launcherForce = Mathf.Clamp01(_launcherForce + Time.deltaTime / _timeToMaxForce);
        }

        /// <summary>
        /// Launches the ball with force depending on how long launch button is pressed.
        /// </summary>
        public void Launch()
        {
            int layermask = 1 << LayerMask.NameToLayer("Pinballs");
            Collider[] colliders = Physics.OverlapSphere(transform.position, 1f, layermask);
            Debug.Log(colliders.Length);
            foreach(Collider coll in colliders)
            {
                coll.GetComponent<Rigidbody>().AddForce(Vector3.forward * _launcherForce * _launcherForceMultiplier, ForceMode.Impulse);
                if(_useDebugs)
                {
                    Debug.Log(_launcherForce);
                }
            }
            _launcherForce = 0;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 1f);
        }
    }
}
