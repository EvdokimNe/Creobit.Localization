using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Creobit.Localization.Editor
{
    public sealed class ImportLocalizationData
    {
        private Vector2 _scrollPositionSheets;
        private Vector2 _scrollPositionLang;

        private readonly IEnumerable<ImportLanguage> ImportLanguages;
        public readonly IEnumerable<LanguagesKeyValue> GlobalKeys;

        public IEnumerable<string> Languages
        {
            get
            {
                var result = new List<string>();

                foreach (var language in ImportLanguages)
                {
                    if (language.IsUsed)
                    {
                        result.Add(language.Value);
                    }
                }

                return result;
            }
        }

        public IEnumerable<LanguagesKeyValue> Keys
        {
            get
            {
                var keys = new List<LanguagesKeyValue>();
                keys.AddRange(GlobalKeys);

                var importLanguages = new List<ImportLanguage>(ImportLanguages);

                for (var i = importLanguages.Count - 1; i >= 0; i--)
                {
                    var lang = importLanguages[i];
                    if (!lang.IsUsed)
                    {
                        foreach (var key in keys)
                        {
                            key.RemoveAt(i);
                        }
                    }
                }

                return keys;
            }
        }

        public ImportLocalizationData(IEnumerable<ImportLanguage> languages, IEnumerable<LanguagesKeyValue> languagesKeys)
        {
            ImportLanguages = languages ?? throw new ArgumentNullException(nameof(languages));
            GlobalKeys = languagesKeys ?? throw new ArgumentNullException(nameof(languagesKeys));
        }


        public void OnGui()
        {
            OnGuiLanguages();           
        }

        private void OnGuiLanguages()
        {
            GUILayout.Label("Languages:");

            EditorGUILayout.BeginVertical("box");
            _scrollPositionLang = EditorGUILayout.BeginScrollView(_scrollPositionLang);

            foreach (var lang in ImportLanguages)
            {
                OnGuiLanguage(lang);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void OnGuiLanguage(ImportLanguage language)
        {
            EditorGUILayout.BeginHorizontal();

            var currentValue = language.IsUsed;
            currentValue = EditorGUILayout.Toggle(currentValue, GUILayout.Width(30));

            if (currentValue != language.IsUsed)
            {
                language.IsUsed = currentValue;
            }

            GUI.enabled = language.IsUsed;
            GUILayout.Label(language.Value);
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
        }

        
    }
}
