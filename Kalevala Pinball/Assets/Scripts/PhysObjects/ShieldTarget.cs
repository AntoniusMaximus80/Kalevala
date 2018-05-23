using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala {
    public class ShieldTarget: MonoBehaviour
    {
        [SerializeField]
        private GameObject _myShield;

        private void OnCollisionEnter( Collision collision )
        {
            if(collision.gameObject.GetComponent<Pinball>() != null)
            {
                Scorekeeper.Instance.AddScore(Scorekeeper.ScoreType.ShieldTarget);
                StartCoroutine(Rotate(Vector3.left * 20, 0.25f));
            }
        }

        private IEnumerator Rotate( Vector3 target, float duration )
        {
            bool leaningBack = true;
            float ratio = 0f;
            float startTime = Time.time;
            Quaternion startRotation = _myShield.transform.localRotation;
            Quaternion targetRotation = Quaternion.Euler(target);
            
            while(ratio < 1.1f)
            {
                ratio = (Time.time - startTime) / duration;
                if(leaningBack && ratio < 1)
                {
                    _myShield.transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, ratio);
                }
                else if(!leaningBack)
                {
                    _myShield.transform.localRotation = Quaternion.Lerp(targetRotation, startRotation, ratio);
                }
                else if(leaningBack && ratio >= 1)
                {
                    startTime = Time.time;
                    ratio = 0;
                    leaningBack = false;
                }
                yield return 0;
            }

            _myShield.transform.localRotation = startRotation;
        }
    }
}
