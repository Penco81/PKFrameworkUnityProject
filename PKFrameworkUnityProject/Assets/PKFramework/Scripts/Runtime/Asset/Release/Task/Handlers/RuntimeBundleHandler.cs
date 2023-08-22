using PKFramework.Runtime.Task;
using UnityEngine;

namespace PKFramework.Runtime.Asset
{
    public interface IBundleHandler
    {
        void OnStart(LoadBundleTask task);
        void Update(LoadBundleTask task);
        void Dispose(LoadBundleTask task);
        void WaitForCompletion(LoadBundleTask task);
    }
    
    /// <summary>
    /// 本地包加载
    /// </summary>
    internal class RuntimeLocalBundleHandler : IBundleHandler
    {
        public string path;

        public void OnStart(LoadBundleTask task)
        {
            task.LoadAssetBundle(path);
        }

        public void Update(LoadBundleTask task)
        {
        }

        public void Dispose(LoadBundleTask task)
        {
        }

        public void WaitForCompletion(LoadBundleTask task)
        {
        }
    }

    /// <summary>
    /// 需要下载的包加载
    /// </summary>
    internal class RuntimeDownloadBundleHandler : IBundleHandler
    {
        private DownloadRequest _downloadAsync;
        private string _savePath;
        private int _retryTimes;

        public void OnStart(LoadBundleTask request)
        {
            _retryTimes = 0;
            var bundle = request.info;
            var url = AssetUtils.GetDownloadURL(bundle.file);
            _savePath = AssetUtils.GetDownloadDataPathWithFileName(bundle.file);
            _downloadAsync = Downloader.DownloadAsync(DownloadContent.Get(url, _savePath, bundle.hash, bundle.size));
        }

        public void Update(LoadBundleTask request)
        {
            request.Progress = _downloadAsync.progress;
            if (!_downloadAsync.isDone)
                return;

            if (_downloadAsync.result == DownloadRequestBase.Result.Success)
            {
                request.LoadAssetBundle(_savePath);
                return;
            }

            // 网络可达才自动 Retry
            if (Application.internetReachability != NetworkReachability.NotReachable
                && _retryTimes < Downloader.MaxRetryTimes)
            {
                _downloadAsync.Retry();
                _retryTimes++;
                return;
            }

            request.SetResult(TaskBase.TaskResult.Failed, _downloadAsync.error);
        }

        public void Dispose(LoadBundleTask request)
        {
        }

        public void WaitForCompletion(LoadBundleTask request)
        {
            _downloadAsync.WaitForCompletion();
            while (!request.IsDone) 
                Update(request);
        }
    }
}