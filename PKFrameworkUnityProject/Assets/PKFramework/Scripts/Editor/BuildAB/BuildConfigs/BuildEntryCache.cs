using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace PKFramework.Editor.BuildAsset
{
    //条目信息
    [Serializable]
    public class BuildEntry
    {
        //路径
        public string path;
        //所有要构建的资产信息
        public List<BuildAsset> assets = new List<BuildAsset>();
        //所属group
        public BuildGroup group;

        //包含的资产 路径-资产信息
        private Dictionary<string, BuildAsset> _assetDict = new Dictionary<string, BuildAsset>();

        public void OnAfterDeserialize()
        {
            foreach (var asset in assets)
            {
                _assetDict[asset.path] = asset;
            }
        }

        public void AddAsset(string assetPath)
        {
            if (Settings.customFilter != null && !Settings.customFilter(assetPath))
            {
                return;
            }

            if (_assetDict.TryGetValue(assetPath, out _))
            {
                PKLogger.LogWarning(
                    $"Failed to add {assetPath} to assets with group {group.name} with entry {path}, because which is already exist.");
                return;
            }

            var asset = Settings.GetAsset(assetPath);
            asset.entry = path;
            asset.group = group;
            asset.addressMode = group.addressMode;
            assets.Add(asset);
            _assetDict.Add(assetPath, asset);
        }

        public void Clear()
        {
            assets.Clear();
            _assetDict.Clear();
        }
    }

    //条目信息缓存
    public class BuildEntryCache : ScriptableObject, ISerializationCallbackReceiver
    {
        public static readonly string Filename = $"Assets/PKFramework/Editor/BuildAB/Config/{nameof(BuildEntryCache)}.asset";
        public List<BuildEntry> data = new List<BuildEntry>();
        private readonly Dictionary<string, BuildEntry> _data = new Dictionary<string, BuildEntry>();


        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            foreach (var asset in data)
            {
                _data[asset.path] = asset;
                asset.OnAfterDeserialize();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param> 该entry的path
        /// <param name="group"></param> 所属的group
        /// <returns></returns>
        public BuildEntry GetEntry(string path, BuildGroup group)
        {
            if (!_data.TryGetValue(path, out var value))
            {
                value = new BuildEntry {path = path};
                _data[path] = value;
                data.Add(value);
                
                UpdateEntry(value, group);
                EditorUtility.SetDirty(this);
            }

            return value;
        }

        /// <summary>
        /// 更新该缓存信息
        /// </summary>
        /// <param name="value"></param>
        /// <param name="group"></param>
        private static void UpdateEntry(BuildEntry value, BuildGroup group)
        {
            value.group = group;

            //为目录则查找目录
            if (Directory.Exists(value.path))
            {
                var guilds = AssetDatabase.FindAssets(group.filter, new[]
                {
                    value.path
                });
                
                //资产的路径set
                var set = new HashSet<string>();
                var exclude = Settings.GetDefaultSettings().bundleSetting.excludeFiles;
                
                foreach (var guild in guilds)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guild);
                    if (string.IsNullOrEmpty(assetPath) || exclude.Exists(assetPath.EndsWith)
                                                        || Directory.Exists(assetPath)
                                                        || set.Contains(assetPath))
                        continue;
                    set.Add(assetPath);
                    value.AddAsset(assetPath);
                }
            }
            else //为文件则直接添加
            {
                if (string.IsNullOrEmpty(value.path))
                    return;
                value.AddAsset(value.path);
            }
        }
    }
}