using System.Collections.Generic;
using PKFramework.Runtime.Task;
using UnityEngine;

namespace PKFramework.Runtime.Asset
{
    /// <summary>
    /// 一个资源对应的依赖项
    /// </summary>
    public class Dependencies
    {
        //所有依赖的包
        private readonly List<LoadBundleTask> _bundles = new List<LoadBundleTask>();
        private readonly List<LoadBundleTask> _loading = new List<LoadBundleTask>();
        private ManifestAsset manifestAsset { get; set; }
        //该资源所在的包
        private LoadBundleTask bundleTask;
        private int _refCount;
        public bool isDone => _loading.Count == 0;
        public float progress => (_bundles.Count - _loading.Count) * 1f / _bundles.Count;

        private LoadBundleTask Load(ManifestBundle bundle)
        {
            var task = LoadBundleTask.Load(bundle);
            _bundles.Add(task);
            _loading.Add(task);
            return task;
        }

        private void LoadAsync()
        {
            if (_refCount == 0)
            {
                var bundles = manifestAsset.manifest.bundles;
                var bundle = bundles[manifestAsset.bundle];
                bundleTask = Load(bundle);
                foreach (var dep in bundle.deps)
                    Load(bundles[dep]);
            }

            _refCount++;
        }

        public bool CheckResult(LoadAssetTask task, out AssetBundle assetBundle)
        {
            assetBundle = null;
            foreach (var bundle in _bundles)
            {
                if (bundle.Result != TaskBase.TaskResult.Failed) 
                    continue;
                task.SetResult(TaskBase.TaskResult.Failed, bundle.Error);
                return false;
            }

            assetBundle = bundleTask.assetBundle;
            if (assetBundle != null) 
                return true;
            task.SetResult(TaskBase.TaskResult.Failed, "assetBundle == null");
            return false;
        }

        public void WaitForCompletion()
        {
            for (var index = 0; index < _loading.Count; index++)
            {
                var task = _loading[index];
                task.WaitForCompletion();
                _loading.RemoveAt(index);
                index--;
            }
        }

        public void Release()
        {
            _refCount--;
            if (_refCount != 0) return;

            ReleaseAssetLoader.dependenceLoaded.Remove(manifestAsset.path);

            foreach (var task in _bundles) 
                task.Release();

            _bundles.Clear();
            bundleTask = null;
        }

        public static Dependencies LoadAsync(ManifestAsset asset)
        {
            if (!ReleaseAssetLoader.dependenceLoaded.TryGetValue(asset.path, out var value))
            {
                value = new Dependencies ();
                value.manifestAsset = asset;
                ReleaseAssetLoader.dependenceLoaded[asset.path] = value;
            }

            value.LoadAsync();
            return value;
        }

        public void Update()
        {
            if (isDone) 
                return;
            for (var index = 0; index < _loading.Count; index++)
            {
                var task = _loading[index];
                if (!task.IsDone) 
                    continue;
                _loading.RemoveAt(index);
                index--;
            }
        }
    }
}