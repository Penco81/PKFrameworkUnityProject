using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using PKFramework.Runtime.Asset;
using UnityEditor;
using UnityEngine;

namespace PKFramework.Editor.BuildAsset
{
    //打包入口
    public static class Builder
    {
        //构建前的回调
        public static Action<Build[], Settings> PreprocessBuildBundles { get; set; } = null;
        
        //构建后的回调
        public static Action<string[]> PostprocessBuildBundles { get; set; } = null;

        //打包
        public static void BuildBundles(params Build[] builds)
        {
            BuildBundlesInternal(false, builds);
        }

        //根据上一次的信息重新打包
        public static void BuildBundlesWithLastBuild(params Build[] builds)
        {
            BuildBundlesInternal(true, builds);
        }
        
        //清除构建相关的cache
        private static void ClearBuildAssetCache()
        {
            // 资源依赖发生修改的时候需要重新生成依赖关系。
            var deleted = new[]
            {
               BuildAssetCache.Filename,
               BuildEntryCache.Filename
            };
            foreach (var file in deleted)
            {
                if (!File.Exists(file)) continue;
                File.Delete(file);
                File.Delete(file + ".meta");
                PKLogger.LogMessage($"delete cache {file}");
            }
        }

        /// <summary>
        /// 开启构建任务
        /// </summary>
        /// <param name="withLastBuild"></param>
        /// <param name="builds"></param>
        private static void BuildBundlesInternal(bool withLastBuild, params Build[] builds)
        {
            ClearBuildAssetCache();
            var settings = Settings.GetDefaultSettings();
            
            //如果选择的Build为空，则搜集所有的Build
            if (builds.Length == 0) 
                builds = BuildUtils.FindAssets<Build>();
            
            PreprocessBuildBundles?.Invoke(builds, settings);
            
            if (FindMultipleReferences())
            {
                PKLogger.LogError("FindReferences is true");
                return;
            }

            CreateDirectories();

            var buildPaths = Array.ConvertAll(builds, AssetDatabase.GetAssetPath);

            if (buildPaths.Length == 0)
            {
                return;
            }

            var watch = new Stopwatch();
            watch.Start();
            var changes = new List<string>();
            //每个Build执行一次构建管线
            foreach (var buildPath in buildPaths)
            {
                var build = AssetDatabase.LoadAssetAtPath<Build>(buildPath);
                var parameters = build.parameters;
                parameters.name = build.name;

                BuildJob job = null;
                //如果是从上一次打包开始打包，则流程不同
                if (withLastBuild)
                {
                    job = BuildJob.StartNew(parameters, new LoadBuildAssets(), new BuildBundles(),
                        new BuildVersions());
                }
                else
                {
                    if (parameters.optimizeDependentAssets)
                    {
                        job = BuildJob.StartNew(parameters, new CollectAssets(), new ClearDuplicateAssets(),
                            new OptimizeDependentAssets(),
                            new SaveBuildAssets(), new BuildBundles(), new BuildVersions());
                    }
                    else
                    {
                        job = BuildJob.StartNew(parameters, new CollectAssets(), new ClearDuplicateAssets(),
                            new SaveBuildAssets(), new BuildBundles(), new BuildVersions());
                    }
                }

                if (!string.IsNullOrEmpty(job.error))
                {
                    PKLogger.LogError(job.error);
                }
                else
                {
                    if (job.changes.Count <= 0) continue;
                    foreach (var change in job.changes) 
                        changes.Add(Settings.GetDataPath(change));
                }
            }

            watch.Stop();
            PKLogger.LogMessage($"Finish {nameof(BuildBundles)} with {watch.ElapsedMilliseconds / 1000f}s.");
            if (changes.Count <= 0) return;
            PostprocessBuildBundles?.Invoke(changes.ToArray());
            SaveVersions(changes);
        }

        private static void SaveVersions(List<string> changes)
        {
            var versions = Settings.GetDefaultVersions();
            var path = Settings.GetDataPath(versions.GetFilename());
            versions.Save(Settings.GetCachePath(Versions.Filename));
            versions.Save(path);
            changes.Add(path);
            PostprocessBuildBundles?.Invoke(changes.ToArray());
            var file = new FileInfo(path);
            
            BuildUpdateInfo(versions, AssetUtils.ComputeHash(path), file.Length);
            
            //存BuildRecord.json，存本次构建有变化的资源
            var size = GetChanges(changes.ToArray(), versions.GetFilename());
            var savePath = Settings.GetCachePath(BuildRecords.Filename);
            var records = AssetUtils.LoadOrCreateFromFile<BuildRecords>(savePath);
            records.Set(versions.GetFilename(), changes.ToArray(), size);
            var json = JsonUtility.ToJson(records);
            File.WriteAllText(savePath, json);
        }

