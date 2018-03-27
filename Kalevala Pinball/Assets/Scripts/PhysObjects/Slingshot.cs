using UnityEngine;

namespace Kalevala
{
    public class Slingshot : MonoBehaviour
    {
        public GameObject _slingshotForceOrigin,
            _strings;
        public AudioSource _slingshotBumperAudioSource,
            _slingshotKanteleAudioSource;
        public BoxCollider _slingshotActivationCollider;
        public Animator _animator;

        private Vector3 _stringsStartPosition;

        [SerializeField, Range(0f, 128f)]
        private float _slingshotForce;

        [SerializeField, Range(0.1f, 0.5f)]
        private float _slingshotForceRandomModifier;

        [SerializeField, Range(0f, 256f)]
        private float _slingshotActivationSensitivity;

        [SerializeField, Range(0f, 1f)]
        private float _shakeMagnitude;

        private void Start()
        {
            _stringsStartPosition = _strings.transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            if (_slingshotKanteleAudioSource.isPlaying)
            {
                _strings.transform.position = _stringsStartPosition;
                HorizontalShake(_strings);
            } else
            {
                _strings.transform.position = _stringsStartPosition;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Mathf.Abs(other.GetComponent<Pinball>().PhysicsVelocity.z) > _slingshotActivationSensitivity) {
                // Scoring.
                Scorekeeper.Instance.AddScore(Scorekeeper.ScoreType.Slingshot);

                // Physics.
                other.GetComponent<Pinball>().StopMotion();
                Vector3 forceDirection = Vector3.Normalize(other.transform.position - _slingshotForceOrigin.transform.position);
                forceDirection.y = 0f; // Null the vertical movement.
                other.GetComponent<Rigidbody>().AddForce(forceDirection * _slingshotForce * (1f + Random.Range(-_slingshotForceRandomModifier, _slingshotForceRandomModifier)),
                    ForceMode.Impulse);

                // Audio.
                if (!_slingshotBumperAudioSource.isPlaying)
                {
                    float randomPitch = Random.Range(0.8f, 1.2f);
                    _slingshotBumperAudioSource.pitch = randomPitch;
                    _slingshotBumperAudioSource.Play();
                }

                if (!_slingshotKanteleAudioSource.isPlaying)
                {
                    float randomPitch = Random.Range(0.8f, 1.2f);
                    _slingshotKanteleAudioSource.pitch = randomPitch;
                    _slingshotKanteleAudioSource.Play();
                }

                // Animation.
                _animator.SetTrigger("Activate");
            }
        }

        private void HorizontalShake(GameObject gameObject)
        {
            gameObject.transform.position += new Vector3(Random.Range(-_shakeMagnitude, _shakeMagnitude), 0f, 0f);
        }
    }
}