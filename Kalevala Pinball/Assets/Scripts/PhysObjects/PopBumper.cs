﻿using UnityEngine;

namespace Kalevala
{
    public class PopBumper : MonoBehaviour
    {
        public GameObject _bumper;

        #region Light
        public Light _light;
        public float _strobeSpeed;
        public float _lightMaximumIntensity;
        private float _lightCurrentIntensity = 0f;
        //public GameObject _body,
        //    _cover,
        //    _bulb;
        //private float _lightMaximumIntensity,
        //    _lightCurrentIntensity = 0f,
        //    _bodyMaterialEmissionMax,
        //    _coverEmissionMax,
        //    _bulbEmissionMax,
        //    _bodyEmissionCurrent = 0f,
        //    _coverEmissionCurrent = 0f,
        //    _bulbEmissionCurrent = 0f;
        #endregion

        [SerializeField, Range(0f, 64f)]
        private float _bumperForce;

        [SerializeField, Tooltip("The pop bumper force shouldn't be constant. Constant force enables endless ping ponging between two pop bumpers."), Range(0.1f, 0.5f)]
        private float _bumperForceRandomModifier;

        [SerializeField, Range(0f, 1f)]
        private float _randomForceVectorMagnitude;

        //[SerializeField, Range(0f, 1f)]
        //private float _shakeMagnitude;

        private Vector3 _bumperUpPosition,
            _bumperDownPosition;

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
            //_hayStakeStartingPosition = _hayStake.transform.position;
            _animationFrame = 0;
            _light.intensity = _lightCurrentIntensity;
            
            //_bodyMaterialEmissionMax = _body.GetComponent<Renderer>().material.;
            //_coverMaterial = _cover.GetComponent<Renderer>().material;
            //_bulbMaterial = _bulb.GetComponent<Renderer>().material;
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
                //_hayStake.transform.position = _hayStakeStartingPosition; // Reset hay stake's position before shaking.
                //Shake(_hayStake);

                if (_animationFrame == 0)
                {
                    _bumpingDown = true;
                    _bumping = false;
                    //_hayStake.transform.position = _hayStakeStartingPosition; // Reset hay stake's position at the end of the animation.
                }

                _lightCurrentIntensity += Time.deltaTime * _strobeSpeed;
                _light.intensity = _lightCurrentIntensity;
                if (_lightCurrentIntensity >= _lightMaximumIntensity)
                {
                    _lightCurrentIntensity = 0f;
                }
            } else
            {
                _light.intensity = 0f;
                _lightCurrentIntensity = 0f;
            }

            //if (_hayParticleSystem.activeInHierarchy)
            //{
            //    _hayParticleCounter += Time.deltaTime;
            //    if (_hayParticleCounter >= _hayParticleDuration)
            //    {
            //        _hayParticleCounter = 0f;
            //        _hayParticleSystem.SetActive(false);
            //    }
            //}
        }

        private void OnTriggerEnter(Collider other)
        {
            // Scoring.
            Scorekeeper.Instance.AddScore(Scorekeeper.ScoreType.PopBumper);

            //Add Resources
            PinballManager.Instance.ChangeResources(1);

            // Physics.
            other.GetComponent<Pinball>().StopMotion(); // Stop the pinball's velocity.
            Vector3 forceDirection = Vector3.Normalize(other.transform.position - transform.position) + // Calculate the force direction.
                new Vector3(Random.Range(-_randomForceVectorMagnitude, _randomForceVectorMagnitude), 0f, Random.Range(-_randomForceVectorMagnitude, _randomForceVectorMagnitude)); // Add a random 2D force to the force direction.
            forceDirection.y = 0f; // Null the vertical movement of the vector.

            other.GetComponent<Rigidbody>().AddForce(forceDirection * _bumperForce * (1f + Random.Range(-_bumperForceRandomModifier, _bumperForceRandomModifier)),
                ForceMode.Impulse);

            // Audio.
            float randomPitch = Random.Range(0.8f, 1.2f);
            SFXPlayer.Instance.Play(Sound.Bumper, randomPitch);
            SFXPlayer.Instance.Play(Sound.BumperParticle, randomPitch);

            //// Particle system.
            //if (!_hayParticleSystem.activeInHierarchy)
            //{
            //    _hayParticleSystem.SetActive(true);
            //}

            // Animation.
            _bumping = true;
        }

        //private void Shake(GameObject gameObject)
        //{
        //    gameObject.transform.position += new Vector3(Random.Range(-_shakeMagnitude, _shakeMagnitude), 0f, Random.Range(-_shakeMagnitude, _shakeMagnitude));
        //}
    }
}
