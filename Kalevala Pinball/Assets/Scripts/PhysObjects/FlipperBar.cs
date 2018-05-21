using UnityEngine;

namespace Kalevala
{
    public class FlipperBar : MonoBehaviour
    {
        public enum FlipperOrientation
        {
            Left,
            Right
        };

        public FlipperOrientation _flipperBarOrientation;

        private HingeJoint _hingeJoint;
        private JointMotor _jointMotor;
        private JointSpring _jointSpring;
        private Rigidbody _rb;

        private PhysicMaterial _flipperMaterial;

        private PinballManager _pinballManager;

        private bool _flipperMaterialBouncinessChanged = false,
            _usingMotor = false,
            _usingSpring = false;

        public GameObject _booster;

        public Vector3 _boosterVector3;

        void Start()
        {
            MakeMaterial();
            _pinballManager = FindObjectOfType<PinballManager>();
            _rb = GetComponent<Rigidbody>();
            _hingeJoint = GetComponent<HingeJoint>();

            // Trying to improve physics by overriding the defaults.
            _rb.maxAngularVelocity = 0;
            _rb.maxDepenetrationVelocity = _rb.maxDepenetrationVelocity * 5;
            _rb.solverIterations = 30;
            _rb.solverVelocityIterations = 5;

            _jointSpring = _hingeJoint.spring;
            _jointMotor = _hingeJoint.motor;

            _hingeJoint.useMotor = false;
            _hingeJoint.useSpring = true;

            _jointMotor.force = _pinballManager._flipperMotorForce;
            _jointMotor.targetVelocity = _pinballManager._flipperMotorTargetVelocity;

            _jointSpring.spring = _pinballManager._springForce;
        }

        private void FixedUpdate()
        {
            if(((_rb.angularVelocity.y > 2 && FlipperOrientation.Right == _flipperBarOrientation) ||
                (_rb.angularVelocity.y < -2 && _flipperBarOrientation == FlipperOrientation.Left)) &&
                !_flipperMaterialBouncinessChanged)
            {
                _flipperMaterial.bounciness = 0.6f;
                _flipperMaterialBouncinessChanged = true;
            } else if (_rb.angularVelocity.y < 2 && _rb.angularVelocity.y > -2f && _flipperMaterialBouncinessChanged)
            {
                _flipperMaterialBouncinessChanged = false;
                _flipperMaterial.bounciness = 0.2f;
            }
        }

        public void UseMotor()
        {
            _hingeJoint.useMotor = true;
            _hingeJoint.useSpring = false;

            if(!_usingMotor)
            {
                float randomPitch = Random.Range(0.95f, 1.05f);
                SFXPlayer.Instance.Play(Sound.FlipperBarUp, randomPitch);
            }
            _usingMotor = true;
            _usingSpring = false;
        }

        public void UseSpring()
        {
            _hingeJoint.useMotor = false;
            _hingeJoint.useSpring = true;
            
            if(!_usingSpring)
            {
                float randomPitch = Random.Range(0.95f, 1.05f);
                SFXPlayer.Instance.Play(Sound.FlipperBarDown, randomPitch);
            }
            _usingMotor = false;
            _usingSpring = true;
        }

        public bool IsReset
        {
            get
            {
                return Mathf.Abs(_hingeJoint.angle) < 5f;
            }
        }

        private void MakeMaterial()
        {
            _flipperMaterial = new PhysicMaterial("Flipper");
            _flipperMaterial.dynamicFriction = 0.2f;
            _flipperMaterial.staticFriction = 0.2f;
            _flipperMaterial.frictionCombine = PhysicMaterialCombine.Average;
            _flipperMaterial.bounceCombine = PhysicMaterialCombine.Maximum;
            _flipperMaterial.bounciness = 0.2f;
            GetComponent<Collider>().material = _flipperMaterial;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_booster.transform.position, 4f);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_hingeJoint.useMotor &&
               ((_rb.angularVelocity.y > 2 && FlipperOrientation.Right == _flipperBarOrientation) ||
               (_rb.angularVelocity.y < -2 && _flipperBarOrientation == FlipperOrientation.Left)) )
            {
                //collision.gameObject.GetComponent<Pinball>().StopMotion();
                float distanceBetweenPinballAndBooster = Vector3.Distance(collision.transform.position, _booster.transform.position);
                float _boosterVectorMultiplier = Mathf.Clamp(4f - distanceBetweenPinballAndBooster, 0f, 4f);
                collision.gameObject.GetComponent<Rigidbody>().AddForce(_boosterVector3 * _boosterVectorMultiplier, ForceMode.Impulse);
            }
        }
    }
}