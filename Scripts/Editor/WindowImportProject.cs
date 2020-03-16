using System;
using System.Reflection;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Creobit.Localization.Editor
{
    public class WindowImportProject : EditorWindow
    {
        private ILocalizationData _data;      
        public ImportLocalizationData _importData;
        

        internal static void Open(ILocalizationData targetObj)
        {
            var target = targetObj ?? throw new ArgumentNullException(nameof(targetObj));
            var window = GetWindow(typeof(WindowImportProject)) as WindowImportProject;
            window.titleContent = new GUIContent("ImportLanguage");
            window._data = target;
            
        }

        private void OnEnable()
        {
            _importData = GoogleSheetImporter.Load();
        }

        private void OnGUI()
        {            
            EditorGUILayout.Space();
            _importData?.OnGui();
            EditorGUILayout.Space();
            OnGuiButtons();
        }

        private void OnGuiButtons()
        {
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = _importData != null;

            if (GUILayout.Button("Import Language"))
            {
                _data.SetData(_importData.Languages, _importData.Keys);
                _importData = null;
                Close();
                Debug.Log("Successful import");
            }

            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
        
    }
}
