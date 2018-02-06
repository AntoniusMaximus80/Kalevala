using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class BlinkingLight : MonoBehaviour
    {

        public float _blinkingInterval,
            _intensityDelay;

        public bool _lightOn,
            _intensityTransition = false;

        private float _blinkingIntervalCountdown,
            _intensityDelayCountup = 0f,
            _originalIntensity;

        // Use this for initialization
        void Start()
        {
            _originalIntensity = GetComponent<Light>().intensity;
            _blinkingIntervalCountdown = _blinkingInterval;
        }

        // Update is called once per frame
        void Update()
        {
            if (!_intensityTransition)
            {
                _blinkingIntervalCountdown -= Time.deltaTime;
                if (_blinkingIntervalCountdown <= 0f)
                {
                    _blinkingIntervalCountdown = _blinkingInterval;
                    _intensityTransition = true;
                }
            }
            else
            {
                _intensityDelayCountup += Time.deltaTime;
                float ratio = Mathf.Clamp(_intensityDelayCountup / _intensityDelay, 0f, 1f);

                if (_lightOn)
                {
                    //Debug.Log(Mathf.Lerp(_originalIntensity, 0f, ratio));
                    GetComponent<Light>().intensity = Mathf.Lerp(_originalIntensity, 0f, ratio);
                }
                else
                {
                    GetComponent<Light>().intensity = Mathf.Lerp(0f, _originalIntensity, ratio);
                }

                if (ratio == 1f)
                {
                    _intensityTransition = false;
                    _intensityDelayCountup = 0f;
                    if (_lightOn)
                    {
                        _lightOn = false;
                    }
                    else
                    {
                        _lightOn = true;
                    }
                }
            }
        }
    }
}