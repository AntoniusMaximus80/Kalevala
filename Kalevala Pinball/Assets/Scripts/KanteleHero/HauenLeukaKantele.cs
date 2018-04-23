using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class HauenLeukaKantele: MonoBehaviour
    {
        private KanteleBumper[] _bumpers;
        public GameObject _door;
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
           StartCoroutine(Rotate(new Vector3(-23f, 0f, 0f),1f));
            ActivateBumpers();
        }

        public void DeactivateKantele ()
        {
            StartCoroutine(Rotate(new Vector3(132f, 0f, 0f), 1f));
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

        private IEnumerator Rotate(Vector3 target, float duration)
        {
            float ratio = 0f;
            float startTime = Time.time;
            Quaternion startRotation = _door.transform.localRotation;
            Quaternion targetRotation = Quaternion.Euler(target);
            while(ratio < 1)
            {
                ratio = (Time.time - startTime) / duration;
                _door.transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, ratio);
                yield return 0;
            }
            _door.transform.localRotation = targetRotation;
        }

    }
}
