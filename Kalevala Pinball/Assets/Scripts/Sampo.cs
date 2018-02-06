using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class Sampo : MonoBehaviour
    {
        public enum SampoProduct
        {
            grain,
            salt,
            coin
        }

        public bool _sampoProducing;
        public GameObject _instancingPoint,
            _grainPrefab,
            _saltPrefab,
            _goldCoinPrefab,
            _silverCoinPrefab,
            _crank,
            _burlapSackParticleSystemGrain,
            _burlapSackParticleSystemSalt;
        public float _crankRotationMultiplier;
        private float _crankRotationY;
        public SampoProduct _sampoProduct;

        // Use this for initialization
        void Start()
        {
            _crankRotationY = _crank.transform.localRotation.y;
        }

        // Update is called once per frame
        void Update()
        {
            if (_sampoProducing)
            {
                if (_sampoProduct == SampoProduct.grain)
                {
                    InstantiateProduct(_grainPrefab);
                    _burlapSackParticleSystemGrain.SetActive(true);
                }
                if (_sampoProduct == SampoProduct.salt)
                {
                    InstantiateProduct(_saltPrefab);
                    _burlapSackParticleSystemSalt.SetActive(true);
                }
                if (_sampoProduct == SampoProduct.coin)
                {
                    int randomType = Random.Range(0, 100);
                    if (randomType > 25)
                    {
                        InstantiateProduct(_goldCoinPrefab);
                    }
                    else
                    {
                        InstantiateProduct(_silverCoinPrefab);
                    }
                    _burlapSackParticleSystemGrain.SetActive(false);
                    _burlapSackParticleSystemSalt.SetActive(false);
                }
            } else
            {
                _burlapSackParticleSystemGrain.SetActive(false);
                _burlapSackParticleSystemSalt.SetActive(false);
            }
        }

        /*float x;
        void Update()
        {

            x += Time.deltaTime * 10;
            transform.rotation = Quaternion.Euler(x, 0, 0);

        }*/

        public void InstantiateProduct(GameObject _prefab)
        {
            int randomProductAmount = 0;

            if (_sampoProduct == SampoProduct.grain || _sampoProduct == SampoProduct.salt)
            {
                randomProductAmount = Random.Range(8, 12);
            } else
            {
                randomProductAmount = Random.Range(2, 4);
            }

            for (int i = 0; i < randomProductAmount; i++)
            {
                GameObject newProduct = Instantiate(_prefab,
                    _instancingPoint.transform.position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f)),
                    Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)),
                    _instancingPoint.transform);
                float productScale = 1f;
                if (_sampoProduct == SampoProduct.grain)
                {
                    productScale = Random.Range(0.5f, 0.75f);
                }
                if (_sampoProduct == SampoProduct.salt)
                {
                    productScale = Random.Range(0.5f, 1f);
                }
                newProduct.transform.localScale = new Vector3(productScale, productScale, productScale);
                newProduct.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-8f, 8f), Random.Range(-8f, 8f), 0f), ForceMode.Impulse);
            }

            _crankRotationY += _crankRotationMultiplier * Time.deltaTime;
            _crank.transform.localRotation = Quaternion.Euler(-90f, _crankRotationY, 0f);
        }
    }
}