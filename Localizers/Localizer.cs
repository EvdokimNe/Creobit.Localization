﻿using System;
using UnityEngine;

namespace Creobit.Localization
{
    public abstract class Localizer : MonoBehaviour
    {
        #region MonoBehaviour

        private void OnEnable()
        {
            LocalizationSystem.LocalizationUpdated += OnLocalizationUpdated;
        }

        private void OnDisable()
        {
            LocalizationSystem.LocalizationUpdated -= OnLocalizationUpdated;
        }

        void Start()
        {
            OnLocalizationUpdated(this, EventArgs.Empty);
        }

        #endregion
        #region LocalizationBehaviour

        public static ILocalizationSystem LocalizationSystem;

        [SerializeField]
        private string _key = string.Empty;

        public string Key => _key;

        protected abstract void UpdateValue(string _value);

        private void OnLocalizationUpdated(object sender, EventArgs eventArgs)
        {
            if (string.IsNullOrEmpty(Key))
            {
                Debug.LogError("The localization key is not installed.", this);
            }
            else
            {
                var value = LocalizationSystem.GetString(Key, this);
                UpdateValue(value);
            }
        }

        #endregion
    }
}
