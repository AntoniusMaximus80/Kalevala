using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala {

    public class StrobeLight : MonoBehaviour
    {
        public Renderer _cover;
        public GameObject _lightBulb;
        public Light _pointLight;

        // onoff color values for the emission.
        public Color _offColor = Color.black;
        public Color _baseColor = Color.magenta;

        private bool _state;

        private float _switchTime = -10f;


        public void Switch(bool onoff)
        {
            _state = onoff;

            //Up.enabled = onoff;
            //Down.enabled = onoff;

            _cover.material.SetColor("_EmissionColor", onoff ? _baseColor : _offColor);
            _lightBulb.SetActive(onoff);
            _pointLight.enabled = onoff;

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

            //Up.enabled = value;
            //Down.enabled = value;

            _cover.material.SetColor("_EmissionColor", value ? _baseColor : _offColor);
            _lightBulb.SetActive(value);
            _pointLight.enabled = value;s

        }

    }
}
