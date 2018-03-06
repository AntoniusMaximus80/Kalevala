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
        private Transform _axe;
        [SerializeField]
        private float _maxRotation;
        [SerializeField]
        private bool _useDebugs;

        private float _axeStartRotationX;
        private float _axeRotateSpeed;
        private float _launcherForce;
        private bool _launch = false;


        private void Awake()
        {
            _axeStartRotationX = _axe.eulerAngles.x;
            _axeRotateSpeed = _maxRotation / _timeToMaxForce;
        }

        private void Update()
        {
            if(_launch)
            {
                SwingAxe();
            }
        }

        private void SwingAxe()
        {
            if(_axe.eulerAngles.x > _axeStartRotationX)
            {
                _axe.Rotate(new Vector3(400f * Time.deltaTime, 0, 0));
            }
            else
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
                _launch = false;
            }
        }

        /// <summary>
        /// Increases the power to launch ball, calmps the force between 0 and 1.
        /// </summary>
        public void PoweringUp()
        {
            _launcherForce = Mathf.Clamp01(_launcherForce + Time.deltaTime / _timeToMaxForce);
            if(_launcherForce < 1)
            {
                _axe.Rotate(new Vector3(_axeRotateSpeed * Time.deltaTime, 0f, 0f));
            
            }
        }

        /// <summary>
        /// Launches the ball with force depending on how long launch button is pressed.
        /// </summary>
        public void Launch()
        {
            _launch = true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 1f);
        }
    }
}
