using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kalevala
{
    public class HeatMap : MonoBehaviour
    {
        private Renderer _renderer;
        private Bounds _bounds;
        private int[,] _map;
        private Texture2D _texture;
        private int _max;

        private Vector2 _bottomLeft, _topRight;
        private int _sizeX, _sizeY;
        private float _timer;
        private Vector3 _oldPos;

        [SerializeField, Tooltip("Scaling factor for the heat map resolution in points per unit.")]
        private float _ScaleFactor = 5;

        [SerializeField, Tooltip("Sampling rate in seconds between samples."), Range(0, 1)]
        private float _SamplingRate = .05f;

        [SerializeField, Tooltip("Color, adjust until it looks right."), Range(0, 1)]
        private float _colorRate = .1f;

        [SerializeField, Tooltip("No point in drawing while playing, switch this on when you want to see results.")]
        private bool _showMap;
        private bool _showMapOld;

        // Use this for initialization
        void Start()
        {
            _renderer = gameObject.GetComponent<Renderer>();
            _bounds = _renderer.bounds;
            _sizeX = Mathf.FloorToInt(_bounds.size.x * _ScaleFactor);
            _sizeY = Mathf.FloorToInt(_bounds.size.z * _ScaleFactor);
            _map = new int[_sizeX, _sizeY];
            

            //Debug.Log("x " + _sizeX + " y " + _sizeY);

            _bottomLeft = new Vector2(_bounds.center.x - _bounds.extents.x, _bounds.center.z - _bounds.extents.z);
            _topRight = new Vector2(_bounds.center.x + _bounds.extents.x, _bounds.center.z + _bounds.extents.z);


        }

        // Update is called once per frame
        void Update()
        {
            _timer -= Time.deltaTime;

            if (_timer > 0) return;



            foreach (Pinball ball in PinballManager.Instance.Pinballs)
            {
                // Skip non-active balls that might be on the list.
                if (!ball.gameObject.activeSelf) continue;

                // Skip not moving balls.
                //if (ball.Speed == 0) continue;

                // Add a hit to the heat map.
                AddToMap(ball.transform.position);
            }

            if (_showMap != _showMapOld)
            {
                if (_showMap)
                {
                    ShowMap();
                }
                else
                {
                    HideMap();
                }


            }

            if (_showMap)
            {
                UpdateMap();
            }

            _timer = _SamplingRate;
        }

        private void HideMap()
        {
            _renderer.enabled = false;
            _showMapOld = false;
        }

        private void ShowMap()
        {

           

            
            _renderer.enabled = true;
            _showMapOld = true;

        }

        private void UpdateMap()
        {
            _texture = new Texture2D(_sizeX, _sizeY);

            for (int x = 0; x < _sizeX; x++)
            {

                for (int y = 0; y < _sizeY; y++)
                {

                    float value = Mathf.Clamp01(_colorRate * _map[x, y]);
                    float subValue = value * .2f;

                    Color color = new Color(value, subValue, subValue, subValue);

                    _texture.SetPixel(_sizeX - x, _sizeY - y, color);

                }

            }

            _renderer.material.mainTexture = _texture;
            _texture.Apply();
        }

        private void AddToMap(Vector3 position)
        {

            // Skip if the ball posiion is outside the area.
            if (position.x < _bottomLeft.x || position.x > _topRight.x) return;
            if (position.z < _bottomLeft.y || position.z > _topRight.y) return;

            // Calculate the map coordinates.
            int x = (int)((position.x - _bottomLeft.x) / (_topRight.x - _bottomLeft.x) * _sizeX);
            int y = (int)((position.z - _bottomLeft.y) / (_topRight.y - _bottomLeft.y) * _sizeY);

            // Add to the map.
            _map[x, y]++;

            // Keep track of max value.
            if (_map[x, y] > _max) _max = _map[x, y];

        }

        //private void OnDrawGizmos()
        //{
        //    if (!_showMap) return;

        //    Vector3 pos = new Vector3();
        //    pos.x = _bottomLeft.x;


        //    float step = 1 / _ScaleFactor;
        //    Gizmos.color = Color.red;

        //    for (int x = 0; x < _sizeX; x++)
        //    {
        //        pos.z = _bottomLeft.y;
        //        for (int y = 0; y < _sizeY; y++)
        //        {
        //            Gizmos.color = new Color(Mathf.Clamp01(_colorRate * _map[x, y]), 0, 0, .2f);

        //            Gizmos.DrawSphere(pos, step / 2);
        //            pos.z += step;
        //        }
        //        pos.x += step;
        //    }
        //}

    }
}