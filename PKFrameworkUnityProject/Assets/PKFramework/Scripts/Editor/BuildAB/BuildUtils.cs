using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using PKFramework.Runtime.Asset;
using UnityEditor;
using UnityEngine;

namespace PKFramework.Editor.BuildAsset
{
    //构建用到的工具类
    public static class BuildUtils
    {
        
        
        /// <summary>
        /// 内置构建任务API
        /// </summary>
        /// <param name="outputPath"></param> 输出路径
        /// <param name="builds"></param>
        /// <param name="options"></param> 构建选项
        /// <param name="target"></param> 目标平台
        /// <returns></returns>
        public static AssetBundleManifest BuildAssetBundles(string outputPath, AssetBundleBuild[] builds,
            BuildAssetBundleOptions options, BuildTarget target)
        {
            var manifest = BuildPipeline.BuildAssetBundles(outputPath, builds, options, target);
            return manifest;
        }
        
        /// <summary>
        /// 获取当前平台
        /// </summary>
        /// <returns></returns>
        public static Platform GetEditorPlatform()
        {
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                    return Platform.Android;
                case BuildTarget.StandaloneOSX:
                    return Platform.OSX;
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return Platform.Windows;
                case BuildTarget.iOS:
                    return Platform.iOS;
                case BuildTarget.StandaloneLinux64:
                    return Platform.Linux;
                default:
                    return Platform.Default;
            }
        }
        
        //获取文件上次修改时间
        public static long GetLastWriteTime(string path)
        {
            var file = new FileInfo(path);
            return file.Exists ? file.LastAccessTime.ToFileTime() : 0;
        }
        
        //寻找项目资产目录下的ScriptableObject
        public static T[] FindAssets<T>() where T : ScriptableObject
        {
            var builds = new List<T>();
            var guilds = AssetDatabase.FindAssets("t:" + typeof(T).FullName);
            foreach (var guild in guilds)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guild);
                if (string.IsNullOrEmpty(assetPath)) continue;

                var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset == null) continue;

                builds.Add(asset);
            }

            return builds.ToArray();
        }

        public static string FixedName(string bundle)
        {
            return bundle.Replace(" ", "").Replace("/", "_").Replace("-", "_").Replace(".", "_").ToLower();
        }
        
        /// <summary>
        /// 检查是否有引用关系
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static bool CheckReferences(BuildAsset asset)
        {
            var type = asset.type;
            if (type == null) 
                return false;
            return !type.Contains("TextAsset") && !type.Contains("Texture");
        }

        public static T GetOrCreateAsset<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) return asset;
            AssetUtils.CreateDirectoryIfNecessary(path);
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }
    }

}
