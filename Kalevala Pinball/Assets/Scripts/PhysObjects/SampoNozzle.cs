using UnityEngine;

namespace Kalevala
{
    public class SampoNozzle : MonoBehaviour
    {
        public enum SampoProduct
        {
            grain,
            salt,
            gold
        }

        public bool _sampoProducing;
        public GameObject _grainPrefab,
            _saltPrefab,
            _goldCoinPrefab,
            _body;
        public float _launchForceMultiplier,
            _launchForceVariance,
            _randomDirectionMultiplier;
        public SampoProduct _sampoProduct;

        void Update()
        {
            if (_sampoProducing)
            {
                if (_sampoProduct == SampoProduct.grain)
                {
                    InstantiateProduct(_grainPrefab);
                }
                if (_sampoProduct == SampoProduct.salt)
                {
                    InstantiateProduct(_saltPrefab);
                }
                if (_sampoProduct == SampoProduct.gold)
                {
                    InstantiateProduct(_goldCoinPrefab);
                }
            }
        }

        public void InstantiateProduct(GameObject _prefab)
        {
            int randomProductAmount = 0;

            if (_sampoProduct == SampoProduct.grain || _sampoProduct == SampoProduct.salt)
            {
                randomProductAmount = Random.Range(2, 4);
            } else
            {
                randomProductAmount = 1;
            }

            for (int i = 0; i < randomProductAmount; i++)
            {
                // Instantiate a sampo product close to the _instancingPoint with a random rotation.
                GameObject newProduct = Instantiate(_prefab,
                    transform.position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f)),
                    Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));

                // Scale the sampo product randomly, to make them look more unique.
                float productScale = 1f;

                if (_sampoProduct == SampoProduct.grain)
                {
                    productScale = Random.Range(1f, 2f);
                }

                if (_sampoProduct == SampoProduct.salt)
                {
                    productScale = Random.Range(1f, 2f);
                }
                newProduct.transform.localScale = new Vector3(productScale, productScale, productScale);

                // Calculate the direction toward which the sampo product should be launched towards.
                Vector3 launchDirection = (transform.position - _body.transform.position) +
                    new Vector3(Random.Range(-_randomDirectionMultiplier, _randomDirectionMultiplier), 0f, Random.Range(-_randomDirectionMultiplier, _randomDirectionMultiplier));
                launchDirection.y = 0f; // Null the vertical direction.
                launchDirection.Normalize();

                // Launch the sampo product along the launchDirection vector.
                newProduct.GetComponent<Rigidbody>().AddForce(launchDirection * (_launchForceMultiplier + Random.Range(-_launchForceVariance, _launchForceVariance)), ForceMode.Impulse);
            }
        }
    }
}