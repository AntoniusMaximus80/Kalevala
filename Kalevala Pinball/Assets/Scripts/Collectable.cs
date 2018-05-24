using System.Collections;
using UnityEngine;

namespace Kalevala
{
    [RequireComponent(typeof(Collider))]
    public class Collectable : MonoBehaviour
    {
        //public enum CollectableType
        //{
        //    None = 0,
        //    Grain = 1,
        //    Salt = 2,
        //    Gold = 3
        //}

        private enum CollectableState
        {
            Off = 0,
            Idle = 1,
            Active = 2,
            Vanish = 3,
            Rest = 4
        }

        [SerializeField]
        private GameObject _grainModel;

        [SerializeField]
        private GameObject _saltModel;

        [SerializeField]
        private GameObject _goldModel;

        [SerializeField]
        private bool _removedWhenCollected;

        [SerializeField]
        private bool _respawnsAfterRest;

        [SerializeField]
        private float _motionSpeed;

        /// <summary>
        /// The time (in seconds) until the collectable expires.
        /// Unlimited if the value is 0.
        /// </summary>
        [SerializeField, Tooltip("The time (in seconds) until the collectable expires. Unlimited if the value is 0.")]
        private float _lifeTime = 25;

        /// <summary>
        /// The time (in seconds) after being collected
        /// until the collectable is made invisible
        /// </summary>
        [SerializeField]
        private float _vanishMotionTime = 1;

        /// <summary>
        /// The duration (in seconds) of particle
        /// effects after being collected
        /// </summary>
        [SerializeField]
        private float _particleTime = 1;

        /// <summary>
        /// The duration (in seconds) of rest before being respawned
        /// </summary>
        [SerializeField]
        private float _restingTime;

        /// <summary>
        /// The collectable item's model, or, if it has
        /// not been set, this component's game object
        /// </summary>
        private GameObject _collectableObject;

        private Vector3 _defaultPosition;
        private Quaternion _defaultRotation;

        private Material _material;
        private ParticleSystem _particles;
        private CollectableSpawner _handler;

        private CollectableState State { get; set; }
        private bool _motionCompleted = false;
        private float _activeTime = 0;
        private float _elapsedTime = 0;

        public SampoProductType Type { get; private set; }
        private Scorekeeper.ScoreType _scoreType;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            _particles = GetComponent<ParticleSystem>();

            if (_particles != null)
            {
                ParticleSystem.MainModule psMain = _particles.main;
                psMain.duration = _particleTime;

                _activeTime = Mathf.Max(_vanishMotionTime, _particleTime);
            }
            else
            {
                _activeTime = _vanishMotionTime;
            }
        }

        public void Init(SampoProductType type)
        {
            Type = type;

            switch (Type)
            {
                case SampoProductType.Grain:
                {
                    _collectableObject = _grainModel;
                    _scoreType = Scorekeeper.ScoreType.CollectableGrain;
                    break;
                }
                case SampoProductType.Salt:
                {
                    _collectableObject = _saltModel;
                    _scoreType = Scorekeeper.ScoreType.CollectableSalt;
                    break;
                }
                case SampoProductType.Gold:
                {
                    _collectableObject = _goldModel;
                    _scoreType = Scorekeeper.ScoreType.CollectableGold;
                    break;
                }
                default:
                {
                    _collectableObject = null;
                    Debug.LogError("Invalid collectable type.");
                    break;
                }
            }

            SetDefaults();

            _motionCompleted = false;
            _elapsedTime = 0;

            State = CollectableState.Idle;
        }

        public void SetDefaults()
        {
            _defaultPosition = _collectableObject.transform.position;
            _defaultRotation = _collectableObject.transform.rotation;
            _material = _collectableObject.GetComponentInChildren<Renderer>().material;
        }

        public void ResetToDefaults()
        {
            if (_collectableObject != null)
            {
                _collectableObject.transform.position = _defaultPosition;
                _collectableObject.transform.rotation = _defaultRotation;
            }
        }

