using UnityEngine;

namespace Kalevala
{
    [RequireComponent(typeof(Collider))]
    public class CollectableItem : MonoBehaviour
    {
        [SerializeField]
        private GameObject _collectableObject;

        [SerializeField]
        private bool _removedWhenCollected;

        [SerializeField]
        private bool _respawnsAfterRest;

        [SerializeField]
        private float _motionSpeed;

        [SerializeField]
        private float _motionTime = 1;

        [SerializeField]
        private float _particleTime = 1;

        [SerializeField]
        private float _restingTime;

        private Vector3 _position;
        private Quaternion _rotation;

        private ParticleSystem _particles;

        private bool _active = false;
        private bool _motionCompleted = false;
        private float _activeTime = 0;
        private float _elapsedActiveTime = 0;

        private bool _resting = false;
        private float _elapsedRestingTime = 0;

        private void Start()
        {
            if (_collectableObject == null)
            {
                _collectableObject = gameObject;
            }

            _position = _collectableObject.transform.position;
            _rotation = _collectableObject.transform.rotation;

            _particles = GetComponent<ParticleSystem>();

            if (_particles != null)
            {
                ParticleSystem.MainModule psMain = _particles.main;
                psMain.duration = _particleTime;

                _activeTime = Mathf.Max(_motionTime, _particleTime);
            }
            else
            {
                _activeTime = _motionTime;
            }
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        protected virtual void Update()
        {
            if (_active)
            {
                UpdateActivity();
            }
            else if (_resting)
            {
                UpdateRest();
            }
        }

        protected virtual void UpdateActivity()
        {
            UpdateActiveMotion();

            _elapsedActiveTime += Time.deltaTime;

            if (_elapsedActiveTime > _activeTime)
            {
                _active = false;
                Debug.Log("_active: " + _active);
                _motionCompleted = false;
                _elapsedActiveTime = 0;

                if (_removedWhenCollected)
                {
                    Remove();
                }

            }
            else
            {
                if (!_motionCompleted &&
                    _elapsedActiveTime > _motionTime)
                {
                    _motionCompleted = true;

                    if (_removedWhenCollected)
                    {
                        ShowCollectableObject(false);
                    }
                    else
                    {
                        ResetPosAndRot();
                    }
                }
                else if (_elapsedActiveTime > _particleTime)
                {
                    // Does nothing
                }
            }
        }

        protected virtual void UpdateRest()
        {
            _elapsedRestingTime += Time.deltaTime;

            if (_elapsedRestingTime > _restingTime)
            {
                _elapsedRestingTime = 0;
                _resting = false;

                if (_respawnsAfterRest)
                {
                    Spawn();
                }
            }
        }

        protected virtual void UpdateActiveMotion()
        {
            Vector3 newPosition = _collectableObject.transform.position;
            newPosition.y += _motionSpeed * Time.deltaTime;

            Vector3 newRotation = _collectableObject.transform.rotation.eulerAngles;
            newRotation.y += 200 * _motionSpeed * Time.deltaTime;

            _collectableObject.transform.position = newPosition;
            _collectableObject.transform.rotation = Quaternion.Euler(newRotation);
        }

        public virtual void Activate()
        {
            // TODO: What happens when collected?

            // Returns if already collected
            if ( !_collectableObject.activeSelf )
            {
                return;
            }

            _active = true;

            if (_particles != null)
            {
                _particles.Play();
            }

            //Debug.Log(name + " collected");
        }

        public virtual void Spawn()
        {
            // TODO: Take from pool

            ResetPosAndRot();
            ShowCollectableObject(true);
        }

        public virtual void Remove()
        {
            // TODO: Return to pool

            // If _cO is this.gameObject, respawning doesn't work
            // (Update is not called)
            _collectableObject.SetActive(false);
        }

        protected virtual void ShowCollectableObject(bool show)
        {
            _collectableObject.SetActive(show);
        }

        private void ResetPosAndRot()
        {
            _collectableObject.transform.position = _position;
            _collectableObject.transform.rotation = _rotation;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!_resting)
            {
                Pinball pinball = other.GetComponent<Pinball>();
                if (pinball != null)
                {
                    Activate();

                    if (!_removedWhenCollected || _respawnsAfterRest)
                    {
                        _resting = true;
                    }
                }
            }
        }
    }
}
