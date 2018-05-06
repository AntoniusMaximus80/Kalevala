using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class Objective: MonoBehaviour
    {
        [SerializeField]
        private GameObject _myObjective;
        [SerializeField]
        private string _objectiveText;

        [SerializeField]
        private StrobeLight _light;

        public GameObject MyObjective
        {
            get { return _myObjective; }
        }

        public void ActivateObjective()
        {
            Debug.Log("Objective Started hit " + _objectiveText);
            if(_myObjective.GetComponent<DropTarget>() != null)
            {
                _myObjective.GetComponent<DropTarget>().ResetDropTarget();
            }
            //turn on lights of the objective
            if (_light != null) _light.Switch(true);
        }

        public void DeactivateObjective()
        {
            Debug.Log("objective cleared " + _objectiveText);
            //turn off the lights of the objective
            if (_light != null) _light.Switch(false);
        }
        
    }
}
