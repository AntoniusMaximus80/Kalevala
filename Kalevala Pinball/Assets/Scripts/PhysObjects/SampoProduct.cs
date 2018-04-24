using UnityEngine;

namespace Kalevala
{
    public enum SampoProductType
    {
        Grain,
        Salt,
        Gold,
        None
    }

    public class SampoProduct : MonoBehaviour
    {
        public float _startFadingOutTime,
            _startFadingOutTimeVariance,
            _fadeOutTimeBase;
        private float _startFadingOutTimeCounter = 0f,
            _fadeOutTimeCountdown,
            _fadeOutTimeModified;
        public Color _originalColor;
        private Color _newColor;
        private SampoProductType _sampoProductType;
        private Sampo _sampo;

        public SampoProductType Type
        {
            get
            {
                return _sampoProductType;
            }
        }

        public void Init (SampoProductType sampoProductType, Sampo sampo, Vector3 spawnPosition)
        {
            _sampoProductType = sampoProductType;
            _sampo = sampo;
            transform.position = spawnPosition;
            _fadeOutTimeModified = _fadeOutTimeBase + Random.Range(-_startFadingOutTimeVariance, _startFadingOutTimeVariance);
            _fadeOutTimeCountdown = _fadeOutTimeModified;
            GetComponent<Renderer>().material.color = _originalColor;
            _newColor = _originalColor;
            _startFadingOutTimeCounter = 0f;
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
                _newColor.a = Mathf.Clamp01(_fadeOutTimeCountdown / _fadeOutTimeModified);
                GetComponent<Renderer>().material.color = _newColor;
                if (_fadeOutTimeCountdown <= 0f)
                {
                    // Reset the product's color before returning it to the pool.
                    GetComponent<Renderer>().material.color = _originalColor;

                    // Reset the product's velocity before returning it to the pool.
                    GetComponent<Rigidbody>().velocity = Vector3.zero;

                    _sampo.ReturnProductToPool(this);
                }
            }
        }
    }
}