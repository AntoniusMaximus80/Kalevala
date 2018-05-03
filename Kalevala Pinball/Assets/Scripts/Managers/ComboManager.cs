using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class ComboManager: MonoBehaviour
    {
        #region Statics
        private static ComboManager instance;

        /// <summary>
        /// Gets or sets the Singleton instance .
        /// </summary>
        public static ComboManager Instance
        {
            get
            {
                if(instance == null)
                {
                    // NOTE:
                    // There must be a Resources folder under Assets and
                    // ComboManager there for this to work. Not necessary if
                    // a ComboManager object is present in a scene from the
                    // get-go.

                    instance =
                        Instantiate(Resources.Load<ComboManager>("ComboManager"));
                }

                return instance;
            }
        }
        #endregion Statics

        [SerializeField]
        private Shot[] _shots;

        [SerializeField]
        private float _comboTime = 5f;

        public Shot _prevActivatedShot;
        public float _elapsedComboTime;
        public int mult;
        private GameObject _prevActivatedGameObject;

        private int _scoreMultiplier;
        public int ScoreMultiplier
        {
            get { return _scoreMultiplier; }
        }

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            } else
            {
                Destroy(this.gameObject);
            }
        }

        public void IncreaseMultiplier(GameObject obj, int value = 1)
        {
            if(obj != _prevActivatedGameObject)
            {
                _elapsedComboTime = 0f;
                _scoreMultiplier = 1;
                _prevActivatedGameObject = obj;
                return;
            }
            _elapsedComboTime = 0f;
            _scoreMultiplier += value;
        }

        //public int ComboScoreMultiplier
        //{
        //    get
        //    {
        //        int multiplier = 1;

        //        if (_prevActivatedShot != null &&
        //            _prevActivatedShot.RepeatActivations > 1)
        //        {
        //            multiplier = _prevActivatedShot.RepeatActivations;
        //        }

        //        return multiplier;
        //    }
        //}

        private void Update()
        {
            //CheckShots();

            if(_prevActivatedGameObject != null)
            {
                UpdateComboTime();
            }

            //mult = ComboScoreMultiplier;
        }

        //private void CheckShots()
        //{
        //    foreach (Shot shot in _shots)
        //    {
        //        if (shot.CheckActivation())
        //        {
        //            if (_prevActivatedShot == null)
        //            {
        //                _prevActivatedShot = shot;
        //            }
        //            else if (shot != _prevActivatedShot)
        //            {
        //                EndCombo();
        //                _prevActivatedShot = shot;
        //            }

        //            shot.RepeatActivations++;
        //            _elapsedComboTime = 0;
        //        }
        //    }
        //}

        private void UpdateComboTime()
        {
            _elapsedComboTime += Time.deltaTime;
            if(_elapsedComboTime >= _comboTime)
            {
                _elapsedComboTime = 0f;
                EndCombo();
            }
        }

        public void EndCombo()
        {
            _prevActivatedGameObject = null;
            _scoreMultiplier = 1;
            //_prevActivatedShot.RepeatActivations = 0;
            //_prevActivatedShot = null;
        }

        //public void ResetCombos()
        //{
        //    EndCombo();

        //    foreach (Shot shot in _shots)
        //    {
        //        shot.ResetShot();
        //    }
        //}
    }
}
