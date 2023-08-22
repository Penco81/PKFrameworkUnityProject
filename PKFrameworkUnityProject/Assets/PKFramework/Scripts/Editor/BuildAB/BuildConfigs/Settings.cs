using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodiceApp.EventTracking;
using PKFramework.Runtime.Asset;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace PKFramework.Editor.BuildAsset
{
    //打包相关全局的设置
    [Serializable]
    public class BundleSettings
    {
        //保存包名还是纯hash名
        public bool saveBundleName = true;
        public bool splitBundleNameWithBuild = true;
        //对所有需要打包的场景按场景文件打包
        public bool packByFileForAllScenes = true;
        //所有一起shader打包
        public bool packTogetherForAllShaders = true;
        //bundle的扩展名
        public string extension = ".bundle";

        public List<string> shaderExtensions = new List<string>
            {".shader", ".shadervariants", ".compute"};
        
        [Tooltip("只能作为entry打包，不能作为entry目录项的子目录下打包")]
        public List<string> excludeFiles = new List<string>
        {
            ".cs",
            ".cginc",
            ".hlsl",
            ".spriteatlas",
            ".dll",
        };
    }
    
    /// <summary>
    /// 包含构建player，bundle，updateinfo等设置
    /// </summary>
    [CreateAssetMenu(fileName = nameof(Settings), menuName = "PKFramework/Bundles" + nameof(Settings))]
    public class Settings : ScriptableObject
    {

        public PlayerAssetsSplitMode playerAssetsSplitMode = PlayerAssetsSplitMode.IncludeAllAssets;

        public BundleSettings bundleSetting = new BundleSettings();
        private static string Filename => $"Assets/PKFramework/Editor/BuildAB/Config/{nameof(Settings)}.asset";

        public static BuildGroup GetAutoGroup()
        {
            var group = BuildUtils.GetOrCreateAsset<BuildGroup>($"Assets/PKFramework/Editor/BuildAB/Config/Auto.asset");
            group.bundleMode = BundleMode.PackByCustom;
            group.addressMode = AddressMode.LoadByDependencies;
            return group;
        }

        public PlayerAssets GetPlayerAssets()
        {
            var assets = CreateInstance<PlayerAssets>();
            assets.version = PlayerSettings.bundleVersion;
            return assets;
        }

        public static string PlatformCachePath
        {
            get
            {
                return $"{Environment.CurrentDirectory}/{AssetUtils.Bundles}Cache/{Platform}".Replace('\\', '/');
            }
        }


        public static string PlatformDataPath
        {
            get
            {
                return $"{Environment.CurrentDirectory.Replace('\\', '/')}/{AssetUtils.Bundles}/{Platform}";

            }
        }

        public static Platform Platform => BuildUtils.GetEditorPlatform();

        private static Settings defaultSettings;

        /// <summary>
        /// 获取settings实例
        /// </summary>
        /// <returns></returns>
        public static Settings GetDefaultSettings()
        {
            if (defaultSettings != null) return defaultSettings;
            var assets = BuildUtils.FindAssets<Settings>();
            defaultSettings = assets.Length > 0 ?
                assets[0]
                : BuildUtils.GetOrCreateAsset<Settings>(Filename);
            return defaultSettings;
        }

        public static string GetDataPath(string filename)
        {
            return $"{PlatformDataPath}/{filename}";
        }

        public static string GetCachePath(string filename)
        {
            return $"{PlatformCachePath}/{filename}";
        }
        
        public static Versions GetDefaultVersions()
        {
            var versions = AssetUtils.LoadOrCreateFromFile<Versions>(GetCachePath(Versions.Filename));
            return versions;
        }

        /// <summary>
        /// 获取上次访问文件或目录的时间戳
        /// </summary>
        /// <param name="path"></param>文件或目录的路径
        /// <returns></returns>
        public static long GetLastWriteTime(string path)
        {
            var file = new FileInfo(path);
            return file.Exists ? file.LastAccessTime.ToFileTime() : 0;
        }

        public static string[] GetDependenciesWithoutCache(string assetPath)
        {
            var set = new HashSet<string>();
            set.UnionWith(AssetDatabase.GetDependencies(assetPath));
            set.Remove(assetPath);
            var exclude = GetDefaultSettings().bundleSetting.excludeFiles;
            // Unity 会存在场景依赖场景的情况。
            set.RemoveWhere(s => s.EndsWith(".unity") || exclude.Exists(s.EndsWith));
            return set.ToArray();
        }

        public static BundleSettings BundleSettingSettings => GetDefaultSettings().bundleSetting;
        public static string extension => BundleSettingSettings.extension;

        /// <summary>
        ///     [path, entry, bundle, group, return(bundle)]
        /// </summary>
        public static Func<string, string, string, BuildGroup, string> customPacker { get; set; } = null;

        public static Func<string, bool> customFilter { get; set; } = s => true;

        public static string PackAsset(BuildAsset asset)
        {
            var assetPath = asset.path;
            var group = asset.group;
            
            //if (group == null)
            //{
            //    asset.addressMode = AddressMode.LoadByDependencies;
            //    return "auto";
            //}

            var entry = asset.entry;
            var bundle = group.name.ToLower();
            
            //eg1: 
            //assetPath = C:/MyDir/MySubDir/myfile.ext
            //entry = C:/MyDir/MySubDir
            //after:
            //assetPath = MySubDir/myfile.ext
            //entry = MySubDir
            
            //eg2: 
            //assetPath = C:/MyDir/MySubDir/myfile.ext
            //entry = C:/MyDir/MySubDir/myfile.ext
            //after:
            //assetPath = myfile.ext
            //entry = myfile.ext
            
            var dir = AssetUtils.GetDirectoryName(entry) + "/";
            //entry做为根路径下的资源路径
            assetPath = assetPath.Replace(dir, "");
            entry = entry.Replace(dir, "");

            switch (group.bundleMode)
            {
                case BundleMode.PackTogether:
                    bundle = group.name;
                    break;
                case BundleMode.PackByFolder:
                    bundle = AssetUtils.GetDirectoryName(assetPath);
                    break;
                case BundleMode.PackByFile:
                    bundle = assetPath;
                    break;
                case BundleMode.PackByEntry:
                    bundle = Path.GetFileNameWithoutExtension(entry);
                    break;
                case BundleMode.PackByCustom:
                    if (customPacker != null) bundle = customPacker?.Invoke(assetPath, entry, bundle, group);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return PackAsset(assetPath, bundle, group.build);
        }

        public static string PackAsset(string assetPath, string bundle, string build)
        {
            var settings = GetDefaultSettings().bundleSetting;

            if (settings.packTogetherForAllShaders && settings.shaderExtensions.Exists(assetPath.EndsWith))
                bundle = "shaders";

            if (settings.packByFileForAllScenes && assetPath.EndsWith(".unity"))
                bundle = assetPath;

            bundle = $"{BuildUtils.FixedName(bundle)}{settings.extension}";

            if (!string.IsNullOrEmpty(build) && settings.splitBundleNameWithBuild)
                return $"{build.ToLower()}/{bundle}";

            return $"{bundle}";
        }

        /// <summary>
        /// 获取资产的信息
        /// </summary>
        /// <param name="path"></param> 资产路径
        /// <returns></returns>
        public static BuildAsset GetAsset(string path)
        {
            return GetBuildAssetCache().GetAsset(path);
        }

        
        /// <summary>
        /// 获取资产的依赖
        /// </summary>
        /// <param name="assetPath"></param> 资产路径
        /// <returns></returns>
        public static string[] GetDependencies(string assetPath)
        {
            return GetBuildAssetCache().GetDependencies(assetPath);
        }
        
        private static BuildAssetCache BuildAssetCache;
        public static BuildAssetCache GetBuildAssetCache()
        {
            if (BuildAssetCache == null)
            {
                BuildAssetCache = BuildUtils.GetOrCreateAsset<BuildAssetCache>(BuildAssetCache.Filename);
            }

            return BuildAssetCache;
        }

        private static BuildEntryCache BuildEntryCache;

        public static BuildEntryCache GetBuildEntryCache()
        {
            if (BuildEntryCache == null)
            {
                BuildEntryCache = BuildUtils.GetOrCreateAsset<BuildEntryCache>(BuildEntryCache.Filename);
            }

            return BuildEntryCache;
        }
        
        /// <summary>
        /// 收集该group下的资产
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public static BuildAsset[] Collect(BuildGroup group)
        {
            var assets = new List<BuildAsset>();
            if (group.entries == null) return assets.ToArray();
            foreach (var entry in group.entries)
            {
                GetAssets(group, entry, assets);
            }

            return assets.ToArray();
        }

        private static void GetAssets(BuildGroup group, Object entry, List<BuildAsset> assets)
        {
            var path = AssetDatabase.GetAssetPath(entry);
            if (string.IsNullOrEmpty(path)) return;
            var cache = GetBuildEntryCache();
            var entryAsset = cache.GetEntry(path, group);
            assets.AddRange(entryAsset.assets);
        }
    }
}