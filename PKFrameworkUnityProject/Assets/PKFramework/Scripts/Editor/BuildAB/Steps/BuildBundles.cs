using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PKFramework.Runtime.Asset;
using UnityEditor;
using UnityEngine;

namespace PKFramework.Editor.BuildAsset
{
    /// <summary>
    /// 使用Unity API开始构建Ab到缓存目录，并且复制到数据目录
    /// </summary>
    public class BuildBundles : IBuildJobStep
    {
        public void Start(BuildJob job)
        {
            if (job.bundledAssets.Count <= 0) return;
            BuildBundlesWithBundledAssets(job);
        }

        private static AssetBundleManifest BuildAssetBundles(BuildJob job)
        {
            return BuildUtils.BuildAssetBundles(Settings.PlatformCachePath, job.bundles.ConvertAll(
                    input =>
                        new AssetBundleBuild
                        {
                            assetNames = input.assets.ToArray(),
                            assetBundleName = input.group
                        }).ToArray(), job.parameters.options, EditorUserBuildSettings.activeBuildTarget);
            
        }

        private static void BuildBundlesWithBundledAssets(BuildJob job)
        {
            //bundleName-buildBundle的字典
            var nameWithBundles = job.bundles.ToDictionary(o => o.group);
            foreach (var asset in job.bundledAssets)
            {
                if (!nameWithBundles.TryGetValue(asset.bundle, out var bundle))
                {
                    var id = job.bundles.Count;
                    bundle = new BuildBundle
                    {
                        id = id,
                        group = asset.bundle,
                        assets = new List<string>()
                    };
                    job.bundles.Add(bundle);
                    nameWithBundles[asset.bundle] = bundle;
                }

                bundle.assets.Add(asset.path);
            }

            var manifest = BuildAssetBundles(job);

            if (manifest == null)
            {
                job.TreatError($"Failed to build AssetBundles with {job.parameters.name}.");
                return;
            }

            var settings = Settings.GetDefaultSettings().bundleSetting;

            var assetBundles = manifest.GetAllAssetBundles();
            foreach (var assetBundle in assetBundles)
                if (nameWithBundles.TryGetValue(assetBundle, out var bundle))
                {
                    var path = Settings.GetCachePath(assetBundle);
                    bundle.deps = Array.ConvertAll(manifest.GetAllDependencies(assetBundle),
                        input => nameWithBundles[input].id);
                    var info = new FileInfo(path);
                    if (info.Exists)
                    {
                        var hash = AssetUtils.ComputeHash(path);
                        var nameWithAppendHash = $"{hash}{Settings.extension}";
                        if (settings.saveBundleName)
                        {
                            var name = assetBundle.Replace(Settings.extension, string.Empty);
                            nameWithAppendHash = $"{name}_{nameWithAppendHash}";
                        }

                        bundle.size = (ulong) info.Length;
                        bundle.hash = hash;
                        bundle.file = nameWithAppendHash;
                        var newPath = Settings.GetDataPath(bundle.file);
                        if (File.Exists(newPath)) 
                            continue;
                        AssetUtils.CreateDirectoryIfNecessary(newPath);
                        //将包从缓存目录复制到资产的目录
                        File.Copy(path, newPath, true);
                    }
                    else
                    {
                        job.TreatError($"File not found: {info}");
                        return;
                    }
                }
                else
                {
                    job.TreatError($"Bundle not found: {assetBundle}");
                    return;
                }
        }
    }
}