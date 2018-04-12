using UnityEngine;

namespace Kalevala
{
    public class FlipperBar : MonoBehaviour
    {
        public enum FlipperOrientation
        {
            Left,
            Right
        };

        public FlipperOrientation _flipperBarOrientation;

        private HingeJoint _hingeJoint;
        private JointMotor _jointMotor;
        private JointSpring _jointSpring;
        private Rigidbody _rb;

        private PhysicMaterial _flipperMaterial;

        private PinballManager _pinballManager;

        private bool _flipperMaterialBouncinessChanged = false;

        public AudioSource _flipperBarUpAudioSource,
            _flipperBarDownAudioSource;

        void Start()
        {
            MakeMaterial();
            _pinballManager = FindObjectOfType<PinballManager>();
            _rb = GetComponent<Rigidbody>();
            _hingeJoint = GetComponent<HingeJoint>();

            // Trying to improve physics by overriding the defaults.
            _rb.maxAngularVelocity = 0;
            _rb.maxDepenetrationVelocity = _rb.maxDepenetrationVelocity * 5;
            _rb.solverIterations = 12;
            _rb.solverVelocityIterations = 5;

            _jointSpring = _hingeJoint.spring;
            _jointMotor = _hingeJoint.motor;

            _hingeJoint.useMotor = false;
            _hingeJoint.useSpring = true;

            _jointMotor.force = _pinballManager._flipperMotorForce;
            _jointMotor.targetVelocity = _pinballManager._flipperMotorTargetVelocity;

            _jointSpring.spring = _pinballManager._springForce;
        }

        private void FixedUpdate()
        {
            if(_rb.angularVelocity.y > 2 || _rb.angularVelocity.y < -2 && !_flipperMaterialBouncinessChanged)
            {
                _flipperMaterial.bounciness = 0.6f;
                _flipperMaterialBouncinessChanged = true;
            } else if (_rb.angularVelocity.y < 2 && _rb.angularVelocity.y > -2f && _flipperMaterialBouncinessChanged)
            {
                _flipperMaterialBouncinessChanged = false;
                _flipperMaterial.bounciness = 0.2f;
            }
        }

        public void UseMotor()
        {
            _hingeJoint.useMotor = true;
            _hingeJoint.useSpring = false;

            if (_flipperBarUpAudioSource == null)
            {
                return;
            }

            if (!_flipperBarUpAudioSource.isPlaying)
            {
                float randomPitch = Random.Range(0.95f, 1.05f);
                _flipperBarUpAudioSource.pitch = randomPitch;
                _flipperBarUpAudioSource.Play();
            } else
            {
                _flipperBarUpAudioSource.Stop();
                float randomPitch = Random.Range(0.95f, 1.05f);
                _flipperBarUpAudioSource.pitch = randomPitch;
                _flipperBarUpAudioSource.Play();
            }
        }

        public void UseSpring()
        {
            _hingeJoint.useMotor = false;
            _hingeJoint.useSpring = true;

            if (_flipperBarDownAudioSource == null)
            {
                return;
            }

            if (!_flipperBarDownAudioSource.isPlaying)
            {
                float randomPitch = Random.Range(0.9f, 1.1f);
                _flipperBarDownAudioSource.pitch = randomPitch;
                _flipperBarDownAudioSource.Play();
            }
            else
            {
                _flipperBarDownAudioSource.Stop();
                float randomPitch = Random.Range(0.9f, 1.1f);
                _flipperBarDownAudioSource.pitch = randomPitch;
                _flipperBarDownAudioSource.Play();
            }
        }

        public bool IsReset
        {
            get
            {
                return Mathf.Abs(_hingeJoint.angle) < 5f;
            }
        }

        private void MakeMaterial()
        {
            _flipperMaterial = new PhysicMaterial("Flipper");
            _flipperMaterial.dynamicFriction = 0.1f;
            _flipperMaterial.staticFriction = 0.5f;
            _flipperMaterial.frictionCombine = PhysicMaterialCombine.Average;
            _flipperMaterial.bounceCombine = PhysicMaterialCombine.Maximum;
            _flipperMaterial.bounciness = 0.2f;
            GetComponent<Collider>().material = _flipperMaterial;

        }
    }
}