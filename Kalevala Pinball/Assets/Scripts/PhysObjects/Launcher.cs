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
        private float _LauncherAreLeftBorderOffset;

        public static Launcher Instance;
        
        private JointLimits _jointLimits;
        private bool _takeInput = true;
        private float _launcherForce;
        private Pinball _pinball;
        private bool _launchDone = true;
        private bool _returnAxeToStartPosition = false;
        private SkillShotGate[] _gates;
        private SkillShotHandler _handler;
        private bool _gatesClosed = false;
        private float _launcherAreaLeftBorder;
        public bool SkillShotSuccesful;


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
            _gates = FindObjectsOfType<SkillShotGate>();
            _handler = FindObjectOfType<SkillShotHandler>();
            _launcherAreaLeftBorder = (transform.position + Vector3.right * _LauncherAreLeftBorderOffset).x;
        }

        private void Update()
        {
            if(_hingejoint.angle <=  40f && !_launchDone)
            {
                Launch();
            }

            if(_returnAxeToStartPosition && _hingejoint.limits.min < 90)
            {
                _jointLimits.min += 160f * Time.deltaTime;
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
            if(_pinball != null && !_gatesClosed && !BallOnLauncher)
            {
                if( _pinball.transform.position.x < _launcherAreaLeftBorder )
                {
                    foreach(SkillShotGate gate in _gates)
                    {
                        gate.CloseGate();
                    }
                    _gatesClosed = true;
                }
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
                _takeInput = false;
            }
        }

        private void Launch()
        {
            BallOnLauncher = false;
            SFXPlayer.Instance.Play(Sound.UkonKirves);
            _hitParticles.SetActive(true);
            PinballManager.Instance.SetPinballPhysicsEnabled(true);
            _pinball.AddImpulseForce(Vector3.forward * _launcherForce * _launcherForceMultiplier);
            _launchDone = true;
            _launcherForce = _minForceTime;

            // 10 seconds of Shoot Again
            PinballManager.Instance.ActivateShootAgain(10);
        }

        public void StartLaunch(Pinball pinball)
        {
            BallOnLauncher = true;
            _hitParticles.SetActive(false);
            _pinball = pinball;
            _returnAxeToStartPosition = true;
            _hingejoint.useLimits = true;
            _gatesClosed = false;
            if(_handler != null)
            {
                _handler.PathDeactivate();
            }

            foreach(SkillShotGate gate in _gates)
            {
                gate.OpenGate();
            }
        }

        public bool BallOnLauncher { get; private set; }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Vector3 launcherLeftBorder = transform.position + Vector3.right * _LauncherAreLeftBorderOffset;
            Gizmos.DrawLine(launcherLeftBorder, Vector3.forward * 80 + launcherLeftBorder);
        }
    }
}