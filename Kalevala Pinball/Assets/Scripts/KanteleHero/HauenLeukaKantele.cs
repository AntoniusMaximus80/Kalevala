using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class HauenLeukaKantele: MonoBehaviour
    {
        [SerializeField]
        private float _rotationAmount;
        [SerializeField]
        private float _turnSpeed;

        private Vector3 _startRotation;
        private Vector3 _defaultRotation;
        private Vector3 _targetRotation;

        // Use this for initialization
        void Start()
        {
            _defaultRotation = transform.eulerAngles;
            GetRotation();
        }

        // Update is called once per frame
        void Update()
        {
            Rotate(_targetRotation);
            //Debug.Log("MyRotation " + transform.eulerAngles + " TargetRotation " + _targetRotation);
            if(Input.GetKeyDown(KeyCode.G))
            {
                GetRotation();
            }
            if(Input.GetKeyDown(KeyCode.M))
            {
                GetDefaultRotation();
            }
        }

        public void GetRotation()
        {
            _targetRotation = transform.eulerAngles + -Vector3.left * _rotationAmount;
            _startRotation = transform.eulerAngles;
        }
        
        public void GetDefaultRotation()
        {
            _targetRotation = new Vector3(_defaultRotation.x,transform.eulerAngles.y,transform.eulerAngles.z);
            _startRotation = transform.eulerAngles;
        }

        private void Rotate( Vector3 target )
        {
            _startRotation = new Vector3(
             Mathf.LerpAngle(_startRotation.x, _targetRotation.x, _turnSpeed * Time.deltaTime ),
             Mathf.LerpAngle(_startRotation.y, _targetRotation.y, _turnSpeed * Time.deltaTime),
             Mathf.LerpAngle(_startRotation.z, _targetRotation.z, _turnSpeed * Time.deltaTime));

            transform.eulerAngles = _startRotation;
        }
    }
}
