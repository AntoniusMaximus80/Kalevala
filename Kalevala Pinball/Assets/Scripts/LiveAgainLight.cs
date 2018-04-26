using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveAgainLight : MonoBehaviour
{


    public Light Up;

    public Color _offColor = Color.black;

    private bool _state;

    public Color _baseColor = Color.magenta;

    private float _switchTime = -10f;



    public void Switch(bool onoff)
    {
        _state = onoff;

        Up.enabled = onoff;
        gameObject.GetComponent<Renderer>().material.EnableKeyword("_Emission");
        gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", onoff ? _baseColor : _offColor);

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
        gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", value ? _baseColor : _offColor);


    }


}
