using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class HauenLeukaKantele: MonoBehaviour
    {
        private KanteleBumper[] _bumpers;
        public GameObject _door;

        public Transform DoorUp, DoorDown;
        // Use this for initialization
        void Start()
        {
            _bumpers = GetComponentsInChildren<KanteleBumper>();
            DeactivateBumpers();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ActivateKantele()
        {
            _door.transform.rotation = DoorUp.rotation;
            ActivateBumpers();
        }

        public void DeactivateKantele ()
        {
            _door.transform.rotation = DoorDown.rotation;
            DeactivateBumpers();
        }

        private void ActivateBumpers()
        {
            foreach(KanteleBumper bumper in _bumpers)
            {
                bumper.ActivateColliders();
            }
        }

        private void DeactivateBumpers()
        {
            foreach(KanteleBumper bumper in _bumpers)
            {
                bumper.DeactivateColliders();
            }
        }

    }
}
