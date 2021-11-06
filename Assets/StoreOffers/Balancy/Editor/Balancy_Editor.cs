﻿using System;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

namespace Balancy.Editor
{
    [ExecuteInEditMode]
    public class Balancy_Editor : EditorWindow
    {
        const string BalancyDefine = "BALANCY";

        public delegate void SynchAddressablesDelegate(string gameId, string privateKey, Constants.Environment environment, Action<string, float> onProgress, Action<string> onComplete);
        public static event SynchAddressablesDelegate SynchAddressablesEvent;

        private static void AddBalancySymbols(string newDefines)
        {
            var newDefineSymbols = newDefines
                .Split(';')
                .Select(d => d.Trim())
                .ToList();
            
            foreach (BuildTarget target in System.Enum.GetValues(typeof(BuildTarget)))
            {
                BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);

                if (group == BuildTargetGroup.Unknown)
                {
                    continue;
                }

                string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);

                var defineSymbols = defines
                    .Split(';')
                    .Select(d => d.Trim())
                    .ToList();

                string definesToAdd = string.Empty;
                
                foreach (var newD in newDefineSymbols)
                {
                    bool found = defineSymbols.Any(symbol => newD.Equals(symbol));

                    if (!found)
                        definesToAdd += ";" + newD;
                }

                if (!string.IsNullOrEmpty(definesToAdd))
                {
                    try
                    {
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(group,  defines + definesToAdd);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogErrorFormat("Could not add Balancy define symbols for build target: {0} group: {1}, {2}", target, group, e);
                    }
                }
            }
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        public static void OnScriptsReloaded()
        {
            AddBalancySymbols(BalancyDefine);
        }

        #region Settings Window
        
