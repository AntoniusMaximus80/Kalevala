using UnityEngine;

namespace Kalevala
{
    public class Slingshot : MonoBehaviour
    {
        public GameObject _slingshotForceOrigin;
        public AudioSource _slingshotBumperAudioSource;
        public BoxCollider _slingshotActivationCollider;
        public Animator _animator;

        [SerializeField, Range(0f, 128f)]
        private float _slingshotForce;

        [SerializeField, Range(0.1f, 0.5f)]
        private float _slingshotForceRandomModifier;

        [SerializeField, Range(0f, 256f)]
        private float _slingshotActivationSensitivity;

        // Update is called once per frame
        void Update()
        {

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

                // Animation.
                _animator.SetTrigger("Activate");
            }
        }
    }
}