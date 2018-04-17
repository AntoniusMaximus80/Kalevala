using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kalevala
{
    public class KickoutHole: MonoBehaviour
    {
        [SerializeField]
        private float _kickoutForce;
        [SerializeField]
        protected float _timeToWait;
        [SerializeField]
        private Vector3 _kickDirection;
        [SerializeField]
        protected int gizmoLenght;
        protected Pinball _ball;
        protected RampEntrance _myEntrance;
        private float _startTime = -1;

        /// <summary>
        /// An event which is fired when a
        /// pinball enters the kickout hole.
        /// </summary>
        public event Action BallEntered;

        public float KickForce
        {
            get { return _kickoutForce; }
        }

        public Vector3 KickDirection
        {
            get { return _kickDirection; }
        }

        public bool BallIncoming (Pinball ball, RampEntrance myEntrance)
        {
            _myEntrance = myEntrance;
            if(_ball != null)
            {
                return false;
            }
            _ball = ball;
            _ball.ExitingRamp += BallInsideHole;
            return true;
        }

        // Update is called once per frame
        protected void Update()
        {
            if(_ball != null && _startTime > 0)
            {
                if(Time.time - _startTime >= _timeToWait)
                {
                    KickOut();
                }
                else
                {
                    KickOutUpdate();
                }
            }
        }

        protected virtual void BallInsideHole(bool exit)
        {
            _ball.IsInKickoutHole = true;
            _startTime = Time.time;
            _ball.ExitingRamp -= BallInsideHole;
            _ball.SetPhysicsEnabled(false);

            // Fires an event that the status panel manager can listen to
            if(BallEntered != null)
            {
                BallEntered();
            }
        }

        protected virtual void KickOut()
        {
            _ball.IsInKickoutHole = false;
            _ball.SetPhysicsEnabled(true);
            _startTime = -1;
            _ball = null;
        }

        protected virtual void KickOutUpdate() { }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Vector3 startpos = transform.position;
            Vector3 endpos = Vector3.zero;
            float gravity = 0;
            for(int i = 0; i < gizmoLenght; i++)
            {
                endpos = startpos + KickDirection + Vector3.forward * gravity;
                Gizmos.DrawLine(startpos, endpos);
                startpos = endpos;
                gravity -= 0.017f;
            }
        }

    }
}
