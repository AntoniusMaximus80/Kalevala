using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class CollectableSpawner : MonoBehaviour
    {
        [SerializeField]
        private bool _launchToPosition = true;

        [SerializeField]
        private float _launchSpeed = 20f;

        private Collectable[] _collectables;

        private bool _spawnMultiple;
        private Collectable.CollectableType _collType;
        private float _spawnInterval;
        private float _elapsedTime;
        private int _leftToSpawn;

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

        public void SpawnCollectables(Collectable.CollectableType type,
            int amount, float interval, bool launchToPosition)
        {
            // True: the collectable flies to its position
            // False: the collectable just appears at its position
            _launchToPosition = launchToPosition;

            SpawnCollectables(type, amount, interval);
        }

        public void SpawnCollectables(Collectable.CollectableType type,
            int amount, float interval)
        {
            _spawnMultiple = true;
            _collType = type;
            _leftToSpawn = amount;
            _spawnInterval = interval;

            // Ensures that one collectable is spawned immediately
            _elapsedTime = interval;

            //StartCoroutine(SpawnCollectablesRoutine(type, amount, interval));
        }

        //public IEnumerator SpawnCollectablesRoutine(
        //    Collectable.CollectableType type, int amount, float interval)
        //{
        //    int spawnedAmount = 0;
        //    while (spawnedAmount < amount)
        //    {
        //        Collectable collectable = SpawnCollectable(type, false);

        //        if (collectable != null)
        //        {
        //            spawnedAmount++;

        //            // Waits the interval time
        //            yield return new WaitForSeconds(interval);
        //        }
        //        else
        //        {
        //            // Can't spawn more collectables, breaks
        //            break;
        //        }
        //    }
        //}

        public Collectable SpawnCollectable(Collectable.CollectableType type,
                                            bool launchToPosition)
        {
            // True: the collectable flies to its position
            // False: the collectable just appears at its position
            _launchToPosition = launchToPosition;

            return SpawnCollectable(type);
        }

        public Collectable SpawnCollectable(Collectable.CollectableType type)
        {
            Collectable collectable = GetRandomItemFromPool();
            if (collectable != null)
            {
                InitCollectable(collectable, type);
            }
            else
            {
                Debug.Log("No collectables in pool.");
            }

            return collectable;
        }

        public Collectable SpawnAllCollectables(Collectable.CollectableType type)
        {
            foreach (Collectable collectable in _collectables)
            {
                InitCollectable(collectable, type);
            }

            return _collectables[0];
        }

        private void InitCollectable(Collectable collectable,
                                     Collectable.CollectableType type)
        {
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
                int randomIndex = Random.Range(0, pooledCollectables.Count - 1);
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
            _collType = Collectable.CollectableType.None;
            _spawnMultiple = false;
            _elapsedTime = 0;
            _leftToSpawn = 0;
            //_launchToPosition = false;
        }
    }
}
