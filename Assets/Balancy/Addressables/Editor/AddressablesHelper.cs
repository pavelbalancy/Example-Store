﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.Networking;
using Balancy.Addressables;
using Newtonsoft.Json;
using Object = UnityEngine.Object;

namespace Balancy.Editor
{
    [InitializeOnLoad]
    public static class AddressablesHelper
    {
        private class ServerRequest : Utils.IRequestInfo
        {
            public string Url { get; }
            public Dictionary<string, string> Headers { get; }
            public Dictionary<string, object> Body { get; }
            public bool IsMultipart { get; set; }
            public List<Utils.ImageInfo> Images { get; }

            public string Method;

            public string GetMethod()
            {
                return Method;
            }

            public ServerRequest(string url)
            {
                Url = Constants.GeneralConstants.ADMINKA_API_URL + url;
                Headers = new Dictionary<string, string>();
                Body = new Dictionary<string, object>();
                Images = new List<Utils.ImageInfo>();
                Method = "POST";
            }

            public ServerRequest SetHeader(string key, string value)
            {
                if (Headers.ContainsKey(key))
                    Headers[key] = value;
                else
                    Headers.Add(key, value);
                return this;
            }
            
            public ServerRequest AddBody(string key, object value)
            {
                Body.Add(key, value);
                return this;
            }

            public ServerRequest AddTexture(Texture2D img, string name, string prefabName)
            {
                Images.Add(new Utils.ImageInfo(img, name, prefabName));

                return this;
            }
            
            public ServerRequest SetMultipart()
            {
                IsMultipart = true;

                return this;
            }
        }

        private class FullInfo
        {
            public AddressablesGroup[] groups;
        }

        private class AddressablesGroup
        {
            public string guid;
            public string name;
            public FileInfo[] entries;

            public AddressablesGroup(int size)
            {
                entries = new FileInfo[size];
            }
        }

        private class FileInfo
        {
            [JsonIgnore]
            public Object link;
            [JsonIgnore]
            public string path;
            [JsonIgnore]
            public string texturePath;

            public string guid;
            public string name;
            public string hash;
            [JsonIgnore]
            public string group;
        }
        
        private class SynchAddressablesResponse
        {
            public string[] assets;
            public Size size;
            
            public class Size
            {
                public int x;
                public int y;
            }
        }

        private class GameInfo
        {
            public string GameId;
            public string Private;
            public Constants.Environment Environment;
            public Action<string, float> OnProgress;
            public Action<string> OnComplete;
        }

        static AddressablesHelper()
        {
            Balancy_Editor.SynchAddressablesEvent += SynchAddressables;
        }

        public static void SynchAddressables(string gameId, string privateKey, Constants.Environment environment, Action<string, float> onProgress, Action<string> onComplete)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("No Addressables Found in the project");
                return;
            }

            var gameInfo = new GameInfo
            {
                GameId = gameId,
                Private = privateKey,
                Environment = environment,
                OnProgress = onProgress,
                OnComplete = onComplete
            };

