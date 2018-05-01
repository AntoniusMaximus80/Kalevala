using System;
using UnityEngine;
using UnityEngine.UI;

namespace Kalevala
{
    public class RampChanger : MonoBehaviour
    {
        [SerializeField]
        private bool _useMainRampByDefault;

        [SerializeField]
        private RampEntrance _mainRamp;

        [SerializeField]
        private RampEntrance _altRamp;

        /// <summary>
        /// An event which is fired when either of the set ramps is activated.
        /// True: main ramp, False: alt ramp.
        /// </summary>
        public event Action<bool> RampActivated;

        /// <summary>
        /// An event which is fired when the active ramp is deactivated.
        /// </summary>
        public event Action RampDeactivated;

        public RampEntrance ActiveRamp { get; private set; }

        private void Start()
        {
            if (_mainRamp != null)
            {
                if (_useMainRampByDefault)
                {
                    ActivateRamp(true);
                }
                else
                {
                    DeactivateRamp(_mainRamp);
                }
            }
            else
            {
                Debug.LogError("Main ramp is not set.");
            }

            if (_altRamp != null)
            {
                DeactivateRamp(_altRamp);
            }
        }

        public void ToggleRamp(bool activateIfNeeded)
        {
            if (ActiveRamp != null)
            {
                bool useMainRamp = (ActiveRamp != _mainRamp);
                ActivateRamp(useMainRamp);
            }
            else if (activateIfNeeded)
            {
                ActivateRamp(true);
            }
        }

        public void ActivateRamp(bool useMainRamp)
        {
            if (ActiveRamp != null)
            {
                DeactivateRamp(ActiveRamp);
            }

            if (useMainRamp)
            {
                ActiveRamp = _mainRamp;
            }
            else if (_altRamp != null)
            {
                ActiveRamp = _altRamp;
            }
            else
            {
                Debug.LogError("Could not activate alt ramp; it is not set.");
                return;
            }

            ActiveRamp.Active = true;
            //ActiveRamp.GizmosColor = _gizmosOnColor;

            if (RampActivated != null)
            {
                RampActivated(useMainRamp);
            }
        }

        public void DeactivateActiveRamp()
        {
            if (ActiveRamp != null)
            {
                DeactivateRamp(ActiveRamp);
                ActiveRamp = null;

                if (RampDeactivated != null)
                {
                    RampDeactivated();
                }
            }
        }

        private void DeactivateRamp(RampEntrance ramp)
        {
            ramp.Active = false;
            //ramp.GizmosColor = _gizmosOffColor;
        }
    }
}
