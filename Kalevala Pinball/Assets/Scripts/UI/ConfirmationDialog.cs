using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    public class ConfirmationDialog : MonoBehaviour
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

        private LanguageStateBase _language;
        private InputType _input = InputType.NoInput;

        public ConfirmationType Type { get; private set; }

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

        private void Start()
        {
            UpdateLanguage();

            if (_dialogBox == null)
            {
                Debug.LogError("Dialog box is not set.");
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
                _altDeclineText.text = _language.GetText("cancel");
                _altDeclineText.transform.parent.gameObject.SetActive(false);
            }
        }

        public void UpdateLanguage()
        {
            _language = GameManager.Instance.Language;
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
                        _acceptText.text = _language.GetText("yes");
                        _declineText.text = _language.GetText("no");
                        break;
                    }
                    case ConfirmationType.SaveSettings:
                    {
                        _altDeclineText.transform.parent.gameObject.SetActive(true);
                        _acceptText.text = _language.GetText("apply");
                        _declineText.text = _language.GetText("revert");
                        break;
                    }
                    case ConfirmationType.EraseHighscores:
                    {
                        _acceptText.text = _language.GetText("accept");
                        _declineText.text = _language.GetText("cancel");
                        break;
                    }
                    default:
                    {
                        _acceptText.text = _language.GetText("ok");
                        _declineText.text = _language.GetText("cancel");
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
                        return _language.GetText("confirmExitGame");
                    }
                    case ConfirmationType.StartGame:
                    {
                        return _language.GetText("confirmStartGame");
                    }
                    case ConfirmationType.ReturnToMainMenu:
                    {
                        return _language.GetText("confirmReturnToMainMenu");
                    }
                    case ConfirmationType.SaveSettings:
                    {
                        return _language.GetText("confirmSaveSettings");
                    }
                    case ConfirmationType.EraseHighscores:
                    {
                        return _language.GetText("confirmEraseHighscores");
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
    }
}
