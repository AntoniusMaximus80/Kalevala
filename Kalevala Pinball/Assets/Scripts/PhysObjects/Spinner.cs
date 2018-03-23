using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class Spinner: MonoBehaviour
    {
        /// <summary>
        /// Multiplier to increase magnitude of rotation.
        /// </summary>
        [SerializeField]
        private float _spinSpeedMultiplier;

        /// <summary>
        /// Defines how wobbly the object is when it stabilizes.
        /// </summary>
        [SerializeField]
        private float _wobblyingAmount;

        /// <summary>
        /// How fast the object stabilizes back to original rotation.
        /// </summary>
        [SerializeField]
        private float _stabilizeRotationSpeed;

        [SerializeField]
        private float _stabilizingThreshold;

        [SerializeField]
        private int _maxAngularVelocity;

        /// <summary>
        /// Defines if the spinner has stopped spinning from the force gained by ball hitting it
        /// </summary>
        private bool _stabilize = false;

        /// <summary>
        /// Defines which side the plate should stabilize after rotation force from ball hitting it is near zero
        /// </summary>
        private bool _checkRotation = true;

        /// <summary>
        /// Used to give score every half round the plate rotates.
        /// </summary>
        private bool _fullRound;

        private Rigidbody _rb;

        /// <summary>
        /// Defines the side which is closest to the spinner when the force gained from the ball is close to zero
        /// </summary>
        private Vector3 _stabilizationDirection;

        private float _timer = 0f;

        private void Start()
        {
            if(transform.rotation.eulerAngles.x >= 0 && transform.rotation.eulerAngles.x <= 90)
            {
                _fullRound = true;
            }
            else if(transform.rotation.eulerAngles.x >= 270 && transform.rotation.eulerAngles.x <= 360)
            {
                _fullRound = false;
            }
            _rb = GetComponent<Rigidbody>();
            _rb.maxAngularVelocity = _maxAngularVelocity;
        }

        // Spins the object based on speed give by passing objects velocity.
        void FixedUpdate()
        {
            if(_timer > 0)
            {
                _timer -= Time.deltaTime;
                CheckScoring();
            }
            else if(_rb.angularVelocity.x < _stabilizingThreshold && _rb.angularVelocity.x > -_stabilizingThreshold &&
                _checkRotation && (transform.rotation.eulerAngles.x != 270 || transform.rotation.eulerAngles.x != 90))
            {
                _stabilize = true;
                _checkRotation = false;
                CheckStabilizationDirection();
            }
            else
            {
                CheckScoring();
            }

            if(_stabilize)
            {
                StabilizeRotation();
            }

        }

        /// <summary>
        /// Stabilizes rotation back to original if the spinning has stopped and the object isn't back to original rotation.
        /// </summary>
        private void StabilizeRotation()
        {
            Vector3 predictedUp = Quaternion.AngleAxis(
             _rb.angularVelocity.magnitude * Mathf.Rad2Deg * _wobblyingAmount / _stabilizeRotationSpeed,
             _rb.angularVelocity
         ) * transform.up;
            Vector3 torqueVector = Vector3.Cross(predictedUp, _stabilizationDirection);
            torqueVector = Vector3.Project(torqueVector, new Vector3(1f, 0, 0));
            _rb.AddTorque(torqueVector * _stabilizeRotationSpeed * _stabilizeRotationSpeed);

            if(transform.rotation.eulerAngles.x == 90 && _rb.angularVelocity.x < 0.01f && _rb.angularVelocity.x > -0.01f)
            {
                _rb.angularVelocity = Vector3.zero;
                _stabilize = false;
            }
            else if(transform.rotation.eulerAngles.x == 270 && _rb.angularVelocity.x < 0.01f && _rb.angularVelocity.x > -0.01f)
            {
                _rb.angularVelocity = Vector3.zero;
                _stabilize = false;
            }
        }

        /// <summary>
        /// Checks side the plate should stabilize
        /// </summary>
        private void CheckStabilizationDirection()
        {
            if(transform.rotation.eulerAngles.x >= 0 && transform.rotation.eulerAngles.x <= 90)
            {
                _stabilizationDirection = Vector3.forward;
            }
            else if(transform.rotation.eulerAngles.x >= 270 && transform.rotation.eulerAngles.x <= 360)
            {
                _stabilizationDirection = -Vector3.forward;
            }
        }

        /// <summary>
        /// Gives score each half round the spinner does.
        /// </summary>
        private void CheckScoring()
        {
            if(transform.rotation.eulerAngles.x >= 0 && transform.rotation.eulerAngles.x <= 90 && !_fullRound)
            {
                Debug.Log("Score added");
                _fullRound = true;
                Scorekeeper.Instance.AddScore(Scorekeeper.ScoreType.Spinner);
            }
            else if(transform.rotation.eulerAngles.x >= 270 && transform.rotation.eulerAngles.x <= 360 && _fullRound)
            {
                Debug.Log("Score added");
                _fullRound = false;
                Scorekeeper.Instance.AddScore(Scorekeeper.ScoreType.Spinner);
            }
        }

        /// <summary>
        /// Takes passing objects velocity and starts rotating the object based on that.
        /// </summary>
        /// <param name="other">Passing object, should always be pinball</param>
        private void OnTriggerEnter( Collider other )
        {

            Pinball pinball = other.GetComponent<Pinball>();
            int sign = 1;
            if(pinball != null)
            {
                _rb.angularVelocity = Vector3.zero;
                if(pinball.PhysicsVelocity.z < 0)
                {
                    sign = -1;
                }
                _rb.AddTorque(transform.right * (pinball.Speed * sign) * _spinSpeedMultiplier, ForceMode.Impulse);
                _checkRotation = true;
                _timer = 0.1f;
                _stabilize = false;
            }
        }
    }
}
