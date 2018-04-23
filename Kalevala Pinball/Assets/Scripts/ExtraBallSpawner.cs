using System.Collections;
using UnityEngine;

namespace Kalevala
{
    //[RequireComponent(typeof(KickoutHole))]
    public class ExtraBallSpawner : MonoBehaviour
    {
        [SerializeField, Tooltip("Transform giving the position, " +
            "you can use an empty GameObject linked to the kickout hole.")]
        private Transform _location;

        [SerializeField, Tooltip("You can give the ball a \"kick\" when " +
            "launching. Use Vector3.zero if not needed.")]
        private Vector3 _impulse;

        [SerializeField]
        private float _minSpawnInterval = 1f;

        private float _elapsedSpawnTime;

        private bool _active;
        private int _ballsLeftToSpawn;
        private Pinball _mostRecentExtraBall;

        private void Update()
        {
            UpdateSpawning();
        }

        private void UpdateSpawning()
        {
            if (_active)
            {
                if (_ballsLeftToSpawn > 0)
                {
                    _elapsedSpawnTime += Time.deltaTime;
                    bool waitTimeOver = _elapsedSpawnTime > _minSpawnInterval;

                    if (SpawnLocationIsAvailable() && waitTimeOver)
                    {
                        LaunchExtraBall();
                        _elapsedSpawnTime = 0;
                    }
                }
                else
                {
                    Deactivate();
                }
            }
        }

        public void Activate(int extraBallCount, bool addToQueue)
        {
            if (_active && !addToQueue)
            {
                Debug.LogWarning("Spawning extra balls is already active.");
            }

            _active = true;

            if (addToQueue)
            {
                _ballsLeftToSpawn += extraBallCount;
            }
            else
            {
                _ballsLeftToSpawn = extraBallCount;

                // Makes sure that one ball is spawned immediately
                _elapsedSpawnTime = _minSpawnInterval;
            }

            Debug.Log("Extra balls to spawn: " + extraBallCount);
        }

        public void Deactivate()
        {
            _active = false;
            _elapsedSpawnTime = 0f;
            _mostRecentExtraBall = null;
        }

        private bool SpawnLocationIsAvailable()
        {
            return _mostRecentExtraBall == null ||
                   _mostRecentExtraBall.PhysicsEnabled;
        }

        /// <summary>
        /// The method to launch extra balls.
        /// </summary>
        public void LaunchExtraBall()
        {
            if (_location == null)
            {
                Debug.LogError("No extra ball launch location set.");
                return;
            }

            // Use a deactivated ball if possible.
            Pinball ball = PinballManager.Instance.RecycleBall();

            // If not create a new ball from prefab.
            if (!ball)
            {
                ball = PinballManager.Instance.CreateNewBall();
            }
            // If it was found activate it.
            else
            {
                ball.gameObject.SetActive(true);
            }

            LaunchExtraBall(ball);

            //Debug.Log("Extra balls left to spawn: " + _ballsLeftToSpawn);
        }

        /// <summary>
        /// Launches a given ball as an extra ball.
        /// </summary>
        public void LaunchExtraBall(Pinball ball)
        {
            if (_location == null)
            {
                Debug.LogError("No extra ball launch location set.");
                return;
            }

            ball.gameObject.SetActive(true);

            // Update active ball counter and set ball location.
            PinballManager.Instance.AdjustActiveBallCounter(true);
            ball.transform.position = _location.position;

            // If given a valid impulse vector apply it.
            if (_impulse != null && !_impulse.Equals(Vector3.zero))
            {
                ball.AddImpulseForce(_impulse);
            }

            _ballsLeftToSpawn--;
            _mostRecentExtraBall = ball;
        }
    }
}
