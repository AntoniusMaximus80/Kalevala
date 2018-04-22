using UnityEngine;
using Kalevala.Localization;

namespace Kalevala.UI
{
    public class LocalizationUI : MonoBehaviour
    {
        public void SetLanguage(int langCode)
        {
            Localization.Localization.LoadLanguage((LangCode) langCode);
        }

        public void SetEnglish()
        {
            Localization.Localization.LoadLanguage(LangCode.EN);
        }

        public void SetFinnish()
        {
            Localization.Localization.LoadLanguage(LangCode.FI);
        }
    }
}
