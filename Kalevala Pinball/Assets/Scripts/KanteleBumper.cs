using UnityEngine;

namespace Kalevala
{
    public class KanteleBumper : MonoBehaviour
    {
        public GameObject _bumperInactive,
            _bumperActive,
            _bumperForceOrigin;
        public Scorekeeper _scoreKeeper;
        public int _bumperActiveFrames,
            _bumperScore;
        private int _bumperActiveCountdown = 0;
        public float _bumperForce;
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
            _scoreKeeper.AddScore(_bumperScore);
            _bumperActiveCountdown = _bumperActiveFrames;
            _bumperCollider.enabled = false;
            _bumperInactive.SetActive(false);
            _bumperActive.SetActive(true);
            other.GetComponent<Rigidbody>().AddForce(other.transform.position - _bumperForceOrigin.transform.position * _bumperForce, ForceMode.Impulse);
        }
    }
}