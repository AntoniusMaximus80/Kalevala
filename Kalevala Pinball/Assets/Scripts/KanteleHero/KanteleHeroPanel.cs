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
        [SerializeField]
        private HauenLeukaKantele _haukiKantele;

        private List<Note> _notes = new List<Note>();
        private int _misses;
        private Pool<KanteleHeroLight> _kanteleLights;
        private float _spawnTimer;
        private bool _canSpawn;
        private LightSide _spawnSide;
        private bool _panelActive = false;
        private int _currentNote = 0;
        private int _noteCount;
        private float _difficulty;

        public bool PanelActive
        {
            get { return _panelActive; }
            private set { _panelActive = value;}
        }
        // Use this for initialization
        void Start()
        {
            for(int i = 0; i < 10; i++)
            {
                if(i % 2 == 0)
                {
                    _notes.Add(new Note(i, 0.5f));
                }
            }
            _kanteleLights = new Pool<KanteleHeroLight>(4, true, _movingLightPrefab);
            _leftTrigger.Init(this);
            _rightTrigger.Init(this);
            DeactivateAllMissLights();
            _spawnTimer = 0;
            _difficulty = 1;
        }

        /// <summary>
        /// Spawns light on the panel that start moving towards the end of the rail
        /// </summary>
        /// <param name="waypoints"> Waypoints for the correct side </param>
        /// <param name="side"> Decide which side the light moves on the panel </param>
        private void SpawnLight(GameObject[] waypoints, LightSide side, int noteNumber)
        {
            KanteleHeroLight light = _kanteleLights.GetPooledObject();
            if(light != null)
            {
                light.Init(waypoints, this, _lightMoveSpeed,side, noteNumber);
            }
        }

        // Update is called once per frame
        void Update()
        {
            
            if(PanelActive)
            {
                Debug.Log("Current index: " + _currentNote + " noteCount " + _noteCount);
                if(_currentNote < _noteCount)
                {
                    UpdateSpawnTimer();
                }

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
                        SpawnLight(_LeftWaypoints, _spawnSide,_currentNote - 1);
                    } else if (_spawnSide == LightSide.right)
                    {
                        SpawnLight(_rightWaypoints, _spawnSide, _currentNote - 1);
                    }
                    _canSpawn = false;
                    
                }
            }
            if(_misses >= _missLights.Length)
            {
                _misses = 0;
                DeactivatePanel();
            }
        }

        /// <summary>
        /// Turns the left light on and 
        /// </summary>
        public void TurnTriggetLightOn(LightSide side, int noteNumber)
        {
            if(side == LightSide.left)
            {
                _leftTrigger.ActivateLight(_triggerActiveTimeBeforeMiss, noteNumber);
            } else if(side == LightSide.right)
            {
                _rightTrigger.ActivateLight(_triggerActiveTimeBeforeMiss, noteNumber);
            }
        }

        /// <summary>
        /// updates the spawntimer to know when the next light should be spawned
        /// </summary>
        private void UpdateSpawnTimer()
        {
            _spawnTimer += Time.deltaTime * _difficulty;
            if(!_canSpawn && _currentNote < _noteCount)
            {
                if(_spawnTimer >= _notes[_currentNote].SpawnTime)
                {
                    _currentNote++;
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
        public void LightMissed(int noteNumber)
        {
            if(_missLights.Length > _misses)
            {
                _missLights[_misses].SetActive(true);
            }
            _misses++;
            if(noteNumber >= _noteCount - 1)
            {
                DeactivatePanel();
            }
        }

        public void LightHit(int noteNumber)
        {
            float pitch = _notes[noteNumber].Pitch;
            // TODO: play sound with pitch taken from given note.
            _haukiKantele.GetRotation();
            if(noteNumber >= _noteCount - 1)
            {
                DeactivatePanel();
            }
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

        private void CheckMissLights()
        {
            for(int i = 0; i < _misses; i++)
            {
                _missLights[i].SetActive(true);
            }
        }

        /// <summary>
        /// Starts the Kantelehero panel minigame
        /// </summary>
        public void ActivatePanel()
        {
            PanelActive = true;
            _spawnTimer = 0;
            _currentNote = 0;
            _noteCount = _notes.Count;
            CheckMissLights();
        }

        /// <summary>
        /// Deactivates the kantelehero panel minigame and all lights on it
        /// also reset all variables for the next round
        /// </summary>
        public void DeactivatePanel()
        {
            DeactivateAllMissLights();
            _leftTrigger.DeactivateLight();
            _rightTrigger.DeactivateLight();
            _kanteleLights.DeactivateAllObjects();
            PanelActive = false;
        }
    }
}
