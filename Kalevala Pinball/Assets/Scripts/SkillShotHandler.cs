using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class SkillShotHandler: MonoBehaviour
    {
        [SerializeField]
        private GameObject _skillshotPath;

        private void Awake()
        {
            PathDeactivate();
        }
        private void OnTriggerEnter( Collider other )
        {
            Pinball ball = other.GetComponent<Pinball>();
            Debug.Log("Open the gates");
            if(ball != null)
            {
                _skillshotPath.SetActive(true);
            }
        }

        public void PathDeactivate()
        {
            _skillshotPath.SetActive(false);
        }

    }
}
