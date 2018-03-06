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
        [SerializeField]
        private HingeJoint _hingejoint;
        
        private JointLimits _jointLimits;
        private bool _takeInput = true;
        private float _launcherForce;

        private bool _checkVelocity = false;
        private bool _launchDone = true;
        private bool _returnAxeToStartPosition = false;


        private void Awake()
        {
            JointMotor motor = _hingejoint.motor;
            motor.force = (_hingejoint.limits.max - _hingejoint.limits.min) / _timeToMaxForce;
            motor.targetVelocity = (_hingejoint.limits.max - _hingejoint.limits.min) / _timeToMaxForce;
            _hingejoint.motor = motor;
            _jointLimits = _hingejoint.limits;
            _hingejoint.useMotor = false;
            _hingejoint.useSpring = true;
        }

        private void Update()
        {
            if(_hingejoint.angle <=  40f && !_launchDone)
            {
                Launch();
            }
            if(_hingejoint.velocity == 0f && _checkVelocity)
            {
                _returnAxeToStartPosition = true;
            }
            if(_returnAxeToStartPosition && _hingejoint.limits.min < 90)
            {
                _jointLimits.min += 20f * Time.deltaTime;
                if(_jointLimits.min > 90f )
                {
                    _jointLimits.min = 90f;
                }
                _hingejoint.limits = _jointLimits;
            } else if(_returnAxeToStartPosition && _hingejoint.limits.min >= 90)
            {
                _returnAxeToStartPosition = false;
                _takeInput = true;
            }
        }

        /// <summary>
        /// Increases the power to launch ball, calmps the force between 0 and 1.
        /// </summary>
        public void PoweringUp()
        {
            if(_takeInput)
            {
                _launcherForce = Mathf.Clamp01(_launcherForce + Time.deltaTime / _timeToMaxForce);
                _hingejoint.useMotor = true;
                _hingejoint.useSpring = false;
            }

        }

        /// <summary>
        /// Launches the ball with force depending on how long launch button is pressed.
        /// </summary>
        public void SwingAxe()
        {
            if(_takeInput)
            {
                _jointLimits.min = 0f;
                _hingejoint.limits = _jointLimits;
                _hingejoint.useMotor = false;
                _hingejoint.useSpring = true;
                _launchDone = false;
            }
        }

        private void Launch()
        {
            _checkVelocity = true;
            _launchDone = true;
            _takeInput = false;
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
