using UnityEngine;

namespace Kalevala
{
    public class KanteleBumper : MonoBehaviour
    {
        public GameObject _bumperInactive,
            _bumperActive,
            _bumperForceOrigin;
        public AudioSource _kanteleBumperAudioSource;
        public int _bumperActiveFrames;
        private int _bumperActiveCountdown = 0;

        [SerializeField, Range(0f, 32f)]
        private float _bumperForce;

        [SerializeField, Range(0.1f, 0.5f)]
        private float _bumperForceRandomModifier;

        private Collider _bumperCollider;

        // Use this for initialization
        void Start()
        {
            _bumperCollider = GetComponent<Collider>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_bumperActiveCountdown != 0)
            {
                _bumperActiveCountdown--;
                if (_bumperActiveCountdown == 0)
                {
                    _bumperCollider.enabled = true;
                    _bumperActive.SetActive(false);
                    _bumperInactive.SetActive(true);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Scorekeeper.Instance.AddScore(Scorekeeper.ScoreType.KanteleBumper);
            _bumperActiveCountdown = _bumperActiveFrames;
            _bumperCollider.enabled = false;
            _bumperInactive.SetActive(false);
            _bumperActive.SetActive(true);
            other.GetComponent<Rigidbody>().velocity = Vector3.zero;
            other.GetComponent<Rigidbody>().AddForce(other.transform.position - _bumperForceOrigin.transform.position * _bumperForce * (1f + Random.Range(-_bumperForceRandomModifier, _bumperForceRandomModifier)),
                ForceMode.Impulse);

            if (!_kanteleBumperAudioSource.isPlaying)
            {
                float randomPitch = Random.Range(0.8f, 1.2f);
                _kanteleBumperAudioSource.pitch = randomPitch;
                _kanteleBumperAudioSource.Play();
            }
        }
    }
}