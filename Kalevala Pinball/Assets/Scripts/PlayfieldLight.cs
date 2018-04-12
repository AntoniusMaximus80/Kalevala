using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayfieldLight : MonoBehaviour
{


    public Light Up, Down;

    private bool _state;

    private float _switchTime = -10f;


    public void Switch(bool onoff)
    {
        _state = onoff;

        Up.enabled = onoff;
        Down.enabled = onoff;

        _switchTime = Time.time;
    }

    public void Update()
    {

        bool value;

        if ((Time.time - _switchTime) < .5f)
        {
            int time = Mathf.RoundToInt((Time.time - _switchTime) * 100);

            int cycle = time / 10;
            int phase = time % 5;

            value = (phase < cycle) ? _state : !_state;
        }
        else
        {
            value = _state;
        }

        Up.enabled = value;
        Down.enabled = value;


    }


}
