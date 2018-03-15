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
        private float _timeToWait;
        private Pinball _ball;
        private RampEntrance _myEntrance;
        private float _startTime = -1;

        public float KickForce
        {
            get { return _kickoutForce; }
        }

        public bool BallIncoming (Pinball ball, RampEntrance myEntrance)
        {
            Debug.Log("Ball INC");
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
        void Update()
        {
            if(_ball != null && _startTime > 0)
            {
                if(Time.time - _startTime >= _timeToWait)
                {
                    KickOut();
                }
            }
        }

        private void BallInsideHole(bool exit)
        {

            _ball.IsInKickoutHole = true;
            _startTime = Time.time;
            _ball.ExitingRamp -= BallInsideHole;
            _ball.SetPhysicsEnabled(false);

        }

        private void KickOut()
        {
            _ball.IsInKickoutHole = false;
            Debug.Log("KikcOut");
            _ball.SetPhysicsEnabled(true);
            _startTime = -1;
            _ball = null;
        }
    }
}
