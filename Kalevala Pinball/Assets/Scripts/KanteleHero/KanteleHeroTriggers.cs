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
            if(_myLight.activeInHierarchy)
            {
                _startTime += Time.deltaTime;
                if(_startTime > _triggerActiveTimeBeforeMiss)
                {

                    Debug.Log("LightMissed");
                    _parent.LightMissed(_noteNumber, false);
                    DeactivateLight();
                }
            }
        }

        /// <summary>
        /// Gets input from player if corresponding button is pressed
        /// if the button is pressed when the light is not on,
        /// player gets 1 miss on the panel
        /// </summary>
        public void TriggerPressed()
        {
            if(_myLight.activeInHierarchy)
            {
                _parent.LightHit(_noteNumber);
                DeactivateLight();
            } else
            {
                _parent.LightMissed(_noteNumber, true);
            }
        }
    }
}
