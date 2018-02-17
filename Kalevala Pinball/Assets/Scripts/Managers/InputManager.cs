﻿using System;
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

        [SerializeField]
        private float _nudgeStrength = 10;

        private static Vector3 _nudgeVector = Vector3.zero;

        public static Vector3 NudgeVector
        {
            get
            {
                return _nudgeVector;
            }

        }

        // Update is called once per frame
        void Update() {

            GameInput();
        }

        private void GameInput()
        {
            if (Input.GetAxis(_launch) > 0)
            {
                _launcher.PoweringUp();
            }
            else if (Input.GetButtonUp(_launch))
            {
                _launcher.Launch();
            }

            if (Input.GetButton(_leftFlipperHit))
            {
                _leftFlipper.UseMotor();
            }
            else
            {
                _leftFlipper.UseSpring();
            }

            if (Input.GetButton(_rightFlipperHit))
            {
                _rightFlipper.UseMotor();
            }
            else
            {
                _rightFlipper.UseSpring();
            }

            // Ugly nudge hack.
            _nudgeVector.x = 0;
            if (Input.GetButtonDown("NudgeLeft")) DoNudge(-1);
            if (Input.GetButtonDown("NudgeRight")) DoNudge(1);
            
        }

        private void DoNudge(int direction)
        {
            // TODO : This really needs a sound effect, maybe the camera shake Toni suggested?
            // Not sure as nudge is not really THAT powerful.
            _nudgeVector.x = direction * _nudgeStrength;
        }
    }
}
