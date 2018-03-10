using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kalevala
{
    public class LanguageChanger : MonoBehaviour
    {
        [SerializeField]
        private string _textID;

        private Text _text;

        private void Start()
        {
            _text = GetComponent<Text>();
            if (_text == null)
            {
                Debug.LogError("Text component not found in the object");
            }
            else
            {
                UpdateText();
            }
        }

        public void UpdateText()
        {
            if (_text != null)
            {
                _text.text = GameManager.Instance.Language.GetText(_textID);
            }
        }
    }
}
