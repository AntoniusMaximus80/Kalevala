using UnityEngine;

namespace Kalevala
{
    public class SampoProduct : MonoBehaviour
    {
        public float _startFadingOutTime,
            _startFadingOutTimeVariance,
            _fadeOutTime;
        private float _startFadingOutTimeCounter = 0f,
            _fadeOutTimeCountdown;
        private Color _currentColor;

        // Use this for initialization
        void Start()
        {
            _currentColor = GetComponent<Renderer>().material.color;
            _fadeOutTime += Random.Range(-_startFadingOutTimeVariance, _startFadingOutTimeVariance);
            _fadeOutTimeCountdown = _fadeOutTime;
        }

        // Update is called once per frame
        void Update()
        {
            if (_startFadingOutTimeCounter < _startFadingOutTime)
            {
                _startFadingOutTimeCounter += Time.deltaTime;
            } else
            {
                _fadeOutTimeCountdown -= Time.deltaTime;
                _currentColor.a = Mathf.Clamp01(_fadeOutTimeCountdown / _fadeOutTime);
                GetComponent<Renderer>().material.color = _currentColor;
                if (_fadeOutTimeCountdown <= 0f)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}