using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kalevala
{
    public class HeatMap : MonoBehaviour
    {
        private int _count;

        private int[,] _heatMap;

        [SerializeField]
        private Vector2 _bottomLeft;

        [SerializeField]
        private Vector2 _topRight;

        [SerializeField]
        private int _sizeX;

        [SerializeField]
        private int _sizeY;


        // Use this for initialization
        void Awake()
        {
            _heatMap = new int[_sizeX, _sizeY];
        }

        // Update is called once per frame
        void Update()
        {

            foreach (Pinball ball in PinballManager.Instance.Pinballs)
            {
                AddToMap(ball.transform.position);
            }

        }

        private void AddToMap(Vector3 position)
        {
            // Skip if the ball posiion is outside the area.
            if (position.x < _bottomLeft.x || position.x > _topRight.x) return;
            if (position.z < _bottomLeft.y || position.z > _topRight.y) return;

            _count++;

            int x = (int)((position.x - _bottomLeft.x) / (_topRight.x - _bottomLeft.x)) * _sizeX;
            int y = (int)((position.z - _bottomLeft.y) / (_topRight.y - _bottomLeft.y)) * _sizeY;

            _heatMap[x, y]++;

        }

        void OnDrawGizmos()
        {
            for (int x = 0; x < _sizeX; x++)
            {
                for (int y = 0; y < _sizeY; y++)
                {
                    DrawPoint(x, y);
                }
            }
        }

        void DrawPoint(int x, int y)
        {
            try
            {
                Vector3 pos = new Vector3(_bottomLeft.x + x * (_topRight.x - _bottomLeft.x) / _sizeX, 0, _bottomLeft.y + y * (_topRight.y - _bottomLeft.y) / _sizeY);
                Color c = new Color(_heatMap[x, y] * 10 / _count, 0, 0);

                Gizmos.color = c;
                Gizmos.DrawSphere(pos, .1f);
            }
            catch { Debug.Log(x + "," + y); }
        }
    }
}