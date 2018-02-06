using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala {
    public class InputManager: MonoBehaviour {

        private const string _launch = "Launch";
        private const string _leftFlipperHit = "LeftFlipper";
        private const string _rightFlipperHit = "RightFlipper";

        [SerializeField]
        private Launcher _launcher;

        [SerializeField]
        private FlipperHingeJoint _leftFlipper;

        [SerializeField]
        private FlipperHingeJoint _rightFlipper;

        // Update is called once per frame
        void Update() {

            GameInput();
        }

        private void GameInput()
        {
            if(Input.GetAxis(_launch) > 0)
            {
                _launcher.PoweringUp();
            }
            else if(Input.GetButtonUp(_launch))
            {
                _launcher.Launch();
            }
            if(Input.GetButtonDown(_leftFlipperHit))
            {
                _leftFlipper.UseMotor();
            }
            else if(Input.GetButtonUp(_leftFlipperHit))
            {
                _leftFlipper.UseSpring();
            }
            if(Input.GetButtonDown(_rightFlipperHit))
            {
                _rightFlipper.UseMotor();
            }
            else if(Input.GetButtonUp(_rightFlipperHit))
            {
                _rightFlipper.UseSpring();
            }
        }
    }
}
