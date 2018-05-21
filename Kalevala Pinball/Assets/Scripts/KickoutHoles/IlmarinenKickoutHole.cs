using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class IlmarinenKickoutHole: KickoutHole
    {
        [SerializeField]
        private Animator _bellows;
        private bool _doOnce = true;
        private bool _hadResources = false;

        public int Resources
        {
            set;
            private get;
        }

        protected override void KickOutUpdate()
        {
            if(_doOnce)
            {
                if(PinballManager.Instance.Resources >= (int)PinballManager.Instance.MaxResources / 3 ||
                   GameManager.Instance.GameMode.State == GameModeStateType.Sampo ||
                   Launcher.Instance.SkillShotSuccesful)
                {
                    if(GameManager.Instance.GameMode.State != GameModeStateType.Sampo &&
                        !Launcher.Instance.SkillShotSuccesful)
                    {
                        PinballManager.Instance.ChangeResources((int)-PinballManager.Instance.MaxResources / 3);
                    } else if (Launcher.Instance.SkillShotSuccesful)
                    {
                        Launcher.Instance.SkillShotSuccesful = false;
                    }
                    _hadResources = true;
                    _bellows.SetBool("Pumping", true);
                    _ball.SetHeatBall(_timeToWait);
                    SFXPlayer.Instance.Play(Sound.IlmarisenKOH);
                } else
                {
                    _timeToWait = 0.2f;
                }
                Scorekeeper.Instance.AddScore(Scorekeeper.ScoreType.IlmarinenKOH);
                _doOnce = false;
            }
        }

        protected override void KickOut()
        {
            base.KickOut();
            if(_hadResources)
            {
                _hadResources = false;
                TriggerEvents();
            }
            _bellows.SetBool("Pumping", false);
            _doOnce = true;
            _timeToWait = 2f;
        }
    }
}