        /// <summary>
        /// 存updateinfo.json
        /// </summary>
        /// <param name="versions"></param>
        /// <param name="hash"></param>
        /// <param name="size"></param>
        public static void BuildUpdateInfo(Versions versions, string hash, long size)
        {
            var updateInfoPath = Settings.GetCachePath(UpdateInfo.Filename);
            var updateInfo = AssetUtils.LoadOrCreateFromFile<UpdateInfo>(updateInfoPath);
            updateInfo.hash = hash;
            updateInfo.size = (ulong) size;
            updateInfo.file = versions.GetFilename();
            updateInfo.version = PlayerSettings.bundleVersion;
            File.WriteAllText(updateInfoPath, JsonUtility.ToJson(updateInfo));
        }

        /// <summary>
        /// 创建Bundle和BundleCache的目录，没有目录调用官方API打包会打包失败
        /// </summary>
        private static void CreateDirectories()
        {
            var directories = new List<string>
            {
                Settings.PlatformCachePath, 
                Settings.PlatformDataPath
            };

            foreach (var directory in directories)
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
        }

        /// <summary>
        /// 检查同一资产是否被多个Group引用
        /// </summary>
        /// <returns></returns>
        public static bool FindMultipleReferences()
        {
            var builds = BuildUtils.FindAssets<Build>();
            if (builds.Length == 0)
            {
                PKLogger.LogMessage("Nothing to build.");
                return false;
            }

            var assets = new List<BuildAsset>();
            foreach (var build in builds)
            {
                var item = build.parameters;
                item.name = build.name;
                var job = item.optimizeDependentAssets?
                    BuildJob.StartNew(item, new CollectAssets(), new OptimizeDependentAssets())
                    : BuildJob.StartNew(item, new CollectAssets());
                
                if (!string.IsNullOrEmpty(job.error)) return true;

                foreach (var asset in job.bundledAssets) 
                    assets.Add(asset);
            }
            
            //资产路径-[build-groupName] 的字典
            var assetWithGroups = new Dictionary<string, HashSet<string>>();
            foreach (var asset in assets)
            {
                if (!assetWithGroups.TryGetValue(asset.path, out var refs))
                {
                    refs = new HashSet<string>();
                    assetWithGroups.Add(asset.path, refs);
                }

                refs.Add($"{asset.group.build}-{asset.group.name}");
            }

            var sb = new StringBuilder();
            foreach (var pair in assetWithGroups)
            {
                if (pair.Value.Count > 1)
                {
                    sb.AppendLine(pair.Key);
                    foreach (var s in pair.Value) sb.AppendLine(" - " + s);
                }
            }

            var content = sb.ToString();
            if (string.IsNullOrEmpty(content))
            {
                PKLogger.LogMessage("Checking completed, Everything is ok.");
                return false;
            }

            const string filename = "MultipleReferences.txt";
            File.WriteAllText(filename, content);
            if (EditorUtility.DisplayDialog("提示", "检测到多重引用关系，是否打开查看？", "确定"))
                EditorUtility.OpenWithDefaultApp(filename);
            return true;
        }

        /// <summary>
        /// 获取需要在PlayerBuild时在StreamingPath下的Bundle
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="versions"></param>
        /// <returns></returns>
        public static ManifestBundle[] GetBundlesInBuild(Settings settings, Versions versions)
        {
            var set = new HashSet<ManifestBundle>();
            switch (settings.playerAssetsSplitMode)
            {
                case PlayerAssetsSplitMode.SplitByAssetPacksWithInstallTime:
                    if (EditorUtility.DisplayDialog("提示", "开源版本不提供分包机制，购买专业版可以解锁这个功能，立即前往？", "确定"))
                    {
                        //TODO
                    }

                    break;
                case PlayerAssetsSplitMode.ExcludeAllAssets:
                    break;
                case PlayerAssetsSplitMode.IncludeAllAssets:
                    foreach (var version in versions.data)
                    {
                        var path = Settings.GetDataPath(version.file);
                        var manifest = AssetUtils.LoadOrCreateFromFile<Manifest>(path);
                        set.UnionWith(manifest.bundles);
                    }

                    break;
            }

            return set.ToArray();
        }

        public static ulong GetChanges(IEnumerable<string> changes, string name)
        {
            var sb = new StringBuilder();
            var size = 0UL;
            var files = new List<FileInfo>();
            foreach (var change in changes)
            {
                var file = new FileInfo(change);
                if (!file.Exists) continue;
                size += (ulong) file.Length;
                files.Add(file);
            }

            files.Sort((a, b) => b.Length.CompareTo(a.Length));
            foreach (var file in files) 
                sb.AppendLine($"{file.FullName}({AssetUtils.FormatBytes((ulong) file.Length)})");

            PKLogger.LogMessage(size > 0
                ? $"GetChanges from {name} with following files({AssetUtils.FormatBytes(size)}):\n {sb}"
                : "Nothing changed.");
            return size;
        }
        
    }
}