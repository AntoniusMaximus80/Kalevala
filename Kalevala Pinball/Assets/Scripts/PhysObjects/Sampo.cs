using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class Sampo : MonoBehaviour
    {
        public ToyElevatorController _toyElevatorController;

        private void LowerElevator()
        {
            _toyElevatorController.LowerElevator();
        }
    }
}