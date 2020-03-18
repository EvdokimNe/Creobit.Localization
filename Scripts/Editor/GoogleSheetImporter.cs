﻿#if CREOBIT_LOCALIZATION_GOOGLEDOCS
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Creobit.Localization.Editor
{
    [CreateAssetMenu(fileName = "GoogleSheetImporter", menuName = "Creobit/Localization/GoogleSheetImporter")]
    public class GoogleSheetImporter : ScriptableObject
    {
        #region GoogleSheetImporter

        [SerializeField]
        private string _clientId;

        [SerializeField]
        private string _clientSecret;

        [SerializeField]
        private string _spreadsheetId;

        [SerializeField]
        private string[] _sheetNames;

        [SerializeField]
        private LocalizationData _localizationData;

        private List<(string Language, string Key, string Value)> _globalEntries;

        public async Task ImportAsync(CancellationToken cancellationToken)
        {
            var entries = new List<(string Language, string Key, string Value)>();          

            await ImportAsync();

            async Task ImportAsync()
            {
                var clientSecrets = new ClientSecrets
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret
                };
                var scopes = new[]
                {
                    SheetsService.Scope.SpreadsheetsReadonly
                };

                var userCredential = await GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecrets, scopes, "user", cancellationToken);
                var clientServiceInitializer = new BaseClientService.Initializer
                {
                    HttpClientInitializer = userCredential
                };

                using (var sheetsService = new SheetsService(clientServiceInitializer))
                {
                    var spreadsheets = sheetsService.Spreadsheets;
                    var valuesResource = spreadsheets.Values;

                    foreach (var sheetName in _sheetNames)
                    {
                        var getRequest = valuesResource.Get(_spreadsheetId, sheetName);
                        var valueRange = await getRequest.ExecuteAsync(cancellationToken);
                        var values = valueRange.Values;

                        UpdateEntries(values);
                    }

                    _globalEntries = entries;  
                    var importLocalizationData = Load();
                    WindowImportProject.Open(_localizationData, importLocalizationData);
                }
            }

            void UpdateEntries(IList<IList<object>> values)
            {
                for (var rowIndex = 1; rowIndex < values.Count; ++rowIndex)
                {
                    var key = Convert.ToString(values[rowIndex][0]).Trim();

                    if (string.IsNullOrWhiteSpace(key) || key.StartsWith("["))
                    {
                        continue;
                    }

                    for (var columnIndex = 1; columnIndex < values[rowIndex].Count; ++columnIndex)
                    {
                        var language = Convert.ToString(values[0][columnIndex]).Trim();    
                        var value = Convert.ToString(values[rowIndex][columnIndex]).Trim();
                        entries.Add((language, key, value));          
                    }
                }
            }

        }

        private ImportLocalizationData Load()
        {       
            var languages = GetLanguages();
            var globalKeys = GetKeyValues();
            return new ImportLocalizationData(languages, globalKeys);            

            IEnumerable<ImportLanguage> GetLanguages()
            {
                var resultImportLanguage = new List<ImportLanguage>();
                var listLang = GetLanguagesString().ToList();
                for (var i = 0; i < listLang.Count(); ++i)
                {
                    var value = listLang[i];
                    var importLanguage = new ImportLanguage(value);
                    resultImportLanguage.Add(importLanguage);
                }
                return resultImportLanguage;
            }

            IEnumerable<string> GetLanguagesString()
            {
                return _globalEntries
                    .Select(x => x.Language)
                    .Distinct();
            }

            IEnumerable<LanguagesKeyValue> GetKeyValues()
            {
                var groups = _globalEntries
                    .Select(x => (x.Key, x.Value))
                    .GroupBy(x => x.Key);

                foreach (var group in groups)
                {
                    var values = group
                        .AsEnumerable()
                        .Select(x => x.Value);

                    yield return new LanguagesKeyValue(group.Key, values);
                }
            }
        }

        #endregion
    }
}
#endif
