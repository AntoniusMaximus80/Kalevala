using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rollover : MonoBehaviour {

    public Transform wire;

    //public TuonelaLightSwitch _light;

    public float downPositionZ = -.5f;

    public float duration = .5f;

    private bool _down;

    private Vector3 _positionTemp, _targetPosition = Vector3.zero;

    private float _elapsedTime;

	// Use this for initialization
	void Start () {

        // Start having moved, might be a good idea to actually move the wires at start instead.
        _elapsedTime = duration;
	}
	
	// Update is called once per frame
	void Update () {

        if (_elapsedTime > duration) return;

        _elapsedTime += Time.deltaTime;

        // Relative height (z) the wire should have in its current state.
        _targetPosition.z = _down ? downPositionZ : 0f;
        
        wire.localPosition = Vector3.Lerp(wire.localPosition, _targetPosition, _elapsedTime / duration);
        
    }

    private void OnTriggerEnter(Collider collider)
    {

        Debug.Log("rollover");
        

        _down = true;
        _elapsedTime = duration - _elapsedTime;


    }

    
}
