using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class SkillShotGate: MonoBehaviour
    {
        [SerializeField]
        public HingeJoint _hingeJoint;

        private Collider _collider;

        public Collider Collider
        {
            get
            {
               if (_collider == null)
                {
                    Collider = _hingeJoint.gameObject.GetComponent<BoxCollider>();
                    
                }
                return _collider;
            }
            private set
            {
                _collider = value;
            }
           
        }

        private void Start()
        {
            Collider = _hingeJoint.gameObject.GetComponent<BoxCollider>();
            Collider.enabled = false;
            OpenGate();
        }

        public void CloseGate()
        {
            _hingeJoint.useMotor = true;
            _hingeJoint.useSpring = false;
            Collider.enabled = true;

        }

        public void OpenGate()
        {
            _hingeJoint.useSpring = true;
            _hingeJoint.useMotor = false;
            Collider.enabled = false;
        }

        private void OnTriggerEnter( Collider other )
        {
            CloseGate();
        }
    }
}
