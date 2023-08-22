using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using PKFramework.Editor.BuildAsset;

namespace PKFramework.Editor.Extension
{
    public static class AssetBuildMenu
    {
        [MenuItem("FrameworkTools/Build/Build Bundles", false, 100)]
        public static void BuildBundles()
        {
            Builder.BuildBundles(Selection.GetFiltered<Build>(SelectionMode.DeepAssets));
        }

        [MenuItem("FrameworkTools/Build/Build Bundles with Last Build", false, 100)]
        public static void BuildBundlesWithLastBuild()
        {
            Builder.BuildBundlesWithLastBuild(Selection.GetFiltered<Build>(SelectionMode.DeepAssets));
        }
        
        [MenuItem("FrameworkTools/Clear/Clear Download", false, 200)]
        public static void ClearDownload()
        {
            var directory = Application.persistentDataPath;
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);
        }

        [MenuItem("FrameworkTools/Clear/Clear Bundles", false, 200)]
        public static void ClearBundles()
        {
            var dirs = new[]
            {
                Settings.PlatformDataPath,
                Settings.PlatformCachePath,
            };

            foreach (var dir in dirs)
                if (Directory.Exists(dir))
                    Directory.Delete(dir, true);
        }

        [MenuItem("FrameworkTools/Clear/Clear History", false, 200)]
        public static void ClearBuildHistory()
        {
            ClearHistory.Start();
        }
    }
}