        [MenuItem("Tools/Balancy", false, -100000)]
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(Balancy_Editor));
            window.titleContent.text = "Balancy";
            window.titleContent.image = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Balancy/Editor/BalancyLogo.png");
        }

        private void Awake()
        {
            minSize = new Vector2(500, 500);
        }

        private static void OpenHash(string hash)
        {
            Application.OpenURL("https://docs.balancy.dev/" + hash);
        }
        
        public class BalancySettings
        {
            public string ApiGameId;
            public string PublicKey;
            public string PrivateKey;
            public bool AutoDownloadDictionaries;
        }
        
        readonly string[] SERVER_TYPE = {"Development", "Stage", "Production"};
        private Balancy_Plugins plugins;
        
        private BalancySettings _settings;
        private string _apiGameId;
        private string _privateKey;
        private string _publicKey;
        private bool _autoDownloadDictionaries;
        private int _selectedServer;
        private bool _downloading;
        private float _downloadingProgress;
        private string _downloadingFileName;

        private string _jsonPath => Plugins.JsonPath;
        
        private Balancy_Plugins Plugins
        {
            get
            {
                if (plugins == null)
                    plugins = new Balancy_Plugins(this);
                return plugins;
            }
        }
        
        private void SaveUnnyJson()
        {
            string finalString = JsonUtility.ToJson(_settings);

            Balancy.Utils.CheckAndCreateDirectoryForFile(_jsonPath);
            StreamWriter writer = new StreamWriter(_jsonPath, false);
            writer.WriteLine(finalString);
            writer.Close();
            AssetDatabase.ImportAsset(_jsonPath);
        }
        
        private void OnEnable()
        {
            EditorApplication.update += update;
        }
        
        private void OnDisable()
        {
            EditorApplication.update -= update;
        }

        private void update()
        {
            if (_downloading)
                Repaint();
        }

        private void PrepareSettings()
        {
            if (_settings != null)
                return;
            
            _settings = CreateOrLoadUnnyJson();
            _apiGameId = _settings.ApiGameId;
            _privateKey = _settings.PrivateKey;
            _publicKey = _settings.PublicKey;
            _autoDownloadDictionaries = _settings.AutoDownloadDictionaries;
        }

        public BalancySettings CreateOrLoadUnnyJson()
        {
            TextAsset textAsset = (TextAsset)AssetDatabase.LoadAssetAtPath(_jsonPath, typeof(TextAsset));
            return textAsset == null ? new BalancySettings() : JsonUtility.FromJson<BalancySettings>(textAsset.text);
        }

        private void OnGUI()
        {
            PrepareSettings();

            GUI.enabled = !_downloading;
            
            RenderSettings();
            EditorGUILayout.Space();
            RenderLoader();
            EditorGUILayout.Space();
            Plugins.Render();
        }
        
        private void RenderSettings()
        {
            m_AnyChanges = false;
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label("Balancy Settings");

            SetColor(!string.Equals(_apiGameId,_settings.ApiGameId));
            GUILayout.BeginHorizontal();
            var validGameId = IsGameIdValid(_apiGameId);
            if (validGameId)
                SetColor(false);
            else
                SetErrorColor();
            _apiGameId = EditorGUILayout.TextField("Api Game ID", _apiGameId);

            if (!validGameId)
            {
                GUILayout.Label(new GUIContent("?", "Incorrect game id"), GUILayout.Width(20));
            }
            GUILayout.EndHorizontal();
            
            SetColor(!string.Equals(_publicKey,_settings.PublicKey));
            _publicKey = EditorGUILayout.TextField("Public key", _publicKey);
            
            SetColor(!string.Equals(_privateKey,_settings.PrivateKey));
            _privateKey = EditorGUILayout.TextField("Private key", _privateKey);
            
            // SetColor(!string.Equals(_autoDownloadDictionaries,_settings.AutoDownloadDictionaries));
            // _autoDownloadDictionaries = EditorGUILayout.Toggle(new GUIContent
            // {
            //     text = "AutoDownload Game Data",
            //     tooltip = "Automatically update Game Data from Balancy servers before the build"
            // }, _autoDownloadDictionaries);

            SetColor(m_AnyChanges);
            GUI.enabled = m_AnyChanges && !_downloading && IsGameIdValid(_apiGameId);
            if (GUILayout.Button("Save"))
            {
                _settings.ApiGameId = _apiGameId;
                _settings.PublicKey = _publicKey;
                _settings.PrivateKey = _privateKey;
                _settings.AutoDownloadDictionaries = _autoDownloadDictionaries;
                SaveUnnyJson();
            }

            GUILayout.EndVertical();
        }

        private void RenderLoader()
        {
            SetColor(false);
            GUI.enabled = !m_AnyChanges && !_downloading && !string.IsNullOrEmpty(_settings.PublicKey) && !string.IsNullOrEmpty(_settings.PrivateKey) && IsGameIdValid(_apiGameId);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label("Data Editor");
            _selectedServer = GUILayout.SelectionGrid(_selectedServer, SERVER_TYPE, SERVER_TYPE.Length, EditorStyles.radioButton);

            if (_downloading)
            {
                GUI.enabled = true;
                var rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
                EditorGUI.ProgressBar(rect, _downloadingProgress, _downloadingFileName);
                GUI.enabled = false;
            }
            else
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Generate Code"))
                    StartCodeGeneration();

                if (GUILayout.Button("Download Data"))
                    StartDownloading();
                
                if (GUILayout.Button("Synch Addressables"))
                    StartSynchingAddressables();
                
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }

        private void StartCodeGeneration()
        {
            _downloading = true;
            _downloadingProgress = 0.5f;
            _downloadingFileName = "Generating the code...";
            Balancy_CodeGeneration.StartGeneration(_settings.ApiGameId, _settings.PublicKey, (Constants.Environment) _selectedServer, () => { _downloading = false; });
        }
        
        private void StartSynchingAddressables()
        {
            if (SynchAddressablesEvent == null)
            {
                EditorUtility.DisplayDialog("Warning", "Addressables Plugin is not installed. Please install it below and don't forget to import Unity's Addressables from Package Manager", "Got it");
            }
            else
            {
                _downloading = true;
                _downloadingProgress = 0f;
                _downloadingFileName = "Synchronizing addressables...";
                SynchAddressablesEvent(
                    _settings.ApiGameId,
                    _settings.PrivateKey,
                    (Constants.Environment) _selectedServer,
                    (fileName, progress) =>
                    {
                        _downloadingFileName = fileName;
                        _downloadingProgress = progress;
                    },
                    (error) =>
                    {
                        _downloading = false;
                        if (!string.IsNullOrEmpty(error))
                            EditorUtility.DisplayDialog("Error", error, "Ok");
                        else
                            EditorUtility.DisplayDialog("Success", "Addressables are now synched. Please reload Balancy web page", "Ok");
                    }
                );
            }
        }

        private void StartDownloading()
        {
            _downloading = true;
            _downloadingProgress = 0;

            var appConfig = new AppConfig
            {
                ApiGameId = _settings.ApiGameId,
                PublicKey = _settings.PublicKey,
                Environment = (Constants.Environment) _selectedServer
            };
            
            DicsHelper.LoadDocs(appConfig, responseData =>
            {
                _downloading = false;
                if (!responseData.Success)
                    EditorUtility.DisplayDialog("Error", responseData.Error.Message, "Ok");
            }, (fileName, progress) =>
            {
                _downloadingFileName = fileName;
                _downloadingProgress = progress;
            });
        }

        // static bool IsGameIdValid()
        // {
        //     return IsGameIdValid(_settings.ApiGameId);
        // }
        
        static bool IsGameIdValid(string gameId)
        {
            if (string.IsNullOrEmpty(gameId))
                return false;

            return Guid.TryParse(gameId, out _);
        }
        
        private bool m_AnyChanges;

        private void SetColor(bool anyChanges) {
            GUI.color = anyChanges ? Color.green : Color.white;
            if (anyChanges)
                m_AnyChanges = true;
        }

        private void SetErrorColor()
        {
            GUI.color = new Color(204f / 255, 51f / 255, 0, 1f);
        }

        #endregion
    }
}