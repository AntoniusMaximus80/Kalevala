using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class CollectableSpawner : MonoBehaviour
    {
        private Collectable[] _collectables;

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

        public Collectable SpawnCollectable(Collectable.CollectableType type,
                                            bool allCollectables)
        {
            Collectable collectable = null;

            if (allCollectables)
            {
                foreach (Collectable coll in _collectables)
                {
                    InitCollectable(coll, type);
                }

                collectable = _collectables[0];
            }
            else
            {
                collectable = GetRandomItemFromPool();
                if (collectable != null)
                {
                    InitCollectable(collectable, type);
                }
                else
                {
                    Debug.Log("No collectables in pool.");
                }
            }

            return collectable;
        }

        private void InitCollectable(Collectable collectable,
                                     Collectable.CollectableType type)
        {
            collectable.ShowCollectableObject(false);
            collectable.SetCollectableType(type);
            collectable.ShowCollectableObject(true);
            collectable.gameObject.SetActive(true);
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
        }
    }
}
