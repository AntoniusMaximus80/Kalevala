using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kalevala
{
    public class ScreenStateMenu : ScreenStateBase, IMenu
    {
        private Button[] _menuButtons;

        public ScreenStateMenu(StateManager owner, GameObject uiObject,
            ScreenStateType stateType)
            : base(owner, stateType)
        {
            ScreenObject = uiObject;
            _menuButtons = ScreenObject.GetComponentsInChildren<Button>();
        }

        public override void Update()
        {
        }

        public Button[] GetMenuButtons()
        {
            return _menuButtons;
        }

        public Button GetDefaultSelectedButton()
        {
            Button defaultSelectedButton = null;

            foreach (Button button in _menuButtons)
            {
                if (button.gameObject.tag.Equals(DEFAULT_SELECTED_BUTTON))
                {
                    defaultSelectedButton = button;
                    break;
                }
            }

            //Debug.Log("Selected button: " + defaultSelectedButton.name);

            return defaultSelectedButton;
        }
    }
}
