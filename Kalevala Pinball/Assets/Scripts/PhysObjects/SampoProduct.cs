using UnityEngine;

namespace Kalevala
{
    public class SampoProduct : MonoBehaviour
    {
        public float _startFadingOutTime,
            _fadeOutTime;
        private float _startFadingOutTimeCounter = 0f,
            _fadeOutTimeCountdown;
        private Color _currentColor;

        // Use this for initialization
        void Start()
        {
            _currentColor = GetComponent<Renderer>().material.color;
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
                if (_fadeOutTimeCountdown <= 0f)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}