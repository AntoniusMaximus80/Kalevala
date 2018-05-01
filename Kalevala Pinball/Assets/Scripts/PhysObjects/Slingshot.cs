using UnityEngine;
using System.Collections;

namespace Kalevala
{
    public class Slingshot : MonoBehaviour
    {
        public GameObject _slingshotForceOrigin,
            _strings;
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

        private Coroutine _coroutine;

        private void Start()
        {
            _stringsStartPosition = _strings.transform.position;
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
                float randomPitch = Random.Range(0.8f, 1.2f);
                SFXPlayer.Instance.Play(Sound.Slingshot, randomPitch);
                SFXPlayer.Instance.Play(Sound.Bumper, randomPitch);

                // Animation.
                _animator.SetTrigger("Activate");
                if(_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                }
                _coroutine = StartCoroutine(Shake(1f, _strings));
            }
        }

        private void HorizontalShake(GameObject gameObject)
        {
            gameObject.transform.position += new Vector3(Random.Range(-_shakeMagnitude, _shakeMagnitude), 0f, 0f);
        }

        private IEnumerator Shake(  float duration, GameObject strings)
        {
            float time = 0f;
            float startTime = Time.time;
            while(time < 1)
            {
                time = (Time.time - startTime) / duration;
                strings.transform.position = _stringsStartPosition;
                strings.transform.position += new Vector3(Random.Range(-_shakeMagnitude, _shakeMagnitude), 0f, 0f);
                yield return 0;
            }
            strings.transform.position = _stringsStartPosition;
        }
    }
}