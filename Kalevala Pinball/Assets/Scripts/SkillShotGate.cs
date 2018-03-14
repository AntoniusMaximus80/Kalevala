using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class SkillShotGate: MonoBehaviour
    {
        [SerializeField]
        public HingeJoint _hingeJoint;

        private void Start()
        {
            OpenGate();
        }

        private void CloseGate()
        {
            _hingeJoint.useMotor = true;
            _hingeJoint.useSpring = false;
        }

        public void OpenGate()
        {
            _hingeJoint.useSpring = true;
            _hingeJoint.useMotor = false;
        }

        private void OnTriggerEnter( Collider other )
        {
            CloseGate();
        }
    }
}
