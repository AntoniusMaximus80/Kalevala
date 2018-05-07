using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala {

    public class StrobeLight : BaseLight
    {
        public Renderer _cover;
        public GameObject _lightBulb;
        public Light _pointLight;

        // onoff color values for the emission.
        public Color _offColor = Color.black;
        public Color _baseColor = Color.magenta;

        private bool _state = true, doOnce;

        
        private float _switchTime = -10f;

        

        public void Switch(bool onoff)
        {

            if (!doOnce)
            {
                _cover.material.EnableKeyword("_Emission");
                _baseColor = _cover.material.GetColor("_EmissionColor");
                doOnce = true;
            }

            _state = onoff;
         
            _cover.material.SetColor("_EmissionColor", onoff ? _baseColor : _offColor);
            _lightBulb.SetActive(onoff);
            _pointLight.enabled = onoff;

            //_switchTime = Time.time;
        }

        public void Update()
        {

            bool value;

            if (_state && Viewscreen.TimeSinceIncrement < .2f)
            {
                int time = Mathf.RoundToInt((Viewscreen.TimeSinceIncrement) * 100);

                int cycle = time / 10;
                int phase = time % 5;

                value = (phase < cycle) ? _state : !_state;
            }
            else
            {
                value = _state;
            }

            //if(Sweep()) _switchTime = Time.time;
            //Up.enabled = value;
            //Down.enabled = value;

            _cover.material.SetColor("_EmissionColor", value ? _baseColor : _offColor);
            _lightBulb.SetActive(value);
            _pointLight.enabled = value;

        }

        
    }
}
