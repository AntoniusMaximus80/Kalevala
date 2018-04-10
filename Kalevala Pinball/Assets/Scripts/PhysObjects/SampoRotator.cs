using UnityEngine;

namespace Kalevala
{
    public class SampoRotator : MonoBehaviour
    {
        public bool _rotating,
            _right;
        public float _rotationMultiplier;
        private float _rotationZ = 0;

        // Use this for initialization
        void Start()
        {
            _rotationZ = _rotationMultiplier * Time.deltaTime;
        }

        // Update is called once per frame
        void Update()
        {
            if (_rotating)
            {
                if (_right) {
                    transform.Rotate(new Vector3(0f, 0f, _rotationZ));
                } else
                {
                    transform.Rotate(new Vector3(0f, 0f, -_rotationZ));
                }
            }
        }
    }
}