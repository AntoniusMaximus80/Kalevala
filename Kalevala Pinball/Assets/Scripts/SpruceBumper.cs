using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class SpruceBumper : MonoBehaviour
    {
        public GameObject _needlesParticleSystemPrefab;
        private AudioSource _spruceBumperHitAudioSource;

        [SerializeField, Range(0f, 32f)]
        private float _bumperForce;

        void Start()
        {
            _spruceBumperHitAudioSource = GetComponent<AudioSource>();
        }

        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Spruce Bumper collision!");
            Vector3 difference = other.gameObject.transform.position - gameObject.transform.position;
            difference.y = 0f;
            other.gameObject.GetComponent<Rigidbody>().AddForce(difference * _bumperForce, ForceMode.Impulse);

            Instantiate(_needlesParticleSystemPrefab, transform);

            if (!_spruceBumperHitAudioSource.isPlaying)
            {
                float randomPitch = Random.Range(0.8f, 1.2f);
                _spruceBumperHitAudioSource.pitch = randomPitch;
                _spruceBumperHitAudioSource.Play();
            }
        }
    }
}
