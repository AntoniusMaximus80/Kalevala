using UnityEngine;
using UnityEngine.UI;
using L10n = Kalevala.Localization.Localization;

namespace Kalevala.UI
{
    public class LocalizedLabel : MonoBehaviour
    {
        [SerializeField]
        private string key;

        private Text textObj;

        public string Text { get; set; }

        private void Awake()
        {
            textObj = GetComponent<Text>();

            L10n.LanguageLoaded += OnLanguageLoaded;
            OnLanguageLoaded();
        }

        private void OnLanguageLoaded()
        {
            Text = L10n.CurrentLanguage.GetTranslation(key);

            if (textObj != null)
            {
                textObj.text = Text;
            }
        }
    }
}
