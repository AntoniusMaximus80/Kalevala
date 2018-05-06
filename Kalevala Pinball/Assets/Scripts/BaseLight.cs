using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala {
    public class BaseLight : MonoBehaviour {

        private static float _min, _max;
        private int _scaledY;

        public void Awake()
        {
            // Get the range of coordinates for the strobes
            if (gameObject.transform.position.y < _min) _min = gameObject.transform.position.y;
            if (gameObject.transform.position.y > _max) _max = gameObject.transform.position.y;
        }

        public void Start()
        {
            // This should scale y to range 0-50. Since all objects have done awake, min and max are known.
            _scaledY = Mathf.CeilToInt((gameObject.transform.position.y - _min) * 50f) % 10;
        }

        protected bool Sweep()
        {
            return _scaledY % 10 == GameManager.TimeModulo;
        }

        protected Color SweptColor(Color baseColor)
        {
            return Sweep()?Color.Lerp(baseColor, Color.yellow, .9f):baseColor;
        }
    } }
