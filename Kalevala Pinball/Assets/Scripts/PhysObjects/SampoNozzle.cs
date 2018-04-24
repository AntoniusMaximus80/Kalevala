using UnityEngine;

namespace Kalevala
{
    public class SampoNozzle : MonoBehaviour
    {
        public bool _generate;
        public float _launchForceMultiplier,
            _launchForceVariance,
            _randomDirectionMultiplier;
        public Sampo _sampo;
        private SampoProduct _sampoProduct;
        public SampoProductType _sampoProductType;
        public GameObject _sampoMiddle;
        public int _spawnInterval;
        private int _spawnIntervalCountdown;

        private void Start()
        {
            _spawnIntervalCountdown = _spawnInterval;
        }

        void Update()
        {
            if (_generate)
            {
                _spawnIntervalCountdown--;
                if (_spawnIntervalCountdown == 0) {
                    _spawnIntervalCountdown = _spawnInterval;
                    SpawnSampoProduct(_sampoProductType);
                }
            }
        }

        public void SpawnSampoProduct(SampoProductType sampoProduct)
        {
            switch (sampoProduct)
            {
                case SampoProductType.Grain:
                    _sampoProduct = _sampo._grainPool.GetPooledObject();
                    break;
                case SampoProductType.Salt:
                    _sampoProduct = _sampo._saltPool.GetPooledObject();
                    break;
                case SampoProductType.Gold:
                    _sampoProduct = _sampo._goldPool.GetPooledObject();
                    break;
            }

            if (_sampoProduct != null) {

                _sampoProduct.Init(sampoProduct, _sampo, transform.position);

                #region Scaling
                // Scale the sampo product randomly, to make them look more unique.
                float productScale = 1.5f;

                if (_sampoProductType == SampoProductType.Grain)
                {
                    productScale = Random.Range(1.5f, 3f);
                }

                if (_sampoProductType == SampoProductType.Salt)
                {
                    productScale = Random.Range(1.5f, 3f);
                }

                _sampoProduct.transform.localScale = new Vector3(productScale, productScale, productScale);
                #endregion

                // Calculate the direction toward which the sampo product should be launched towards.
                Vector3 launchDirection = (transform.position - _sampoMiddle.transform.position) +
                    new Vector3(Random.Range(-_randomDirectionMultiplier, _randomDirectionMultiplier), 0f, Random.Range(-_randomDirectionMultiplier, _randomDirectionMultiplier));
                launchDirection.y = 0f; // Null the vertical direction.
                launchDirection.Normalize();

                // Launch the sampo product along the launchDirection vector.
                _sampoProduct.GetComponent<Rigidbody>().AddForce(launchDirection * (_launchForceMultiplier + Random.Range(-_launchForceVariance, _launchForceVariance)), ForceMode.Impulse);
            }
        }
    }
}