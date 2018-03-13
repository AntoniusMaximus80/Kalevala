using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class SampoLayer : MonoBehaviour
    {
        public bool _rotating;
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
                transform.Rotate(new Vector3(0f, 0f, _rotationZ));
            }
        }
    }
}