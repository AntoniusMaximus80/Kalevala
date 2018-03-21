using UnityEngine;

namespace Kalevala
{
    public class PopBumper : MonoBehaviour
    {
        public GameObject _bumper;

        public AudioSource _popBumperAudioSource;

        [SerializeField, Range(0f, 64f)]
        private float _bumperForce;

        [SerializeField, Tooltip("The pop bumper force shouldn't be constant. Constant force enables endless ping ponging between two pop bumpers."), Range(0.1f, 0.5f)]
        private float _bumperForceRandomModifier;

        [SerializeField, Range(0f, 1f)]
        private float _randomForceVectorMagnitude;

        private Vector3 _bumperUpPosition,
            _bumperDownPosition;

        public float _animationDuration;

        private float _animationFrame;

        private bool _bumping = false,
            _bumpingDown = true;

        private void Start()
        {
            _bumperUpPosition = _bumper.transform.position;
            _bumperDownPosition = _bumper.transform.position - new Vector3(0f, 1f, 0f);
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

                if (_animationFrame == _animationDuration)
                {
                    _bumpingDown = false;
                }

                if (_animationFrame == 0)
                {
                    _bumpingDown = true;
                    _bumping = false;
                }

                float ratio = _animationFrame / _animationDuration;
                //Debug.Log("Ratio == " + ratio);
                _bumper.transform.position = Vector3.Lerp(_bumperUpPosition, _bumperDownPosition, ratio);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Scoring.
            Scorekeeper.Instance.AddScore(Scorekeeper.ScoreType.PopBumper);

            // Physics.
            other.GetComponent<Pinball>().StopMotion();
            Vector3 forceDirection = Vector3.Normalize(other.transform.position - transform.position) +
                new Vector3(Random.Range(-_randomForceVectorMagnitude, _randomForceVectorMagnitude), 0f, Random.Range(-_randomForceVectorMagnitude, _randomForceVectorMagnitude));
            forceDirection.y = 0f; // Null the vertical movement.
            other.GetComponent<Rigidbody>().AddForce(forceDirection * _bumperForce * (1f + Random.Range(-_bumperForceRandomModifier, _bumperForceRandomModifier)),
                ForceMode.Impulse);

            // Audio.
            if (!_popBumperAudioSource.isPlaying)
            {
                float randomPitch = Random.Range(0.8f, 1.2f);
                _popBumperAudioSource.pitch = randomPitch;
                _popBumperAudioSource.Play();
            }

            // Animation.
            _bumping = true;
        }
    }
}
