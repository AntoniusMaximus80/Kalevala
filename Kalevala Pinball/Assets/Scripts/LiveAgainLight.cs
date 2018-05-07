using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kalevala
{
    public class LiveAgainLight : BaseLight
    {


        public Light Up;

        public Color _offColor = Color.black;

        private bool _state, doOnce;

        private Color _baseColor;

        private float _switchTime = -10f;



        public void Switch(bool onoff)
        {
            
            if (!doOnce) {
                gameObject.GetComponent<Renderer>().material.EnableKeyword("_Emission");
                _baseColor = gameObject.GetComponent<Renderer>().material.GetColor("_EmissionColor");
                doOnce = true;
            }

            _state = onoff;
            Up.enabled = onoff;            
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
}
