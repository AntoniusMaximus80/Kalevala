using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Kalevala
{
    public class CollectableSpawner : MonoBehaviour
    {
        [SerializeField]
        private bool _launchToPosition = true;

        [SerializeField]
        private float _launchSpeed = 20f;

        //[SerializeField]
        //private SampoSpinner _sampoSpinner;

        [SerializeField]
        private int _defaultGrainChance = 5;

        [SerializeField]
        private int _defaultSaltChance = 3;

        [SerializeField]
        private int _defaultGoldChance = 1;

        private Collectable[] _collectables;

        private bool _spawnMultiple;
        private bool _randomProduct;
        private SampoProductType _collType;
        private float _spawnInterval;
        private float _elapsedTime;
        private int _leftToSpawn;

        private int _grainChance;
        private int _saltChance;
        private int _goldChance;

        private Action EndGenerating;

        private int TotalChances
        {
            get
            {
                return _grainChance + _saltChance + _goldChance;
            }
        }

        private void Start()
        {
            // All of the collectables are already
            // in the world in specific positions
            _collectables = FindObjectsOfType<Collectable>();

            foreach (Collectable collectable in _collectables)
            {
                collectable.SetHandler(this);
                ReturnItemToPool(collectable);
            }

            // Debugging
            //_sampoSpinner.HalfTurn += OnSampoSpinnerHalfTurn;

            ResetChances();
        }

        private void Update()
        {
            UpdateSpawningCollectables();
        }

        private void UpdateSpawningCollectables()
        {
            if (_spawnMultiple)
            {
                if (_leftToSpawn > 0)
                {
                    _elapsedTime += Time.deltaTime;

                    if (_elapsedTime > _spawnInterval)
                    {
                        _elapsedTime = 0;
                        Collectable collectable = SpawnCollectable(_collType);

                        if (collectable != null)
                        {
                            _leftToSpawn--;
                        }
                        else
                        {
                            // Can't spawn more collectables, ends the spawning
                            ResetSpawning();
                            return;
                        }
                    }
                }
                else
                {
                    ResetSpawning();
                }
            }
        }

        public void SpawnCollectables(SampoProductType type,
            int amount, float interval, bool launchToPosition)
        {
            // True: the collectable flies to its position
            // False: the collectable just appears at its position
            _launchToPosition = launchToPosition;

            _randomProduct = false;

            SpawnCollectables(type, amount, interval);
        }

        public void SpawnCollectables(int amount, float interval, Action endGeneratingCallBack, Vector3 position)
        {
            transform.position = position;
            _randomProduct = true;
            EndGenerating = endGeneratingCallBack;
            SpawnCollectables(SampoProductType.None, amount, interval);
        }

        public void SpawnCollectables(SampoProductType type,
            int amount, float interval)
        {
            _spawnMultiple = true;
            _collType = type;
            _leftToSpawn = amount;
            _spawnInterval = interval;

            // Ensures that one collectable is spawned immediately
            _elapsedTime = interval;
        }

        public Collectable SpawnCollectable(SampoProductType type,
                                            bool launchToPosition)
        {
            // True: the collectable flies to its position
            // False: the collectable just appears at its position
            _launchToPosition = launchToPosition;

            return SpawnCollectable(type);
        }

        public Collectable SpawnCollectable(SampoProductType type)
        {
            Collectable collectable = GetRandomItemFromPool();
            if (collectable != null)
            {
                if (_randomProduct)
                {
                    type = GetRandomProductType();
                }

                InitCollectable(collectable, type);
            }
            else
            {
                Debug.LogWarning("No collectables in pool.");
            }

            return collectable;
        }

        public Collectable SpawnAllCollectables(SampoProductType type)
        {
            foreach (Collectable collectable in _collectables)
            {
                InitCollectable(collectable, type);
            }

            return _collectables[0];
        }

        private void InitCollectable(Collectable collectable,
                                     SampoProductType type)
        {
            if (type == SampoProductType.None)
            {
                Debug.LogError
                    ("Trying to initialize a collectable with type None.");
                return;
            }

            collectable.ShowCollectableObject(false);
            collectable.Init(type);
            collectable.ShowCollectableObject(true);
            collectable.gameObject.SetActive(true);

            if (_launchToPosition)
            {
                // Launches the collectable from the spawner's position
                // to the collectable's own target position
                collectable.LaunchToPosition
                    (transform.position, _launchSpeed);
            }
        }

        public Collectable GetRandomItemFromPool()
        {
            Collectable result = null;

            List<Collectable> pooledCollectables = new List<Collectable>();

            foreach (Collectable item in _collectables)
            {
                if ( !item.gameObject.activeSelf )
                {
                    pooledCollectables.Add(item);
                }
            }

            if (pooledCollectables.Count > 1)
            {
                int randomIndex = UnityEngine.Random.Range(0, pooledCollectables.Count - 1);
                result = pooledCollectables[randomIndex];
            }
            else if (pooledCollectables.Count == 1)
            {
                result = pooledCollectables[0];
            }

            return result;
        }

        public void ReturnItemToPool(Collectable collectable)
        {
            collectable.gameObject.SetActive(false);
        }

        public void ResetCollectables()
        {
            foreach (Collectable collectable in _collectables)
            {
                collectable.Remove();
            }

            ResetSpawning();
        }

        private void ResetSpawning()
        {
            _collType = SampoProductType.None;
            _spawnMultiple = false;
            _randomProduct = false;
            _elapsedTime = 0;
            _leftToSpawn = 0;
            if(EndGenerating != null)
            {
                EndGenerating();
            }
        }
              
        private SampoProductType GetRandomProductType()
        {
            SampoProductType result;

            int typeNum = UnityEngine.Random.Range(0, TotalChances);

            if (typeNum < _grainChance)
            {
                result = SampoProductType.Grain;
            }
            else if (typeNum < _grainChance + _saltChance)
            {
                result = SampoProductType.Salt;
            }
            else
            {
                result = SampoProductType.Gold;
            }

            return result;
        }

        public bool ImproveValueChances()
        {
            bool result = false;

            if (_grainChance > _saltChance && _grainChance > 1)
            {
                _grainChance--;
                _saltChance++;
                result = true;
            }
            else if (_saltChance > 1)
            {
                _saltChance--;
                _goldChance++;
                result = true;
            }

            return result;
        }

        public void ResetChances()
        {
            _grainChance = _defaultGrainChance;
            _saltChance = _defaultSaltChance;
            _goldChance = _defaultGoldChance;
        }
    }
}
