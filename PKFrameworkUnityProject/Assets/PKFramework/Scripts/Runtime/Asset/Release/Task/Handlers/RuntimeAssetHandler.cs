using System;
using PKFramework.Runtime.Asset;
using PKFramework.Runtime.Task;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using xasset;

namespace PKFramework.Runtime.Asset
{
    public interface IAssetHandler
    {
        void OnStart(LoadAssetTask task);
        void Update(LoadAssetTask task);
        void Dispose(LoadAssetTask task);
        void WaitForCompletion(LoadAssetTask task);
    }

    public class RuntimeAssetHandler : IAssetHandler
    {
        private enum Step
        {
            LoadDependencies,
            LoadAsset
        }

        private Dependencies _dependencies;
        private AssetBundleRequest _loadAssetAsync;
        private Step _step;

        public void OnStart(LoadAssetTask task)
        {
            _dependencies = Dependencies.LoadAsync(task.info);
            _step = Step.LoadDependencies;
        }

        public void Update(LoadAssetTask task)
        {
            switch (_step)
            {
                case Step.LoadDependencies:
                    _dependencies.Update();
                    task.Progress = _dependencies.progress * 0.5f;
                    if (!_dependencies.isDone) return;
                    LoadAssetAsync(task);
                    break;

                case Step.LoadAsset:
                    task.Progress = 0.5f + _loadAssetAsync.progress * 0.5f;
                    if (!_loadAssetAsync.isDone) return;
                    SetResult(task);
                    break;
                default:
                    throw new Exception("RuntimeAssetHandle Wrong");
            }
        }

        private void LoadAssetAsync(LoadAssetTask task)
        {
            if (!_dependencies.CheckResult(task, out var assetBundle))
                return;

            var type = task.type;
            var path = task.Path;
            _loadAssetAsync = task.isAll
                ? assetBundle.LoadAssetWithSubAssetsAsync(path, type)
                : assetBundle.LoadAssetAsync(path, type);
            _step = Step.LoadAsset;
        }

        private void SetResult(LoadAssetTask task)
        {
            if (task.isAll)
            {
                task.assets = _loadAssetAsync.allAssets;
                if (task.assets == null)
                {
                    task.SetResult(TaskBase.TaskResult.Failed, "assets == null");
                    return;
                }
            }
            else
            {
                task.asset = _loadAssetAsync.asset;
                if (task.asset == null)
                {
                    task.SetResult(TaskBase.TaskResult.Failed, "asset == null");
                    return;
                }
            }

            task.SetResult(TaskBase.TaskResult.Success);
        }

        public void Dispose(LoadAssetTask task)
        {
            _dependencies.Release();
            _loadAssetAsync = null;
        }

        public void WaitForCompletion(LoadAssetTask task)
        {
            _dependencies.WaitForCompletion();
            if (task.Result == TaskBase.TaskResult.Failed) return;
            if (_loadAssetAsync == null) LoadAssetAsync(task);
            if (task.Result == TaskBase.TaskResult.Failed) return;
            SetResult(task);
        }

        public static IAssetHandler CreateInstance()
        {
            return new RuntimeAssetHandler();
        }
    }
}