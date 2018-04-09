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

        /// <summary>
        /// Spawns light on the panel that start moving towards the end of the rail
        /// </summary>
        /// <param name="waypoints"> Waypoints for the correct side </param>
        /// <param name="side"> Decide which side the light moves on the panel </param>
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

        /// <summary>
        /// updates the spawntimer to know when the next light should be spawned
        /// </summary>
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

        /// <summary>
        /// Returns light to the pool after it has reached the end of the waypoints.
        /// </summary>
        /// <param name="light"> Light that should be placed back to the pool </param>
        public void ReturnLightToPool(KanteleHeroLight light)
        {
            if(!_kanteleLights.ReturnObject(light))
            {
                Debug.LogError("Could not return light back to the pool! ");
            }
        }

        /// <summary>
        /// Randomizes the side which the light should spawn
        /// </summary>
        /// <returns> Random side which the light moves </returns>
        private LightSide RandomSide()
        {
            if(Random.Range(0f,2f) > 1f)
            {
                return LightSide.right;
            }
            return LightSide.left;
        }

        /// <summary>
        /// If light has spawned before minimum light spawn time, 
        /// always spawn it to other side of the panel
        /// </summary>
        /// <param name="side"> Which side the light was spawned before </param>
        /// <returns> opposite side of the panel </returns>
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

        /// <summary>
        /// If player presses trigger when it's not active or fails to press the trigger
        /// before it's timer runs out. Activate next misslight
        /// </summary>
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

        /// <summary>
        /// Starts the Kantelehero panel minigame
        /// </summary>
        public void ActivatePanel()
        {
            PanelActive = true;
            _misses = 0;
        }

        /// <summary>
        /// Deactivates the kantelehero panel minigame and all lights on it
        /// also reset all variables for the next round
        /// </summary>
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
