using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using Kalevala.Localization;
using L10n = Kalevala.Localization.Localization;

namespace Kalevala.Editor
{
    public class LocalizationWindow : EditorWindow
    {
        private const string LocalizationKey = "Localization";

        public LangCode CurrentLanguage = LangCode.None;

        private Dictionary<string, string> localizations =
            new Dictionary<string, string>();

        /// <summary>
        /// Opens a localization editor window.
        /// </summary>
        [MenuItem("Localization/Edit Localization")]
        private static void OpenWindow()
        {
            LocalizationWindow window =
                GetWindow<LocalizationWindow>("Edit Localization");
            window.Show();
        }

        private void OnEnable()
        {
            LangCode language = (LangCode) EditorPrefs.
                GetInt(LocalizationKey, (int) LangCode.None);
            SetLanguage(language);
        }

        private void OnGUI()
        {
            LangCode langCode = (LangCode) EditorGUILayout.
                EnumPopup(CurrentLanguage);
            SetLanguage(langCode);

            EditorGUILayout.BeginVertical();

            Dictionary<string, string> newValues =
                new Dictionary<string, string>();

            List<string> deletedKeys = new List<string>();

            foreach (var localization in localizations)
            {
                EditorGUILayout.BeginHorizontal();

                string key = EditorGUILayout.TextField(localization.Key);
                string value = EditorGUILayout.TextField(localization.Value);

                newValues.Add(key, value);

                if (DeleteButtonPressed())
                {
                    deletedKeys.Add(localization.Key);
                }

                EditorGUILayout.EndHorizontal();
            }

            localizations = newValues;

            DeleteKeys(deletedKeys);

            AddValueButton();

            SaveButton();

            EditorGUILayout.EndVertical();
        }

        private void DeleteKeys(List<string> deletedKeys)
        {
            foreach (var deletedKey in deletedKeys)
            {
                if (localizations.ContainsKey(deletedKey))
                {
                    localizations.Remove(deletedKey);
                }
            }
        }

        private bool DeleteButtonPressed()
        {
            return GUILayout.Button("X");
        }

        private void AddValueButton()
        {
            if (GUILayout.Button("Add Value"))
            {
                if ( !localizations.ContainsKey("") )
                {
                    localizations.Add("", "");
                }
            }
        }

        private void SaveButton()
        {
            if (GUILayout.Button("Save"))
            {
                L10n.CurrentLanguage.SetValues(localizations);
                L10n.SaveCurrentLanguage();
            }
        }

        private void SetLanguage(LangCode langCode)
        {
            // Current language is already set to langCode. Just return.
            if (CurrentLanguage == langCode)
            {
                return;
            }

            CurrentLanguage = langCode;
            EditorPrefs.SetInt(LocalizationKey, (int) CurrentLanguage);
            localizations.Clear();

            // Load localization file
            L10n.LoadLanguage(CurrentLanguage);
            localizations = L10n.CurrentLanguage.GetValues();
        }
    }
}
