using UnityEngine;

namespace Kalevala
{
    public class ToyElevator : MonoBehaviour
    {
        public ToyElevatorController _toyElevatorController;

        private void ActivateToy()
        {
            _toyElevatorController.ActivateToy();
        }
    }
}