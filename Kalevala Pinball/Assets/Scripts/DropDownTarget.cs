using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class DropDownTarget : MonoBehaviour
    {
        private bool _triggered = false,
            _animating = false,
            _animatingDown = true;
        public Scorekeeper _scorekeeper;
        private Collider _collider;
        public float _animationTime;
        private float  _animationTimeCounter;
        private Vector3 _upPosition,
            _downPosition;

        private void Start()
        {
            _collider = GetComponent<Collider>();
            _upPosition = transform.localPosition;
            _downPosition = transform.localPosition;
            _downPosition.z -= 1.96f;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R) && _triggered)
            {
                Reset();
            }
            if (_animating)
            {
                _animationTimeCounter += Time.deltaTime;
                float ratio = _animationTimeCounter / _animationTime;
                Mathf.Clamp01(ratio);

                if (_animatingDown) {
                    transform.localPosition = new Vector3(_downPosition.x,
                        _downPosition.y,
                        Mathf.Lerp(_upPosition.z, _downPosition.z, ratio));
                } else
                {
                    transform.localPosition = new Vector3(_upPosition.x,
                        _upPosition.y,
                        Mathf.Lerp(_downPosition.z, _upPosition.z, ratio));
                }

                if (ratio == 1f)
                {
                    _animating = false;
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            _triggered = true;
            _animating = true;
            _animatingDown = true;
            _collider.enabled = false;
            _scorekeeper.AddScore(10000);
        }

        public void Reset()
        {
            _triggered = false;
            _animating = true;
            _animatingDown = false;
            _collider.enabled = true;
        }
    }
}