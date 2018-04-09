using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{

    public class KanteleHeroLight: MonoBehaviour
    {
        public GameObject[] Waypoints;
        private int _currentWaypointIndex;
        private bool _reachedEnd;

        private Vector3 _startPos;
        private float _speed = 0f;
        private float startTime;
        private float _waypointSpeed;
        private KanteleHeroPanel _parent;
        private LightSide _side;
        // Update is called once per frame

        void Update()
        {
            if(Waypoints != null)
            {
                _reachedEnd = MoveTowards(_currentWaypointIndex);
            }

            if(_reachedEnd)
            {
                _parent.TurnTriggetLightOn(_side);
                _reachedEnd = false;
                BackToPool(_parent);
            }
        }

        /// <summary>
        /// Initializes the moving light on the panel
        /// </summary>
        /// <param name="waypoints"> Waypoints that the light moves along</param>
        /// <param name="parent"> the panel that initializes the light</param>
        public void Init( GameObject[] waypoints, KanteleHeroPanel parent, float speed, LightSide side)
        {
            _speed = speed;
            _parent = parent;
            _currentWaypointIndex = 1;
            Waypoints = waypoints;
            transform.position = waypoints[0].transform.position;
            _startPos = transform.position;
            startTime = 0;
            _side = side;
            _waypointSpeed = Vector3.Distance(transform.position, Waypoints[_currentWaypointIndex].transform.position);
        }

        public void BackToPool(KanteleHeroPanel parent)
        {
            _speed = 0;
            _parent = null;
            _currentWaypointIndex = 0;
            Waypoints = null;
            _startPos = transform.position;
            startTime = 0;
            _waypointSpeed = 0;
            parent.ReturnLightToPool(this);
        }

        /// <summary>
        /// Moves along given waypoints, after reaching the end destroys or disables itself.
        /// </summary>
        /// <param name="index"> what waypoint index currently going towards </param>
        /// <returns>return true if reached the end of the waypoint array</returns>
        private bool MoveTowards(int index )
        {

            startTime += Time.deltaTime / _waypointSpeed * _speed;
            transform.position = Vector3.Lerp(_startPos, Waypoints[index].transform.position, startTime);
            float sqrMyPos = (transform.position - Waypoints[index].transform.position).sqrMagnitude;
            if(sqrMyPos < 0.01f)
            {
                
                if(index + 1 < Waypoints.Length)
                {
                    _currentWaypointIndex++;
                    startTime = 0f;
                    _startPos = transform.position;
                    _waypointSpeed = Vector3.Distance(transform.position, Waypoints[_currentWaypointIndex].transform.position);
                } else
                {
                    return true;
                }
            }
            return false;
        }
    }
}