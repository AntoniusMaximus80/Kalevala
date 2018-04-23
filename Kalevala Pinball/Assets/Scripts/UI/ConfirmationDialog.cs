using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using L10n = Kalevala.Localization.Localization;

namespace Kalevala
{
    public enum ConfirmationType
    {
        QuitGame = 0,
        StartGame = 1,
        ReturnToMainMenu = 2,
        SaveSettings = 3,
        EraseHighscores = 4
    }

    public class ConfirmationDialog : MonoBehaviour, IMenu
    {
        public enum InputType
        {
            NoInput = 0,
            Accept = 1,
            Decline = 2,
            AltDecline = 3
        }

        [SerializeField]
        private GameObject _dialogBox;

        [SerializeField]
        private Text _dialogText;

        [SerializeField]
        private Text _acceptText;

        [SerializeField]
        private Text _declineText;

        [SerializeField]
        private Text _altDeclineText;

        //private LanguageStateBase _language;
        private InputType _input = InputType.NoInput;

        private Button[] _menuButtons;

        public ConfirmationType Type { get; private set; }

        private void Start()
        {
            //UpdateLanguage();

            if (_dialogBox == null)
            {
                Debug.LogError("Dialog box is not set.");
            }
            else
            {
                _menuButtons = _dialogBox.GetComponentsInChildren<Button>();
            }

            if (_dialogText == null)
            {
                Debug.LogError("Dialog box dialog text is not set.");
            }
            if (_acceptText == null)
            {
                Debug.LogError("Dialog box accept text is not set.");
            }
            if (_declineText == null)
            {
                Debug.LogError("Dialog box decline text is not set.");
            }
            if (_altDeclineText == null)
            {
                Debug.LogError("Dialog box alt decline text is not set.");
            }
            else
            {
                _altDeclineText.text = L10n.CurrentLanguage.GetTranslation("cancel");
                _altDeclineText.transform.parent.gameObject.SetActive(false);
            }
        }

        //public void UpdateLanguage()
        //{
        //    _language = GameManager.Instance.Language;
        //}

        /// <summary>
        /// Is the confirmation screen active
        /// </summary>
        public bool Active
        {
            get
            {
                if (_dialogBox == null)
                {
                    Debug.LogError("Dialog box is not set.");
                    return false;
                }

                return _dialogBox.activeSelf;
            }
        }

        public void Activate(ConfirmationType type)
        {
            if (_dialogBox != null)
            {
                Type = type;
                _dialogBox.SetActive(true);
                _dialogText.text = GetConfirmationText();
                _input = InputType.NoInput;

                switch (type)
                {
                    case ConfirmationType.StartGame:
                    {
                        _acceptText.text = L10n.CurrentLanguage.GetTranslation("yes");
                        _declineText.text = L10n.CurrentLanguage.GetTranslation("no");
                        break;
                    }
                    case ConfirmationType.SaveSettings:
                    {
                        _altDeclineText.transform.parent.gameObject.SetActive(true);
                        _altDeclineText.text = L10n.CurrentLanguage.GetTranslation("cancel");
                        _acceptText.text = L10n.CurrentLanguage.GetTranslation("apply");
                        _declineText.text = L10n.CurrentLanguage.GetTranslation("revert");
                        break;
                    }
                    case ConfirmationType.EraseHighscores:
                    {
                        _acceptText.text = L10n.CurrentLanguage.GetTranslation("accept");
                        _declineText.text = L10n.CurrentLanguage.GetTranslation("cancel");
                        break;
                    }
                    default:
                    {
                        _acceptText.text = L10n.CurrentLanguage.GetTranslation("ok");
                        _declineText.text = L10n.CurrentLanguage.GetTranslation("cancel");
                        break;
                    }
                }
            }
        }

        public void Deactivate()
        {
            if (_dialogBox != null)
            {
                _dialogBox.SetActive(false);

                if (Type == ConfirmationType.SaveSettings)
                {
                    _altDeclineText.transform.parent.gameObject.SetActive(false);
                }
            }
        }

        public void Accept()
        {
            _input = InputType.Accept;
        }

        public void Decline()
        {
            _input = InputType.Decline;
        }

        public void AltDecline()
        {
            _input = InputType.AltDecline;
        }

        public string GetConfirmationText()
        {
            if (Active)
            {
                switch (Type)
                {
                    case ConfirmationType.QuitGame:
                    {
                        return L10n.CurrentLanguage.GetTranslation("confirmExitGame");
                    }
                    case ConfirmationType.StartGame:
                    {
                        return L10n.CurrentLanguage.GetTranslation("confirmStartGame");
                    }
                    case ConfirmationType.ReturnToMainMenu:
                    {
                        return L10n.CurrentLanguage.GetTranslation("confirmReturnToMainMenu");
                    }
                    case ConfirmationType.SaveSettings:
                    {
                        return L10n.CurrentLanguage.GetTranslation("confirmSaveSettings");
                    }
                    case ConfirmationType.EraseHighscores:
                    {
                        return L10n.CurrentLanguage.GetTranslation("confirmEraseHighscores");
                    }
                }
            }

            return "--- ?";
        }

        public InputType GetInput()
        {
            if (Active)
            {
                // On-screen button input
                if (_input != InputType.NoInput)
                {
                    return _input;
                }

                // Decline
                if (Input.GetButtonUp("Cancel"))
                {
                    if (Type == ConfirmationType.SaveSettings)
                    {
                        return InputType.AltDecline;
                    }
                    else
                    {
                        return InputType.Decline;
                    }
                }
            }

            return InputType.NoInput;
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
                if (button.gameObject.tag.Equals(
                        ScreenStateBase.DEFAULT_SELECTED_BUTTON))
                {
                    defaultSelectedButton = button;
                    break;
                }
            }

            //Debug.Log("Selected confirmation button: " + defaultSelectedButton.name);

            return defaultSelectedButton;
        }
    }
}