        public void SetHandler(CollectableSpawner handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        protected virtual void Update()
        {
            if (_collectableObject != null)
            {
                switch (State)
                {
                    case CollectableState.Idle:
                    {
                        UpdateIdle();
                        break;
                    }
                    case CollectableState.Active:
                    {
                        UpdateActivity();
                        break;
                    }
                    case CollectableState.Vanish:
                    {
                        UpdateVanishing();
                        break;
                    }
                    case CollectableState.Rest:
                    {
                        UpdateRest();
                        break;
                    }
                }
            }
        }

        protected virtual void UpdateActivity()
        {
            UpdateActiveMotion();

            _elapsedTime += Time.deltaTime;

            if (_elapsedTime > _activeTime)
            {
                _motionCompleted = false;
                _elapsedTime = 0;

                if (_removedWhenCollected)
                {
                    Remove();
                }
                else if (_restingTime > 0)
                {
                    State = CollectableState.Rest;
                }
                else
                {
                    State = CollectableState.Idle;
                }
            }
            else
            {
                if (!_motionCompleted &&
                    _elapsedTime > _vanishMotionTime)
                {
                    _motionCompleted = true;

                    if (_removedWhenCollected)
                    {
                        ShowCollectableObject(false);
                    }
                    else
                    {
                        ResetToDefaults();
                    }
                }
            }
        }

        protected virtual void UpdateVanishing()
        {
            UpdateVanishMotion();

            _elapsedTime += Time.deltaTime;

            if (_elapsedTime > _activeTime)
            {
                _motionCompleted = false;
                _elapsedTime = 0;
                Remove();
            }
            else
            {
                if (!_motionCompleted &&
                    _elapsedTime > _vanishMotionTime)
                {
                    _motionCompleted = true;
                    ShowCollectableObject(false);
                }
            }
        }

        protected virtual void UpdateRest()
        {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime > _restingTime)
            {
                State = CollectableState.Idle;
                _elapsedTime = 0;

                if (_respawnsAfterRest)
                {
                    Respawn();
                }
            }
        }

        protected virtual void UpdateIdle()
        {
            UpdateIdleMotion();

            if (_lifeTime > 0)
            {
                _elapsedTime += Time.deltaTime;

                if (_elapsedTime > _lifeTime)
                {
                    _elapsedTime = 0;
                    State = CollectableState.Vanish;
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

        protected virtual void UpdateVanishMotion()
        {
            Vector3 newPosition = _collectableObject.transform.position;
            newPosition.y -= _motionSpeed * Time.deltaTime;
            _collectableObject.transform.position = newPosition;
        }

        protected virtual void UpdateIdleMotion()
        {
            Vector3 newRotation = _collectableObject.transform.rotation.eulerAngles;
            newRotation.y += 75 * _motionSpeed * Time.deltaTime;

            _collectableObject.transform.rotation = Quaternion.Euler(newRotation);
        }

        public virtual void Activate()
        {
            // Returns if already collected
            if (!_collectableObject.activeSelf)
            {
                return;
            }

            State = CollectableState.Active;
            _elapsedTime = 0;

            // Gives score based on the collectable type
            if (Type != SampoProductType.None)
            {
                Scorekeeper.Instance.AddScore(_scoreType);
            }

            // Plays a particle effect
            if (_particles != null)
            {
                _particles.Play();
            }

            Debug.Log(Type + " collected");
        }

        public virtual void Respawn()
        {
            State = CollectableState.Idle;
            ResetToDefaults();
            ShowCollectableObject(true);
        }

        public virtual void Remove()
        {
            State = CollectableState.Off;
            ResetToDefaults();
            _handler.ReturnItemToPool(this);

            // If _cO is this.gameObject, respawning doesn't work
            // (Update is not called)
            //if (!(_respawnsAfterRest && _collectableObject == gameObject))
            //{
            //    _collectableObject.SetActive(false);
            //}
        }

        public void ShowCollectableObject(bool show)
        {
            if (_collectableObject != null)
            {
                _collectableObject.SetActive(show);
            }
        }

        public void LaunchToPosition(Vector3 launchPosition, float speed)
        {
            if (speed > 0)
            {
                Vector3 targetPosition = _collectableObject.transform.position;
                float duration =
                    Vector3.Distance(launchPosition, targetPosition) / speed;

                StartCoroutine(FlyToPositionRoutine
                                   (launchPosition, targetPosition, duration));
            }
        }

        private IEnumerator FlyToPositionRoutine(Vector3 start, Vector3 target,
            float duration)
        {
            float startTime = Time.time;
            float ratio = 0;

            Vector3 center = (start + target) / 2f;
            center.y -= 1f;
            Vector3 startCenter = start - center;
            Vector3 targetCenter = target - center;

            _collectableObject.transform.position = start;
            Vector3 baseScale = _collectableObject.transform.localScale;
            Color baseColor = _material.color;

            while (ratio < 1)
            {
                ratio = (Time.time - startTime) / duration;

                // Moves the object in an arc towards the target position
                _collectableObject.transform.position =
                    Vector3.Slerp(startCenter, targetCenter, ratio);
                _collectableObject.transform.position += center;

                // Increases the object's size as it
                // gets closer to the target position
                _collectableObject.transform.localScale =
                    new Vector3(ratio * baseScale.x,
                                ratio * baseScale.y,
                                ratio * baseScale.z);

                // Changes the object's transparency
                Color newColor = _material.color;
                newColor.a = ratio;
                _material.color = newColor;

                yield return 0;
            }

            _collectableObject.transform.position = target;
            _collectableObject.transform.localScale = baseScale;
            _material.color = baseColor;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (State == CollectableState.Idle)
            {
                Pinball pinball = other.GetComponent<Pinball>();
                if (pinball != null)
                {
                    Activate();
                }
            }
        }
    }
}
