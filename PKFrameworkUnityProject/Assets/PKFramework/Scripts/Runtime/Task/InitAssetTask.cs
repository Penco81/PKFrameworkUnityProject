using System;
using System.Collections.Generic;
using System.IO;
using PKFramework.Runtime.Asset;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;
using Version = PKFramework.Runtime.Asset.Version;

namespace PKFramework.Runtime.Task
{
    /// <summary>
    /// 初始化资源配置的Task，
    /// </summary>
    public class InitAssetTask : TaskBase
    {
        /// <summary>
        /// 顺序
        /// </summary>
        private enum Step
        {
            LoadPlayerAssets,
            LoadVersionsHeader,
            LoadPlayerVersions,
            LoadVersionsContent
        }
        
        private Queue<Version> _queue;
        private List<UnityWebRequest> _requests;
        private Versions _downloadVersions;
        private Step _step;
        private UnityWebRequest _unityWebRequest;

        protected override void OnCompleted()
        {
            _step = Step.LoadPlayerVersions;
            _queue = new Queue<Version>();
            _requests = new List<UnityWebRequest>();
            
            _unityWebRequest = UnityWebRequest.Get(AssetUtils.GetPlayerDataURl(PlayerAssets.Filename));
            _unityWebRequest.SendWebRequest();
            _step = Step.LoadPlayerAssets;
        }

        protected override void OnUpdated()
        {
            switch (_step)
            {
                case Step.LoadPlayerAssets:
                    UpdateLoadingPlayerAssets();
                    break;
                case Step.LoadVersionsHeader:
                    UpdateLoadVersionsHeader();
                    break;
                case Step.LoadPlayerVersions:
                    UpdateLoadPlayerVersions();
                    break;
                case Step.LoadVersionsContent:
                    UpdateLoadVersions();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void LoadVersionsHeader(string url)
        {
            _unityWebRequest = UnityWebRequest.Get(url);
            _unityWebRequest.SendWebRequest();
            _step = Step.LoadVersionsHeader;
        }

        private void UpdateLoadVersions()
        {
            while (_queue.Count > 0)
            {
                var version = _queue.Dequeue();
                var path = AssetUtils.GetDownloadDataPathWithFileName(version.file);
                var manifest = AssetUtils.LoadOrCreateFromFile<Manifest>(path);
                manifest.build = version.name;
                manifest.name = version.file;
                version.manifest = manifest;
                foreach (var asset in manifest.assets)
                {
                    switch (asset.addressMode)
                    {
                        case AddressMode.LoadByDependencies:
                        case AddressMode.LoadByPath:
                            break;
                        case AddressMode.LoadByName:
                            AssetUtils.SetAddress(asset.path, Path.GetFileName(asset.path));
                            break;
                        case AddressMode.LoadByNameWithoutExtension:
                            AssetUtils.SetAddress(asset.path, Path.GetFileNameWithoutExtension(asset.path));
                            break;
                    }
                }

                //TODO
                //if (Scheduler.Busy) return;
            }

            SetResult(TaskResult.Success);
        }


        private void UpdateLoadPlayerVersions()
        {
            for (var index = 0; index < _requests.Count; index++)
            {
                var unityWebRequest = _requests[index];
                if (!unityWebRequest.isDone) return;
                _requests.RemoveAt(index);
                index--;
                if (!string.IsNullOrEmpty(unityWebRequest.error))
                {
                    TreatError(unityWebRequest);
                }

                unityWebRequest.Dispose();
            }

            var path = AssetUtils.GetDownloadDataPathWithFileName(Versions.Filename);
            _downloadVersions = AssetUtils.LoadOrCreateFromFile<Versions>(path);
            if (_downloadVersions != null && _downloadVersions.IsNew(AssetUtils.Versions))
                AssetUtils.Versions = _downloadVersions;
            foreach (var version in AssetUtils.Versions.data)
                _queue.Enqueue(version);
            _step = Step.LoadVersionsContent;
        }

        private void TreatError(UnityWebRequest unityWebRequest)
        {
            SetResult(TaskResult.Failed, unityWebRequest.error);
            PKLogger.LogError($"Failed to load {unityWebRequest.url} with error {unityWebRequest.error}");
        }

        private void UpdateLoadingPlayerAssets()
        {
            if (!_unityWebRequest.isDone) return;
            if (!string.IsNullOrEmpty(_unityWebRequest.error))
            {
                SetResult(TaskResult.Failed, _unityWebRequest.error);
                return;
            }

            AssetUtils.PlayerAssets = AssetUtils.LoadOrCreateFromFile<PlayerAssets>(_unityWebRequest.downloadHandler.text);

            // TODO: 这里在正式环境，可以在初始化之后，自己重写 UpdateInfoURL 的地址。
            AssetUtils.UpdateInfoURL = AssetUtils.PlayerAssets.updateInfoURL;
            AssetUtils.DownloadURL = AssetUtils.PlayerAssets.downloadURL;

            _unityWebRequest.Dispose();
            LoadVersionsHeader(AssetUtils.GetPlayerDataURl(Versions.Filename));
        }

        private void UpdateLoadVersionsHeader()
        {
            if (!_unityWebRequest.isDone) return;
            if (!string.IsNullOrEmpty(_unityWebRequest.error))
            {
                TreatError(_unityWebRequest);
                return;
            }

            var json = _unityWebRequest.downloadHandler.text;
            AssetUtils.Versions = AssetUtils.LoadOrCreateFromFile<Versions>(json);
            _unityWebRequest.Dispose();
            foreach (var version in AssetUtils.Versions.data)
            {
                if (AssetUtils.IsDownloaded(version)) continue;
                var url = AssetUtils.GetPlayerDataURl(version.file);
                var savePath = AssetUtils.GetDownloadDataPathWithFileName(version.file);
                var unityWebRequest = UnityWebRequest.Get(url);
                unityWebRequest.downloadHandler = new DownloadHandlerFile(savePath);
                unityWebRequest.SendWebRequest();
                _requests.Add(unityWebRequest);
            }

            _step = Step.LoadPlayerVersions;
        }
    }
}