            var info = ReadData(settings);
            CalculateHashes(info);
            SendInfoToServer(info, gameInfo);
        }

        private static FullInfo ReadData(AddressableAssetSettings settings)
        {
            var groupsCount = GetGroupsCount(settings);
            var info = new FullInfo {groups = new AddressablesGroup[groupsCount]};

            int i = 0;
            foreach (var group in settings.groups)
            {
                if (group.ReadOnly)
                    continue;

                var entries = group.entries;

                var infoGroup = info.groups[i] = new AddressablesGroup(entries.Count);
                infoGroup.name = group.Name;
                infoGroup.guid = group.Guid;
                var files = infoGroup.entries;

                var j = 0;
                foreach (var entry in entries)
                {
                    files[j++] = new FileInfo
                    {
                        link = entry.MainAsset,
                        guid = entry.guid,
                        name = entry.address,
                        path = entry.AssetPath,
                        group = group.Name
                    };
                }

                i++;
            }

            return info;
        }

        private static int GetGroupsCount(AddressableAssetSettings settings)
        {
            return settings.groups.Count(group => !group.ReadOnly);
        }

        private static void CalculateHashes(FullInfo info)
        {
            foreach (var group in info.groups)
            {
                foreach (var entry in group.entries)
                {
                    string filePath = null;
                    switch (entry.link)
                    {
                        case Texture2D _texture2D:
                            filePath = entry.texturePath = entry.path;
                            break;
                        case GameObject _gameObject:
                        {
                            var script = _gameObject.GetComponentInChildren<IUnnyAsset>();
                            filePath = entry.texturePath = script?.GetPreviewImagePath();
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(filePath))
                    {
                        var md5 = MD5.Create();
                        var stream = File.OpenRead(filePath);
                        var checkSum = md5.ComputeHash(stream);
                        var hash = BitConverter.ToString(checkSum).Replace("-", string.Empty);
                        entry.hash = hash;
                    }
                }
            }
        }

        private static void SendInfoToServer(FullInfo info, GameInfo gameInfo)
        {
            var helper = EditorCoroutineHelper.Create();
            var req = new ServerRequest("/v1/check_assets");
            req.SetHeader("Content-Type", "application/json")
                .SetHeader("Authorization", "Basic " + gameInfo.Private)
                .SetHeader("game-id", gameInfo.GameId)
                .AddBody("groups", info.groups)
                .AddBody("env", (int) gameInfo.Environment);
            
            var cor = Utils.SendRequest(req, request =>
            {
#if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.Success)
#else
                if (request.isNetworkError || request.isHttpError)
#endif
                {
                    gameInfo.OnComplete?.Invoke(request.error);
                }
                else
                {
                    var response = JsonConvert.DeserializeObject<SynchAddressablesResponse>(request.downloadHandler.text);
                    SendImagesToServer(gameInfo, info, response.assets, new Vector2Int(response.size.x, response.size.y));
                }
            });
            helper.LaunchCoroutine(cor);
        }
        
        private static void SendImagesToServer(GameInfo gameInfo, FullInfo info, string[] guids, Vector2Int maxSize)
        {
            var helper = EditorCoroutineHelper.Create();

            var cor = SendImagesToServerCoroutine(gameInfo, info, guids, maxSize);

            helper.LaunchCoroutine(cor);
        }
        
        private static IEnumerator SendImagesToServerCoroutine(GameInfo gameInfo, FullInfo info, string[] guids, Vector2Int maxSize)
        {
            var dict = MapFiles(info);
            int uploadedFiles = 0;
            string currentError = null;
            foreach (var guid in guids)
            {
                if (!string.IsNullOrEmpty(currentError))
                    break;
                
                if (!dict.TryGetValue(guid, out var fileInfo))
                {
                    Debug.LogError("File with guid " + guid + " wasn't found");
                    continue;
                }

                var newTexture = GetCompressedTexture(fileInfo, maxSize);

                if (newTexture == null)
                    continue;

                gameInfo.OnProgress?.Invoke(fileInfo.name, (float)uploadedFiles / guids.Length);
                uploadedFiles++;
                var req = new ServerRequest("/v1/upload_img");
                req.SetHeader("Authorization", "Basic " + gameInfo.Private)
                    .SetHeader("game-id", gameInfo.GameId)
                    .AddBody("guid", guid)
                    .AddBody("hash", fileInfo.hash)
                    .AddBody("group", fileInfo.group)
                    .AddBody("name", fileInfo.name)
                    .AddBody("env", (int) gameInfo.Environment)
                    .SetMultipart()
                    .AddTexture(newTexture, fileInfo.texturePath, fileInfo.name);

                yield return Utils.SendRequest(req, request =>
                {
#if UNITY_2020_1_OR_NEWER
                    if (request.result != UnityWebRequest.Result.Success)
#else
                    if (request.isNetworkError || request.isHttpError)
#endif
                    {
                        currentError = request.error;
                        Debug.LogWarning("Image upload error: " + request.error + " : " + request.url);
                    }
                });
            }
            
            gameInfo.OnComplete?.Invoke(currentError);
        }

        private static Texture2D GetCompressedTexture(FileInfo fileInfo, Vector2Int maxSize)
        {
            if (string.IsNullOrEmpty(fileInfo.texturePath))
            {
                Debug.LogError("No image found for guid " + fileInfo.guid);
                return null;
            }

            var tImporter = AssetImporter.GetAtPath(fileInfo.texturePath) as TextureImporter;
            if (tImporter == null)
                return null;

            tImporter.isReadable = true;
            AssetDatabase.ImportAsset(fileInfo.texturePath);
            AssetDatabase.Refresh();
        
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(fileInfo.texturePath);

            var scaleX = (float) maxSize.x / texture.width;
            var scaleY = (float) maxSize.y / texture.height;
            var scale = Mathf.Min(scaleX, scaleY);

            int newWidth = Mathf.RoundToInt(scale * texture.width);
            int newHeight = Mathf.RoundToInt(scale * texture.height);

            var newTexture = Utils.ScaleTexture(texture, newWidth, newHeight);
            
            tImporter.isReadable = false;
            AssetDatabase.ImportAsset(fileInfo.texturePath);
            AssetDatabase.Refresh();
        
            return newTexture;
        } 

        private static Dictionary<string, FileInfo> MapFiles(FullInfo info)
        {
            return info.groups.SelectMany(group => group.entries).ToDictionary(entry => entry.guid);
        }

        private static string ConvertToJson(FullInfo info)
        {
            return JsonConvert.SerializeObject(info);
        }
    }
}