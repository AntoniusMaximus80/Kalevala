using UnityEngine;

namespace Kalevala
{
    public class PopBumper : MonoBehaviour
    {
        public GameObject _bumper,
            _hayStake,
            _hayParticleSystem;

        public AudioSource _popBumperAudioSource,
            _hayAudioSource;

        [SerializeField, Range(0f, 64f)]
        private float _bumperForce;

        [SerializeField, Tooltip("The pop bumper force shouldn't be constant. Constant force enables endless ping ponging between two pop bumpers."), Range(0.1f, 0.5f)]
        private float _bumperForceRandomModifier;

        [SerializeField, Range(0f, 1f)]
        private float _randomForceVectorMagnitude;

        [SerializeField, Range(0f, 1f)]
        private float _shakeMagnitude;

        private Vector3 _bumperUpPosition,
            _bumperDownPosition,
            _hayStakeStartingPosition;

        public float _halfAnimationDuration;

        private float _animationFrame,
            _hayParticleDuration = 1.5f,
            _hayParticleCounter = 0f;

        private bool _bumping = false,
            _bumpingDown = true;

        private void Start()
        {
            _bumperUpPosition = _bumper.transform.position;
            _bumperDownPosition = _bumper.transform.position - new Vector3(0f, 1f, 0f);
            _hayStakeStartingPosition = _hayStake.transform.position;
            _animationFrame = 0;
        }

        private void Update()
        {
            if (_bumping)
            {
                if (_bumpingDown)
                {
                    _animationFrame++;
                } else
                {
                    _animationFrame--;
                }    

                if (_animationFrame == _halfAnimationDuration)
                {
                    _bumpingDown = false;
                }

                float ratio = _animationFrame / _halfAnimationDuration;
                //Debug.Log("Ratio == " + ratio);
                _bumper.transform.position = Vector3.Lerp(_bumperUpPosition, _bumperDownPosition, ratio);
                _hayStake.transform.position = _hayStakeStartingPosition; // Reset hay stake's position before shaking.
                Shake(_hayStake);

                if (_animationFrame == 0)
                {
                    _bumpingDown = true;
                    _bumping = false;
                    _hayStake.transform.position = _hayStakeStartingPosition; // Reset hay stake's position at the end of the animation.
                }
            }

            if (_hayParticleSystem.activeInHierarchy)
            {
                _hayParticleCounter += Time.deltaTime;
                if (_hayParticleCounter >= _hayParticleDuration)
                {
                    _hayParticleCounter = 0f;
                    _hayParticleSystem.SetActive(false);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Scoring.
            Scorekeeper.Instance.AddScore(Scorekeeper.ScoreType.PopBumper);

            // Physics.
            other.GetComponent<Pinball>().StopMotion(); // Stop the pinball's velocity.
            Vector3 forceDirection = Vector3.Normalize(other.transform.position - transform.position) + // Calculate the force direction.
                new Vector3(Random.Range(-_randomForceVectorMagnitude, _randomForceVectorMagnitude), 0f, Random.Range(-_randomForceVectorMagnitude, _randomForceVectorMagnitude)); // Add a random 2D force to the force direction.
            forceDirection.y = 0f; // Null the vertical movement of the vector.

            other.GetComponent<Rigidbody>().AddForce(forceDirection * _bumperForce * (1f + Random.Range(-_bumperForceRandomModifier, _bumperForceRandomModifier)),
                ForceMode.Impulse);

            // Audio.
            if (!_popBumperAudioSource.isPlaying)
            {
                float randomPitch = Random.Range(0.8f, 1.2f);
                _popBumperAudioSource.pitch = randomPitch;
                _popBumperAudioSource.Play();
            }

            if (!_hayAudioSource.isPlaying)
            {
                float randomPitch = Random.Range(0.8f, 1.2f);
                _hayAudioSource.pitch = randomPitch;
                _hayAudioSource.Play();
            }

            // Particle system.
            if (!_hayParticleSystem.activeInHierarchy)
            {
                _hayParticleSystem.SetActive(true);
            }

            // Animation.
            _bumping = true;
        }

        private void Shake(GameObject gameObject)
        {
            gameObject.transform.position += new Vector3(Random.Range(-_shakeMagnitude, _shakeMagnitude), 0f, Random.Range(-_shakeMagnitude, _shakeMagnitude));
        }
    }
}
