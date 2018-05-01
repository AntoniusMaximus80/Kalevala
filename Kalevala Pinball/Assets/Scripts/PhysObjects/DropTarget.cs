using UnityEngine;

namespace Kalevala
{
    public class DropTarget : MonoBehaviour
    {
        private bool _triggered = false,
            _animating = false,
            _animatingDown = true;
        private Scorekeeper _scorekeeper;
        private Collider _collider;
        public float _animationTime;
        private float _animationTimeCounter = 0f;
        private Vector3 _upPosition,
            _downPosition;

        private void Start()
        {
            _collider = GetComponent<Collider>();
            _scorekeeper = FindObjectOfType<Scorekeeper>();
            _upPosition = transform.localPosition;
            _downPosition = transform.localPosition;
            _downPosition.z = _upPosition.z - 2.5f;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R) && _triggered) // For debug purposes.
            {
                ResetDropTarget();
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

            if (_collider != null)
            {
                _collider.enabled = false;
            }
            else
            {
                Debug.LogError("Collider is missing.");
            }
            SFXPlayer.Instance.Play(Sound.DropDownTarget);
            Scorekeeper.Instance.AddScore(Scorekeeper.ScoreType.DropTarget);
        }

        public void ResetDropTarget()
        {
            _triggered = false;
            _animating = true;
            _animatingDown = false;
            _animationTimeCounter = 0f;

            if (_collider != null)
            {
                _collider.enabled = true; 
            }
            else
            {
                Debug.LogError("Collider is missing.");
            }
        }
    }
}