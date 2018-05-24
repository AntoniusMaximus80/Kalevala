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
        [SerializeField]
        private RampEntrance _kanteleHeroRamp;
        public Material LeftLightMaterial;
        public Material RightLightMaterial;

        private List<Note> _notes = new List<Note>();
        private int _misses;
        private Pool<KanteleHeroLight> _kanteleLights;
        private float _spawnTimer;
        private bool _canSpawn;
        private LightSide _spawnSide;
        private bool _panelActive = false;
        private int _currentNote = 0;
        private int _noteCount;
        [SerializeField]
        private float _difficulty = 2;
        private float _waitTimeElapsed;
        private BoxCollider _kanteleEntranceCollider;

        private CameraController _cameroController;

        public float CurrentDifficulty
        {
            get { return _difficulty; }
        }
        public bool PanelActive
        {
            get { return _panelActive; }
            private set { _panelActive = value;}
        }
        // Use this for initialization
        void Start()
        {
            _kanteleEntranceCollider = GetComponent<BoxCollider>();
            _kanteleHeroRamp.BallEnteredRamp += ActivateEntranceCollider;
            GameManager.Instance.StateManager.GameOver += ResetPanel;
            CreateNotes();
            _cameroController = FindObjectOfType<CameraController>();
            _kanteleLights = new Pool<KanteleHeroLight>(4, true, _movingLightPrefab);
            _leftTrigger.Init(this);
            _rightTrigger.Init(this);
            DeactivateAllMissLights();
            _spawnTimer = 0;
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
                if(side == LightSide.left)
                {
                    light.Init(waypoints, this, _lightMoveSpeed, side, noteNumber);
                    light.GetComponent<Renderer>().material = LeftLightMaterial;
                } else
                {
                    light.Init(waypoints, this, _lightMoveSpeed, side, noteNumber);
                    light.GetComponent<Renderer>().material = RightLightMaterial;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Debugging purposes only
            //if(Input.GetKeyDown(KeyCode.C))
            //{
            //    ActivatePanel();
                
            //}
            if(PanelActive)
            {
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
                _difficulty = 2f;
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
            if(_waitTimeElapsed <= 3f)
            {
                _waitTimeElapsed += Time.deltaTime;
                return;
            }
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
        public void LightMissed(int noteNumber, bool playErrorSound)
        {
            if(PanelActive)
            {
                if(playErrorSound)
            {
                SFXPlayer.Instance.Play(Sound.KanteleheroError);
            }
            if(_missLights.Length > _misses)
            {
                _missLights[_misses].SetActive(true);
            }
            
                _misses++;
            }

            ComboManager.Instance.EndCombo();
            if(noteNumber >= _noteCount - 1)
            {
                _difficulty += 0.5f;
                DeactivatePanel();
            }
        }

        public void LightExpired( int noteNumber )
        {
            ComboManager.Instance.EndCombo();
            if(noteNumber >= _noteCount - 1)
            {
                _difficulty += 0.5f;
                DeactivatePanel();
            }
        }

        public void LightHit(int noteNumber)
        {
            PlayNote(_notes[noteNumber].NotePitch);
            ComboManager.Instance.IncreaseMultiplier(this.gameObject);
            Scorekeeper.Instance.AddScore(Scorekeeper.ScoreType.KanteleLight);
            if(noteNumber >= _noteCount - 1)
            {
                _difficulty += 0.5f;
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

        private void PlayNote(NotePitch notePitch)
        {
            switch(notePitch) {
                case NotePitch.A:
                    SFXPlayer.Instance.Play(Sound.GuitarA);
                    break;
                case NotePitch.D:
                    SFXPlayer.Instance.Play(Sound.GuitarD);
                    break;
                case NotePitch.E:
                    SFXPlayer.Instance.Play(Sound.GuitarE);
                    break;
                case NotePitch.F:
                    SFXPlayer.Instance.Play(Sound.GuitarF);
                    break;
                case NotePitch.G:
                    SFXPlayer.Instance.Play(Sound.GuitarG);
                    break;
            }
        }

        /// <summary>
        /// Starts the Kantelehero panel minigame
        /// </summary>
        public void ActivatePanel()
        {
            if(GameManager.Instance.GameMode.State == GameModeStateType.Sampo)
            {
                _haukiKantele.OpenDoor();
                return;
            }
            MusicPlayer.Instance.Pause();
            _cameroController.MoveCurrentCamTo(CameraController.CameraPosition.Kantele, false);
            //Physics.gravity = new Vector3(25f, -98.1f, -25f);
            PanelActive = true;
            _spawnTimer = 0;
            _currentNote = 0;
            _noteCount = _notes.Count;
            CheckMissLights();
            _haukiKantele.ActivateKantele();
            _rightTrigger.ResetNoteNumber();
            _leftTrigger.ResetNoteNumber();
            _waitTimeElapsed = 0f;

            Viewscreen.StartKH();
    }

        /// <summary>
        /// Deactivates the kantelehero panel minigame and all lights on it
        /// also reset all variables for the next round
        /// </summary>
        public void DeactivatePanel()
        {
            MusicPlayer.Instance.Unpause();
            _cameroController.MoveCurrentCamTo(CameraController.CameraPosition.Playfield, false);
            DeactivateAllMissLights();
            _leftTrigger.DeactivateLight();
            _rightTrigger.DeactivateLight();
            _kanteleLights.DeactivateAllObjects();
            _haukiKantele.DeactivateKantele();
            //Physics.gravity = new Vector3(0f, -98.1f, -65f);
            PanelActive = false;

            Viewscreen.EndKH();
        }

        public void ResetPanel(bool arg)
        {
            DeactivatePanel();
            _misses = 0;
            _difficulty = 2f;
        }

        private void CreateNotes()
        {
            _notes.Add(new Note(1f, NotePitch.D));
            _notes.Add(new Note(2f, NotePitch.D));
            _notes.Add(new Note(3f, NotePitch.E));
            _notes.Add(new Note(4f, NotePitch.E));
            _notes.Add(new Note(5f, NotePitch.F));
            _notes.Add(new Note(6f, NotePitch.A));
            _notes.Add(new Note(7f, NotePitch.E));
            _notes.Add(new Note(9f, NotePitch.E));

            _notes.Add(new Note(11f, NotePitch.F));
            _notes.Add(new Note(12f, NotePitch.D));
            _notes.Add(new Note(13f, NotePitch.G));
            _notes.Add(new Note(14f, NotePitch.F));
            _notes.Add(new Note(15f, NotePitch.E));
            _notes.Add(new Note(16f, NotePitch.F));
            _notes.Add(new Note(17f, NotePitch.D));
            _notes.Add(new Note(19f, NotePitch.D));
        }

        private void OnTriggerEnter( Collider other )
        {
            if(!_panelActive)
            {
                ActivatePanel();
                _kanteleEntranceCollider.enabled = false;
            }
        }

        private void ActivateEntranceCollider()
        {
            _kanteleEntranceCollider.enabled = true;
        }
    }
}
