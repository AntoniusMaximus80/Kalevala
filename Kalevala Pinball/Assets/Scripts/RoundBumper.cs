using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class RoundBumper : MonoBehaviour
    {
        public GameObject _stormCloudGameObject;
        private AudioSource _thunderAudioSource,
            _bumperAudioSource;

        public float _bumperForce;
        public float _flashingDuration;
        private float _flashingCountdown;

        private bool _emissionOn = false;
        private Renderer _renderer;
        public Color _originalEmissionColor,
            _targetEmissionColor;
        private Color _currentEmissionColor;
        private float _transitionCurrentTime;
        public float _transitionDuration;

        // Use this for initialization
        void Start()
        {
            _renderer = _stormCloudGameObject.GetComponent<Renderer>();
            _renderer.material.shader = Shader.Find("Standard");
            Shader.EnableKeyword("_EmissionColor");
            _flashingCountdown = _flashingDuration;

            _thunderAudioSource = _stormCloudGameObject.GetComponent<AudioSource>();
            _bumperAudioSource = GetComponent<AudioSource>();
        }

        /*void Update()
        {
            Renderer renderer = GetComponent<Renderer>();
            Material mat = renderer.material;

            float emission = Mathf.PingPong(Time.time, 1.0f);
            Color baseColor = Color.yellow; //Replace this with whatever you want for your base color at emission level '1'

            Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);

            mat.SetColor("_EmissionColor", finalColor);
        }*/

        /*void Start()
        {
            Renderer rend = GetComponent<Renderer>();
            rend.material.shader = Shader.Find("Specular");
            rend.material.SetColor("_SpecColor", Color.red);
        }*/

        // Update is called once per frame
        void Update()
        {
            if (_emissionOn)
            {
                _transitionCurrentTime += Time.deltaTime;
                float ratio = _transitionCurrentTime / _transitionDuration;
                //Debug.Log("ratio = " + ratio);
                if (ratio < 1f) {
                    //Debug.Log("_currentEmissionColor.r ==" + Mathf.Lerp(_originalEmissionColor.r, _targetEmissionColor.r, ratio));
                    _currentEmissionColor = new Color(Mathf.Lerp(_originalEmissionColor.r, _targetEmissionColor.r, ratio),
                        Mathf.Lerp(_originalEmissionColor.g, _targetEmissionColor.g, ratio),
                        Mathf.Lerp(_originalEmissionColor.b, _targetEmissionColor.b, ratio));
                    _renderer.material.SetColor("_EmissionColor", _currentEmissionColor);
                } else
                {
                    _flashingCountdown -= Time.deltaTime;
                    if (_flashingCountdown > 0f)
                    {
                        float colorModifier = Random.Range(0f, 0.4f);
                        _currentEmissionColor = _targetEmissionColor;
                        _currentEmissionColor.r = _currentEmissionColor.r * colorModifier;
                        _currentEmissionColor.g = _currentEmissionColor.g * colorModifier;
                        _currentEmissionColor.b = _currentEmissionColor.b * colorModifier;
                        _renderer.material.SetColor("_EmissionColor", _currentEmissionColor);
                    } else
                    {
                        //Debug.Log("_emissionOn = false");
                        _emissionOn = false;
                        _renderer.material.SetColor("_EmissionColor", _originalEmissionColor);
                        _flashingCountdown = _flashingDuration;
                    }
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            //Debug.Log("Round Bumper collision!");
            Vector3 difference = collision.gameObject.transform.position - gameObject.transform.position;
            difference.z = 0f;
            collision.gameObject.GetComponent<Rigidbody>().AddForce(difference * _bumperForce, ForceMode.Impulse);

            float randomPitch = Random.Range(0.8f, 1.2f);
            _bumperAudioSource.pitch = randomPitch;
            _bumperAudioSource.Play();

            if (!_emissionOn) {
                _emissionOn = true;
                _transitionCurrentTime = 0f;

                randomPitch = Random.Range(0.6f, 1f);
                _thunderAudioSource.pitch = randomPitch;
                _thunderAudioSource.Play();
            }
        }
    }
}
