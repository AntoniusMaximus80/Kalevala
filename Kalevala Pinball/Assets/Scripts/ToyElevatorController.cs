using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class ToyElevatorController : MonoBehaviour
    {
        [SerializeField]
        private Animator _toyElevatorAnimator,
            _irisDoorAnimator;

        private bool _elevatorUp = false;

        private float _animationDuration = 4f,
            _animationCounter = 0f;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
        _animationCounter += Time.deltaTime;
            if (_animationCounter > _animationDuration) {
                _animationCounter = 0f;
                if (_elevatorUp)
                {
                    _elevatorUp = false;
                    LowerElevator();
                } else
                {
                    _elevatorUp = true;
                    RaiseElevator();
                }
            }
        }

        void RaiseElevator()
        {
            _toyElevatorAnimator.SetBool("Rise", true);
            _irisDoorAnimator.SetBool("Open", true);
        }

        void LowerElevator()
        {
            _toyElevatorAnimator.SetBool("Rise", false);
            _irisDoorAnimator.SetBool("Open", false);
        }
    }
}