using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class KanteleHeroTriggers: MonoBehaviour
    {
        [SerializeField]
        private GameObject _myLight;
        private KanteleHeroPanel _parent;
        private float _startTime;
        private float _triggerActiveTimeBeforeMiss;
        private int _noteNumber;
        private bool _warmUp;
        private bool _triggerLinger;
        private float _triggerLingerTime = 0.02f;
        private float _timeElapsed;
        private float _warmUpTime;
        private float _warmUpFlashTime;
        // Use this for initialization
        void Start()
        {
            _myLight.SetActive(false);
        }

        public void Init( KanteleHeroPanel parent )
        {
            _parent = parent;
        }

        /// <summary>
        /// Activates the trigger and starts the timer 
        /// in which the player needs to press corresponding button to succesfully play the kantele
        /// </summary>
        /// <param name="triggerActiveTimeBeforeMiss"> The time before the trigger shuts itself
        /// and increases misses on the panel by 1 </param>
        public void ActivateLight(float triggerActiveTimeBeforeMiss, int noteNumber )
        {
            if(_myLight.activeInHierarchy)
            {
                _parent.LightMissed(noteNumber, false);
            }
            _myLight.SetActive(true);
            _startTime = 0;
            _noteNumber = noteNumber;
            _triggerActiveTimeBeforeMiss = triggerActiveTimeBeforeMiss;
        }
        
        public void DeactivateLight()
        {
            _myLight.SetActive(false);
        }

        /// <summary>
        /// Runs the timer before the trigger shuts itself
        /// </summary>
        private void Update()
        {
            if(_warmUp)
            {
                _warmUpTime += Time.deltaTime;
                _warmUpFlashTime += Time.deltaTime;
                if(_warmUpTime < 2f)
                {
                    if(_warmUpFlashTime >= 0.5f / _parent.CurrentDifficulty)
                    {
                        FlashTriggers();
                    }
                } else
                {
                    DeactivateLight();
                    _warmUp = false;
                    _warmUpTime = 0f;
                }
                return;
            }
            if(_myLight.activeInHierarchy)
            {
                _startTime += Time.deltaTime;
                if(_startTime > _triggerActiveTimeBeforeMiss)
                {
                    _parent.LightExpired(_noteNumber);
                    DeactivateLight();
                }
            }
            if(_triggerLinger)
            {
                TriggerLinger();
            }
        }

        /// <summary>
        /// Gets input from player if corresponding button is pressed
        /// if the button is pressed when the light is not on,
        /// player gets 1 miss on the panel
        /// </summary>
        public void TriggerPressed()
        {
            if(!_warmUp) {
                if(_triggerLinger)
                {
                    _parent.LightMissed(_noteNumber, true);
                }
                _triggerLinger = true;
                _timeElapsed = 0f;
            }
        }

        private void TriggerLinger()
        {
            _timeElapsed += Time.deltaTime;
            
            if(_myLight.activeInHierarchy)
            {
                _parent.LightHit(_noteNumber);
                DeactivateLight();
                _triggerLinger = false;
                _timeElapsed = 0f;
            }
            else
            {
                if(_timeElapsed > _triggerLingerTime)
                {
                    _parent.LightMissed(_noteNumber, true);
                    _triggerLinger = false;
                    _timeElapsed = 0f;
                }
            }
        }

        public void ResetNoteNumber()
        {
            _noteNumber = 0;
            _warmUp = true;
            _warmUpTime = 0f;
            _warmUpFlashTime = 0f;
        }

        private void FlashTriggers()
        {
            if(_myLight.activeInHierarchy)
            {
                _myLight.SetActive(false);
            }
            else
            {
                _myLight.SetActive(true);
            }
            _warmUpFlashTime = 0f;
        }
    }
}
