using UnityEngine;

namespace Kalevala
{
    public class SpruceBumper : MonoBehaviour
    {
        public GameObject _needlesParticleSystemPrefab,
            _spruceGameObject;
        public AudioSource _spruceAudioSource,
            _bumperAudioSource;

        [SerializeField, Range(0f, 32f)]
        private float _bumperForce;

        [SerializeField, Tooltip("The bumper force shouldn't be constant, that enables endless ping ponging between two bumpers."), Range(0.1f, 0.25f)]
        private float _bumperForceRandomModifier;

        private void OnTriggerEnter(Collider other)
        {
            //Debug.Log("Spruce Bumper collision!");
            Vector3 difference = other.gameObject.transform.position - gameObject.transform.position;
            difference.y = 0f; // Reset vertical force.
            other.GetComponent<Rigidbody>().velocity = Vector3.zero; // Reset the pinball's velocity, before adding bumper force.
            other.gameObject.GetComponent<Rigidbody>().AddForce(difference * _bumperForce * (1f + Random.Range(-_bumperForceRandomModifier, _bumperForceRandomModifier)),
                ForceMode.Impulse);

            GameObject _newParticleSystem = Instantiate(_needlesParticleSystemPrefab, transform);
            _newParticleSystem.transform.localScale = Vector3.one * _spruceGameObject.transform.localScale.x; // Scale the particle system to match the spruce model's scale.

            if (!_spruceAudioSource.isPlaying)
            {
                float randomPitch = Random.Range(0.8f, 1.2f);
                _spruceAudioSource.pitch = randomPitch;
                _spruceAudioSource.Play();
            } else
            {
                _spruceAudioSource.Stop();
                float randomPitch = Random.Range(0.8f, 1.2f);
                _spruceAudioSource.pitch = randomPitch;
                _spruceAudioSource.Play();
            }

            if (!_bumperAudioSource.isPlaying)
            {
                float randomPitch = Random.Range(0.8f, 1.2f);
                _bumperAudioSource.pitch = randomPitch;
                _bumperAudioSource.Play();
            }

            Scorekeeper.Instance.AddScore(Scorekeeper.ScoreType.SpruceBumper);
        }
    }
}
