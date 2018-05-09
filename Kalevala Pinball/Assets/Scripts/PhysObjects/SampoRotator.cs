using UnityEngine;

namespace Kalevala
{
    public class SampoRotator : MonoBehaviour
    {
        private enum AccelerateOrDecelerate
        {
            Accelerate,
            Decelerate
        }

        private AccelerateOrDecelerate _accelerateOrDecelerate;

        public bool _rotate,
            _rotateRight;

        [SerializeField, Tooltip("This is the target speed the object will accelerate towards.")]
        private float _targetSpeed;

        [SerializeField, Range(1f, float.PositiveInfinity), Tooltip("This multiplier determines how quickly the object will reach it's target speed.")]
        private float _accelerationMultiplier;

        private float _rotationSpeedZ = 0f;

        public Sampo _sampo;

        private void Start()
        {
            _accelerateOrDecelerate = AccelerateOrDecelerate.Accelerate;
        }

        // Update is called once per frame
        void Update()
        {
            if (_rotate)
            {
                if (_rotationSpeedZ != _targetSpeed) { // Only this condition is needed, because when _rotationSpeedZ reaches a negative value, _rotate is set to false.
                    if (_accelerateOrDecelerate == AccelerateOrDecelerate.Accelerate) {
                        _rotationSpeedZ += Time.deltaTime * _accelerationMultiplier;
                        if (_rotationSpeedZ > _targetSpeed)
                        {
                            _accelerateOrDecelerate = AccelerateOrDecelerate.Decelerate;
                            _rotationSpeedZ = _targetSpeed;
                            //_sampo.ChangeState(Sampo.SampoStateType.Generate);
                        }
                    } else
                    {
                        _rotationSpeedZ -= Time.deltaTime * _accelerationMultiplier;
                        if (_rotationSpeedZ < 0f)
                        {
                            _accelerateOrDecelerate = AccelerateOrDecelerate.Accelerate;
                            _rotationSpeedZ = 0f;
                            _rotate = false;
                            if(_sampo._sampoState != Sampo.SampoStateType.Start)
                            {
                                _sampo.ChangeState(Sampo.SampoStateType.Idle);
                            }
                        }
                    }
                }

                if (_rotateRight) {
                    transform.Rotate(new Vector3(0f, 0f, _rotationSpeedZ));
                } else
                {
                    transform.Rotate(new Vector3(0f, 0f, -_rotationSpeedZ));
                }
            }
        }

        public bool ActivateRotator()
        {
            if (!_rotate)
            {
                _rotate = true;
                return true;
            }

            return false;
        }

        public bool DeactivateRotator()
        {
            if (_rotate && _accelerateOrDecelerate == AccelerateOrDecelerate.Accelerate)
            {
                _accelerateOrDecelerate = AccelerateOrDecelerate.Decelerate;
                return true;
            }

            return false;
        }
    }
}