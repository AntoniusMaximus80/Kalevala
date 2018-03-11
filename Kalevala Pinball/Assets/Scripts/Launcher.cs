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
        private float _minForceTime;
        [SerializeField]
        private bool _useDebugs;
        [SerializeField]
        private HingeJoint _hingejoint;
        [SerializeField]
        private GameObject _hitParticles;
        [SerializeField]
        private AudioSource _hitSFX;

        public static Launcher Instance;
        
        private JointLimits _jointLimits;
        private bool _takeInput = true;
        private float _launcherForce;
        private Pinball _pinball;

        private bool _checkVelocity = false;
        private bool _launchDone = true;
        private bool _returnAxeToStartPosition = false;


        private void Awake()
        {
            _launcherForce = _minForceTime;
            JointMotor motor = _hingejoint.motor;
            motor.force = (_hingejoint.limits.max - _hingejoint.limits.min) /(_timeToMaxForce - _minForceTime);
            motor.targetVelocity = (_hingejoint.limits.max - _hingejoint.limits.min) / (_timeToMaxForce - _minForceTime);
            _hingejoint.motor = motor;
            _jointLimits = _hingejoint.limits;
            _hingejoint.useMotor = false;
            _hingejoint.useSpring = true;
            Instance = this;
        }

        private void Update()
        {
            if(_hingejoint.angle <=  40f && !_launchDone)
            {
                Launch();
            }

            if(_returnAxeToStartPosition && _hingejoint.limits.min < 90)
            {
                _jointLimits.min += 80f * Time.deltaTime;
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
                _launcherForce = Mathf.Clamp(_launcherForce + Time.deltaTime / _timeToMaxForce, _minForceTime, 1);
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
                _hingejoint.useLimits = false;
                _hingejoint.useMotor = false;
                _hingejoint.useSpring = true;
                _launchDone = false;
            }
        }

        private void Launch()
        {
            _hitSFX.Play();
            _hitParticles.SetActive(true);
            PinballManager.Instance.SetPinballPhysicsEnabled(true);
            _pinball.AddImpulseForce(Vector3.forward * _launcherForce * _launcherForceMultiplier);
            _checkVelocity = true;
            _launchDone = true;
            _takeInput = false;
            _launcherForce = _minForceTime;

            StateManager.LaunchOver();
        }

        public void StartLaunch(Pinball pinball)
        {
            _hitParticles.SetActive(false);
            _pinball = pinball;
            _returnAxeToStartPosition = true;
            _hingejoint.useLimits = true;

            StateManager.ShowLaunch();
        }
    }
}