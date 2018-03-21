using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class SkillShotHandler: MonoBehaviour
    {
        [SerializeField]
        private GameObject _skillshotPath;

        private bool _skillshotSuccesful;

        private void Awake()
        {
            PathDeactivate();
        }
        private void OnTriggerStay( Collider other )
        {
            Pinball ball = other.GetComponent<Pinball>();
            if(ball != null)
            {
                if(ball.GetComponent<Rigidbody>().velocity.z < 0 && !_skillshotSuccesful)
                {
                    _skillshotSuccesful = true;
                    PathActivate();
                }
            }
        }

        public void PathDeactivate()
        {
            _skillshotSuccesful = false;
            _skillshotPath.SetActive(false);
        }

        public void PathActivate()
        {
            _skillshotPath.SetActive(true);
        }

    }
}
