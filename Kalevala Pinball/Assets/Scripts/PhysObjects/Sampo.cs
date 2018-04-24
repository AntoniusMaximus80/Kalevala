using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class Sampo : MonoBehaviour
    {
        public enum SampoStateType
        {
            Start,
            Idle,
            Generate,
            End
        }

        public SampoStateType _sampoState;

        public SampoRotator _middle,
            _innerLayer,
            _outerLayer;

        public SampoNozzle _goldSampoNozzle,
            _saltSampoNozzle,
            _grainSampoNozzle;

        public ToyElevatorController _toyElevatorController;

        public SampoProduct _grainPrefab,
            _saltPrefab,
            _goldPrefab;

        public Pool<SampoProduct> _grainPool,
            _saltPool,
            _goldPool;

        private void Start()
        {
            _grainPool = new Pool<SampoProduct>(32, false, _grainPrefab);
            _saltPool = new Pool<SampoProduct>(32, false, _saltPrefab);
            _goldPool = new Pool<SampoProduct>(32, false, _goldPrefab);
        }

        private void Update()
        {
            if (_sampoState == SampoStateType.End) {
                _sampoState = SampoStateType.Start;
                _toyElevatorController.EndGameMode();
            }

            if (_sampoState == SampoStateType.Generate)
            {
                _goldSampoNozzle._generate = true;
                _saltSampoNozzle._generate = true;
                _grainSampoNozzle._generate = true;
            } else
            {
                _goldSampoNozzle._generate = false;
                _saltSampoNozzle._generate = false;
                _grainSampoNozzle._generate = false;
            }
        }

        public void ChangeState(SampoStateType sampoStateType)
        {
            switch (sampoStateType)
            {
                case SampoStateType.Start:
                    _sampoState = SampoStateType.Start;
                    break;

                case SampoStateType.Idle:
                    StopRotating();
                    _sampoState = SampoStateType.Idle;
                    break;

                case SampoStateType.Generate:
                    StartRotating();
                    _sampoState = SampoStateType.Generate;
                    break;

                default:
                    _sampoState = SampoStateType.Idle;
                    break;
            }
        }

        private void StartRotating()
        {
            _middle.ActivateRotator();
            _innerLayer.ActivateRotator();
            _outerLayer.ActivateRotator();
        }

        private void StopRotating()
        {
            _middle.DeactivateRotator();
            _innerLayer.DeactivateRotator();
            _outerLayer.DeactivateRotator();
        }

        private void LowerElevator()
        {
            _toyElevatorController.LowerElevator();
        }

        public void ReturnProductToPool(SampoProduct sampoProduct)
        {
            switch (sampoProduct.Type)
            {
                case SampoProductType.Grain:
                    _grainPool.ReturnObject(sampoProduct);
                    break;
                case SampoProductType.Salt:
                    _saltPool.ReturnObject(sampoProduct);
                    break;
                case SampoProductType.Gold:
                    _goldPool.ReturnObject(sampoProduct);
                    break;
            }
        }
    }
}