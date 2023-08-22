using System;
using System.Collections.Generic;
using PKFramework.Runtime.Task;
using UnityEngine;

namespace PKFramework.Runtime.Asset
{
    public sealed class LoadBundleTask : LoadTaskBase
    {
        internal IBundleHandler handler;
        internal AssetBundle assetBundle { get; set; }
        public ManifestBundle info { get; set; }

        protected override void OnStart()
        {
            handler.OnStart(this);
        }

        protected override void OnUpdated()
        {
            handler.Update(this);
        }

        protected override void OnWaitForCompletion()
        {
            handler.WaitForCompletion(this);
        }

        public void LoadAssetBundle(string filename)
        {
            ReloadAssetBundle(info.name);
            assetBundle = AssetBundle.LoadFromFile(filename);
            Progress = 1;
            if (assetBundle == null)
            {
                SetResult(TaskResult.Failed, $"assetBundle == null, {info.file}");
                return;
            }

            SetResult(TaskResult.Success);
            AddAssetBundle(info.name, assetBundle);
        }

        protected override void OnDispose()
        {
            Remove(this);
            handler.Dispose(this);
            if (assetBundle != null)
            {
                assetBundle.Unload(true);
                RemoveAssetBundle(info.name);
                assetBundle = null;
            }
        }

        #region Hotreload

        private static readonly Dictionary<string, AssetBundle> AssetBundles = new Dictionary<string, AssetBundle>();

        private static void AddAssetBundle(string name, AssetBundle assetBundle)
        {
            AssetBundles[name] = assetBundle;
        }

        private static void ReloadAssetBundle(string name)
        {
            if (!AssetBundles.TryGetValue(name, out var assetBundle)) return;
            if (assetBundle != null) assetBundle.Unload(false);
            AssetBundles.Remove(name);
        }

        private static void RemoveAssetBundle(string name)
        {
            AssetBundles.Remove(name);
        }

        #endregion

        #region Internal
        

        private static IBundleHandler GetHandler(LoadBundleTask task)
        {
            var bundle = task.info;

            if (AssetUtils.IsPlayerAsset(bundle.hash))
                return new RuntimeLocalBundleHandler {path = AssetUtils.GetPlayerDataPath(bundle.file)};

            if (AssetUtils.IsDownloaded(bundle))
                return new RuntimeLocalBundleHandler {path = AssetUtils.GetDownloadDataPathWithFileName(bundle.file)};

            return new RuntimeDownloadBundleHandler();
        }

        private static void Remove(LoadBundleTask request)
        {
            ReleaseAssetLoader.bundleLoaded.Remove(request.info.file);
        }

        internal static LoadBundleTask Load(ManifestBundle bundle)
        {
            if (!ReleaseAssetLoader.bundleLoaded.TryGetValue(bundle.file, out var task))
            {
                task = TaskManager.Instance.CreateTask<LoadBundleTask>();
                task.Reset();
                task.info = bundle;
                task.Path = bundle.file;
                task.handler = GetHandler(task);
                ReleaseAssetLoader.bundleLoaded[bundle.file] = task;
            }

            return task;
        }

        #endregion
    }
}