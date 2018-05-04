using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class ObjectiveManager: MonoBehaviour
    {

        #region Statics
        private static ObjectiveManager instance;

        /// <summary>
        /// Gets or sets the Singleton instance .
        /// </summary>
        public static ObjectiveManager Instance
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
                        Instantiate(Resources.Load<ObjectiveManager>("ObjectiveManager"));
                }

                return instance;
            }
        }
        #endregion Statics

        [SerializeField]
        private Objective[] _objectives;

        private Objective _activeObjective;

        // Use this for initialization
        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
            StartRandomObjective();
        }

        public void CheckObjective( GameObject obj)
        {
            if(_activeObjective == null)
            {
                return;
            }

            if(_activeObjective.MyObjective == obj)
            {
                ObjectiveCleared();
                StartRandomObjective();
            }
        }

        private void StartRandomObjective()
        {
            if(_objectives.Length < 1)
            {
                Debug.Log("No objectives in the list");
                return;
            }
            int random = 0;

            if(_activeObjective == null)
            {
                random = Random.Range(0, _objectives.Length);
                _activeObjective = _objectives[random];
                _activeObjective.ActivateObjective();
                return;
            }

            int randomizerCounter = 0;

            do
            {
                randomizerCounter++;
                random = Random.Range(0, _objectives.Length);
            } while(_objectives[random] == _activeObjective && randomizerCounter < 10);

            _activeObjective = _objectives[random];
            _activeObjective.ActivateObjective();
        }

        private void ObjectiveCleared()
        {
            _activeObjective.DeactivateObjective();
        }
        public void Reset()
        {
            _activeObjective.DeactivateObjective();
            _activeObjective = null;
        }

    }
}
