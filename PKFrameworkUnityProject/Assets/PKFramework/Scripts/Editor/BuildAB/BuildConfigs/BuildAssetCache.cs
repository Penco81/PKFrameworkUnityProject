using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PKFramework.Editor.BuildAsset
{
    //BuildAsset的缓存
    public class BuildAssetCache : ScriptableObject, ISerializationCallbackReceiver
    {
        public static readonly string Filename = $"Assets/PKFramework/Editor/BuildAB/Config/{nameof(BuildAssetCache)}.asset";
        
        //所有的资产信息
        public List<BuildAsset> data = new List<BuildAsset>();
        
        //路径-资产的字典
        private readonly Dictionary<string, BuildAsset> _data = new Dictionary<string, BuildAsset>();

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            foreach (var asset in data) 
                _data[asset.path] = asset;
        } 
        
        /// <summary>
        /// 根据path获取asset
        /// </summary>
        /// <param name="path"></param>路径
        /// <returns></returns>
        public BuildAsset GetAsset(string path)
        {
            if (!_data.TryGetValue(path, out var value))
            {
                //获取类型
                var type = AssetDatabase.GetMainAssetTypeAtPath(path);
                value = new BuildAsset
                {
                    path = path,
                    type = type == null ? "MissType" : type.Name
                };
                _data[path] = value;
                data.Add(value);
                EditorUtility.SetDirty(this);
            }

            BuildDependenciesIfNeed(value);

            return value;
        }

        /// <summary>
        /// 获取资产大小，单位字节
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static ulong GetAssetSize(string path)
        {
            var file = new FileInfo(path);
            return (ulong) (file.Exists ? file.Length : 0);
        }

        /// <summary>
        /// 生成依赖信息
        /// </summary>
        /// <param name="asset"></param>
        private void BuildDependenciesIfNeed(BuildAsset asset)
        {
            if (BuildUtils.CheckReferences(asset))
            {
                asset.dependencies = Settings.GetDependenciesWithoutCache(asset.path);
                EditorUtility.SetDirty(this);
            }

            GetAssetSize(asset.path);
        }

        
        /// <summary>
        /// 获取依赖
        /// </summary>
        /// <param name="path"></param> 资源路径
        /// <returns></returns>
        public string[] GetDependencies(string path)
        {
            return GetAsset(path).dependencies;
        }
    }
}