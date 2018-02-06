using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class FlipperHingeJoint : MonoBehaviour
    {
        public enum FlipperOrientation
        {
            Left,
            Right
        };

        public FlipperOrientation _flipperOrientation;

        private HingeJoint _hingeJoint;
        private JointMotor _jointMotor;
        private JointSpring _jointSpring;

        public PinballManager _pinballManager;

        /*public class HingeExample : MonoBehaviour
        {
            void Start()
            {
                HingeJoint hinge = GetComponent<HingeJoint>();

                // Make the spring reach shoot for a 70 degree angle.
                // This could be used to fire off a catapult.

                JointSpring hingeSpring = hinge.spring;
                hingeSpring.spring = 10;
                hingeSpring.damper = 3;
                hingeSpring.targetPosition = 70;
                hinge.spring = hingeSpring;
                hinge.useSpring = true;
            }
        }*/

        // Use this for initialization
        void Start()
        {
            _hingeJoint = GetComponent<HingeJoint>();
            _jointSpring = _hingeJoint.spring;
            _jointMotor = _hingeJoint.motor;

            _hingeJoint.useMotor = false;
            _hingeJoint.useSpring = true;

            _jointMotor.force = _pinballManager._flipperMotorForce;
            _jointMotor.targetVelocity = _pinballManager._flipperMotorTargetVelocity;

            _jointSpring.spring = _pinballManager._springForce;

            //Debug.Log(_jointSpring.spring);
            //Debug.Log(GetComponent<HingeJoint>().spring.spring);
        }

        /*using UnityEngine;
        using System.Collections;

        public class ExampleClass : MonoBehaviour
        {
            void Start()
            {
                HingeJoint hinge = GetComponent<HingeJoint>();
                JointMotor motor = hinge.motor;
                motor.force = 100;
                motor.targetVelocity = 90;
                motor.freeSpin = false;
                hinge.motor = motor;
                hinge.useMotor = true;
            }
        }*/

        void FixedUpdate()
        {
            if (_flipperOrientation == FlipperOrientation.Left)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    _hingeJoint.useMotor = true;
                    _hingeJoint.useSpring = false;
                }
                else
                {
                    _hingeJoint.useMotor = false;
                    _hingeJoint.useSpring = true;
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.L))
                {
                    _hingeJoint.useMotor = true;
                    _hingeJoint.useSpring = false;
                }
                else
                {
                    _hingeJoint.useMotor = false;
                    _hingeJoint.useSpring = true;
                }
            }
        }
    }
}