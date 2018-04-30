using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class ComboManager : MonoBehaviour
    {
        [SerializeField]
        private Shot[] _shots;

        [SerializeField]
        private float _comboTime = 5f;

        public Shot _prevActivatedShot;
        public float _elapsedComboTime;
        public int mult;

        public int ComboScoreMultiplier
        {
            get
            {
                int multiplier = 1;

                if (_prevActivatedShot != null &&
                    _prevActivatedShot.RepeatActivations > 1)
                {
                    multiplier = _prevActivatedShot.RepeatActivations;
                }

                return multiplier;
            }
        }

        private void Update()
        {
            CheckShots();

            if (_prevActivatedShot != null)
            {
                UpdateComboTime();
            }

            mult = ComboScoreMultiplier;
        }

        private void CheckShots()
        {
            foreach (Shot shot in _shots)
            {
                if (shot.CheckActivation())
                {
                    if (_prevActivatedShot == null)
                    {
                        _prevActivatedShot = shot;
                    }
                    else if (shot != _prevActivatedShot)
                    {
                        EndCombo();
                        _prevActivatedShot = shot;
                    }

                    shot.RepeatActivations++;
                    _elapsedComboTime = 0;
                }
            }
        }

        private void UpdateComboTime()
        {
            _elapsedComboTime += Time.deltaTime;
            if (_elapsedComboTime >= _comboTime)
            {
                EndCombo();
            }
        }

        public void EndCombo()
        {
            _prevActivatedShot.RepeatActivations = 0;
            _prevActivatedShot = null;
        }

        public void ResetCombos()
        {
            EndCombo();

            foreach (Shot shot in _shots)
            {
                shot.ResetShot();
            }
        }
    }
}
