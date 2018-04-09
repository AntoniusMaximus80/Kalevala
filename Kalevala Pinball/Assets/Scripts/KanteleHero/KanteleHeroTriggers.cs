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
        // Use this for initialization
        void Start()
        {
            _myLight.SetActive(false);
        }

        public void Init( KanteleHeroPanel parent )
        {
            _parent = parent;
        }

        public void ActivateLight(float triggerActiveTimeBeforeMiss )
        {
            if(_myLight.activeInHierarchy)
            {
                _parent.LightMissed();
            }
            _myLight.SetActive(true);
            _startTime = 0;
            _triggerActiveTimeBeforeMiss = triggerActiveTimeBeforeMiss;
        }

        public void DeactivateLight()
        {
            _myLight.SetActive(false);
        }

        private void Update()
        {
            if(_myLight.activeInHierarchy)
            {
                _startTime += Time.deltaTime;
                if(_startTime > _triggerActiveTimeBeforeMiss)
                {

                    Debug.Log("LightMissed");
                    _parent.LightMissed();
                    DeactivateLight();
                }
            }
        }

        public void TriggerPressed()
        {
            if(_myLight.activeInHierarchy)
            {
                DeactivateLight();
            } else
            {
                _parent.LightMissed();
            }
        }
    }
}
