using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rollover : MonoBehaviour {

    public Transform wire;

    public PlayfieldLight _light;

    public float downPositionZ = -.5f;

    public float duration = .5f;

    private bool _down;

    private Vector3 _targetPosition = Vector3.zero;

    private float _elapsedTime;

    private static List<Rollover> _instances = new List<Rollover>();

    private static AudioSource _leftSound, _rightSound;

	// Use this for initialization
	void Awake () {

        Init();

        _instances.Add(this);

        if (_leftSound == null )
        {
            _leftSound = GameObject.Find("RolloverLeftAudio").GetComponent<AudioSource>();
            _rightSound = GameObject.Find("RolloverRightAudio").GetComponent<AudioSource>();
        }

	}

    internal static void Reset()
    {
        foreach(Rollover r in _instances)
        {
            r.Init();
        }
    }

    private void Init()
    {
        // Start having moved, might be a good idea to actually move the wires at start instead.
        _elapsedTime = duration;

        _down = false;

        // The light should start off.
        _light.TurnOff();
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

        // Was already down.
        if (_down) return;

        _down = true;
        _light.TurnOn();
        _elapsedTime = duration - _elapsedTime;

        (transform.position.x < 0 ? _leftSound : _rightSound).Play();
        
    }

    
}
