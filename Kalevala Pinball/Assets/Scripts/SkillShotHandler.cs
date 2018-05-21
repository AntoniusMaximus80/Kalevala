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

        private SkillShotGate[] _gates;

        private void Awake()
        {
            _gates = FindObjectsOfType<SkillShotGate>();
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
                    if(GameManager.Instance.GameMode.State != GameModeStateType.Sampo)
                    {
                        Launcher.Instance.SkillShotSuccesful = true;
                        Scorekeeper.Instance.AddScore(Scorekeeper.ScoreType.Skillshot);
                    }
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
            if(_gates.Length > 0)
            {
                foreach(SkillShotGate gate in _gates)
                {
                    gate.CloseGate();
                }
            }
            _skillshotPath.SetActive(true);
        }

    }
}
