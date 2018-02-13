using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour {

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

    /// <summary>
    /// Defines if the spinner has stopped spinning from the force gained by ball hitting it
    /// </summary>
    private bool _stabilize = false;

    private bool _checkRotation = true;

    private Rigidbody _rb;

    /// <summary>
    /// Defines the side which is closest to the spinner when the force gained from the ball is close to zero
    /// </summary>
    private Vector3 _stabilizationDirection;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    // Spins the object based on speed give by passing objects velocity.
    void FixedUpdate () {

        if(_rb.angularVelocity.x <= _stabilizingThreshold && _rb.angularVelocity.x >= -_stabilizingThreshold &&
            _checkRotation && (transform.rotation.eulerAngles.x != 270 || transform.rotation.eulerAngles.x != 90))
        {
            _stabilize = true;
            _checkRotation = false;
            CheckStabilizationDirection();
        }

        //transform.rotation *= Quaternion.AngleAxis(_startSpeed * _spinSpeedMultiplier * Time.deltaTime, Vector3.right);

        //DecrementRotation();
        if(_stabilize)
        {
            StabilizeRotation();
        }

    }

    /// <summary>
    /// Decreases rotation speed each fixed update.
    /// </summary>
    //private void DecrementRotation()
    //{
    //    if(_startSpeed > 0)
    //    {
    //        _startSpeed = Mathf.Clamp(_startSpeed - 0.5f, 0, _startSpeed);
    //    }
    //    else if(_startSpeed < 0)
    //    {
    //        _startSpeed = Mathf.Clamp(_startSpeed + 0.5f, _startSpeed, 0);
    //    }
    //}

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
        //if(transform.rotation.eulerAngles.x > 0 && transform.rotation.eulerAngles.x <= 90 && _startSpeed == 0)
        //{
        //    //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(90f, 0f, 0f), Time.time * _stabilizeRotation);
        //    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(90f, 0f, 0f), Time.fixedDeltaTime * _stabilizeRotation);
        //    _stabilizeRotation = Mathf.Clamp(_stabilizeRotation + 0.1f, _stabilizeRotation, 10);
        //}
        //else if(transform.rotation.eulerAngles.x > 270 && transform.rotation.eulerAngles.x <= 360 && _startSpeed == 0)
        //{
        //    //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(270f, 0f, 0f), Time.time * _stabilizeRotation);
        //    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(270f, 0f, 0f), Time.fixedDeltaTime * _stabilizeRotation);
        //    _stabilizeRotation = Mathf.Clamp(_stabilizeRotation + 0.1f, _stabilizeRotation, 10);
        //} else
        //{
        //    _stabilizeRotation = 1f;
        //}
    }

    /// <summary>
    /// Checks side the plate should stabilize
    /// </summary>
    private void CheckStabilizationDirection()
    {
        if(transform.rotation.eulerAngles.x > 0 && transform.rotation.eulerAngles.x <= 90)
        {
            _stabilizationDirection = Vector3.forward;
        }
        else if(transform.rotation.eulerAngles.x > 270 && transform.rotation.eulerAngles.x <= 360)
        {
            _stabilizationDirection = -Vector3.forward;
        }
    }

    /// <summary>
    /// Takes passing objects velocity and starts rotating the object based on that.
    /// </summary>
    /// <param name="other">Passing object, should always be pinball</param>
    private void OnTriggerEnter( Collider other )
    {
        
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if(rb != null)
        {
            //_startSpeed = rb.velocity.z * -1;
            gameObject.GetComponent<Rigidbody>().AddTorque(transform.right * _spinSpeedMultiplier * rb.velocity.z * -1);
            _checkRotation = true;
        }
    }
}
