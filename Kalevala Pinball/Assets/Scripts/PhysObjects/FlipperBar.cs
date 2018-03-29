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

        private PinballManager _pinballManager;

        public AudioSource _flipperBarUpAudioSource,
            _flipperBarDownAudioSource;

        void Start()
        {
            GetComponent<Rigidbody>().maxAngularVelocity = 0; // Set the maximum angular velocity to infinite.

            _pinballManager = FindObjectOfType<PinballManager>();

            _hingeJoint = GetComponent<HingeJoint>();
            _jointSpring = _hingeJoint.spring;
            _jointMotor = _hingeJoint.motor;

            _hingeJoint.useMotor = false;
            _hingeJoint.useSpring = true;

            _jointMotor.force = _pinballManager._flipperMotorForce;
            _jointMotor.targetVelocity = _pinballManager._flipperMotorTargetVelocity;

            _jointSpring.spring = _pinballManager._springForce;
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
                return Mathf.Abs(_hingeJoint.angle) < 10f;
            }
        }
    }
}