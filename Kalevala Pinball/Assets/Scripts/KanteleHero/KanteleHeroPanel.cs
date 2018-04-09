using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public enum LightSide
    {
        left = 0,
        right = 1
    }

    public class KanteleHeroPanel: MonoBehaviour
    {
        [SerializeField]
        private float _spawnInterval;
        [SerializeField]
        private float _minSpawnOnSameSide;
        [SerializeField]
        private float _lightMoveSpeed;
        [SerializeField]
        private float _triggerActiveTimeBeforeMiss;
        [SerializeField]
        private GameObject[] _LeftWaypoints;
        [SerializeField]
        private GameObject[] _rightWaypoints;
        [SerializeField]
        private GameObject[] _missLights;
        [SerializeField]
        private KanteleHeroLight _movingLightPrefab;
        [SerializeField]
        private KanteleHeroTriggers _leftTrigger;
        [SerializeField]
        private KanteleHeroTriggers _rightTrigger;

        private int _misses;
        private Pool<KanteleHeroLight> _kanteleLights;
        private float _spawnTimer;
        private bool _canSpawn;
        private LightSide _spawnSide;

        private bool _panelActive = false;

        public bool PanelActive
        {
            get { return _panelActive; }
            private set { _panelActive = value;}
        }
        // Use this for initialization
        void Start()
        {
            _kanteleLights = new Pool<KanteleHeroLight>(4, true, _movingLightPrefab);
            _leftTrigger.Init(this);
            _rightTrigger.Init(this);
            DeactivateAllMissLights();
        }

        private void SpawnLight(GameObject[] waypoints, LightSide side)
        {
            KanteleHeroLight light = _kanteleLights.GetPooledObject();
            if(light != null)
            {
                light.Init(waypoints, this, _lightMoveSpeed,side);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(PanelActive)
            {
                UpdateSpawnTimer();
                if(_canSpawn)
                {
                    if(_spawnTimer > _minSpawnOnSameSide)
                    {
                        _spawnSide = RandomSide();
                    } else
                    {
                        _spawnSide = SpawnOnOtherSide(_spawnSide);
                    }
                    if(_spawnSide == LightSide.left)
                    {
                        SpawnLight(_LeftWaypoints, _spawnSide);
                    } else if (_spawnSide == LightSide.right)
                    {
                        SpawnLight(_rightWaypoints, _spawnSide);
                    }
                    _canSpawn = false;
                    _spawnTimer = 0;
                }
            }
            if(_misses >= _missLights.Length)
            {
                DeactivatePanel();
            }
        }

        /// <summary>
        /// Turns the left light on and 
        /// </summary>
        public void TurnTriggetLightOn(LightSide side)
        {
            if(side == LightSide.left)
            {
                _leftTrigger.ActivateLight(_triggerActiveTimeBeforeMiss);
            } else if(side == LightSide.right)
            {
                _rightTrigger.ActivateLight(_triggerActiveTimeBeforeMiss);
            }
        }

        private void UpdateSpawnTimer()
        {
            if(!_canSpawn)
            {
                _spawnTimer += Time.deltaTime;
                if(_spawnTimer >= _spawnInterval)
                {
                    _canSpawn = true;
                }
            }
        }

        public void ReturnLightToPool(KanteleHeroLight light)
        {
            if(!_kanteleLights.ReturnObject(light))
            {
                Debug.LogError("Could not return light back to the pool! ");
            }
        }

        private LightSide RandomSide()
        {
            if(Random.Range(0f,2f) > 1f)
            {
                return LightSide.right;
            }
            return LightSide.left;
        }

        private LightSide SpawnOnOtherSide(LightSide side)
        {
            LightSide result = LightSide.right;
            if(side == LightSide.right)
            {
                result = LightSide.left;
            }
            return result;
        }

        public void LeftTriggerPressed()
        {
            _leftTrigger.TriggerPressed();
        }

        public void RightTriggerPressed ()
        {
            _rightTrigger.TriggerPressed();
        }

        public void LightMissed()
        {
            if(_missLights.Length > _misses)
            {
                _missLights[_misses].SetActive(true);
            }
            _misses++;
        }
        
        private void ActivateAllMissLights()
        {
            foreach(GameObject obj in _missLights)
            {
                obj.SetActive(true);
            }
        }

        private void DeactivateAllMissLights()
        {
            foreach(GameObject obj in _missLights)
            {
                obj.SetActive(false);
            }
        }

        public void ActivatePanel()
        {
            PanelActive = true;
            _misses = 0;
        }

        public void DeactivatePanel()
        {
            _misses = 0;
            DeactivateAllMissLights();
            _leftTrigger.DeactivateLight();
            _rightTrigger.DeactivateLight();
            _kanteleLights.DeactivateAllObjects();
            PanelActive = false;
        }
    }
}
