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
    /// Speed based on passing objects velocity.
    /// </summary>
    private float _startSpeed;

    /// <summary>
    /// How fast the object stabilizes back to original rotation.
    /// </summary>
    [SerializeField]
    private float _stabilizeRotation;
    
	
	// Spins the object based on speed give by passing objects velocity.
	void FixedUpdate () {
        Debug.Log(_startSpeed);
        transform.rotation *= Quaternion.AngleAxis(_startSpeed * _spinSpeedMultiplier * Time.deltaTime, Vector3.right);

        DecrementRotation();

        StabilizeRotation();
        
    }

    /// <summary>
    /// Decreases rotation speed each fixed update.
    /// </summary>
    private void DecrementRotation()
    {
        if(_startSpeed > 0)
        {
            _startSpeed = Mathf.Clamp(_startSpeed - 0.5f, 0, _startSpeed);
        }
        else if(_startSpeed < 0)
        {
            _startSpeed = Mathf.Clamp(_startSpeed + 0.5f, _startSpeed, 0);
        }
    }

    /// <summary>
    /// Stabilizes rotation back to original if the spinning has stopped and the object isn't back to original rotation.
    /// </summary>
    private void StabilizeRotation()
    {
        if(transform.rotation.eulerAngles.x > 0 && transform.rotation.eulerAngles.x <= 90 && _startSpeed == 0)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(90f, 0f, 0f), Time.time * _stabilizeRotation);
        }
        else if(transform.rotation.eulerAngles.x > 270 && transform.rotation.eulerAngles.x <= 360 && _startSpeed == 0)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(270f, 0f, 0f), Time.time * _stabilizeRotation);
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
            _startSpeed = rb.velocity.z * -1;
        }
    }
}
