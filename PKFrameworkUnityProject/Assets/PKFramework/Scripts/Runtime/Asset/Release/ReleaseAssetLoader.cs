using System;
using System.Collections.Generic;
using PKFramework.Runtime.Task;
using Object = UnityEngine.Object;


namespace PKFramework.Runtime.Asset
{
    public class ReleaseAssetLoader : IAssetLoader
    {
        //引用计数
        private static readonly Dictionary<string, string[]> assetWithDependencies = new Dictionary<string, string[]>();
        private static readonly Dictionary<string, int> assetWithReferences = new Dictionary<string, int>();
        
        private static readonly Dictionary<string, LoadTaskBase> progressing = new Dictionary<string, LoadTaskBase>();
        
        private static readonly Dictionary<string, Object> cachedAsset = new Dictionary<string, Object>();
        private static readonly Dictionary<string, Object> cachedBundle = new Dictionary<string, Object>();
        
        //依赖
        public static readonly Dictionary<string, Dependencies> dependenceLoaded = new Dictionary<string, Dependencies>();
        public static readonly Dictionary<string, LoadBundleTask> bundleLoaded = new Dictionary<string, LoadBundleTask>();
        

        #region 引用计数相关
        private static string[] GetDependencies(string path)
        {
            if (assetWithDependencies.TryGetValue(path, out var dependencies)) return dependencies;
            dependencies = GetInternal(path);
            assetWithDependencies[path] = dependencies;
            return dependencies;
        }

        private static string[] GetInternal(string path)
        {
            return AssetUtils.TryGetAsset(ref path, out var asset) ? Array.ConvertAll(asset.deps, input => asset.manifest.assets[input].path) : Array.Empty<string>();
        }

        public static void Retain(string path)
        {

            RetainInternal(path);
            var children = GetDependencies(path);
            foreach (var child in children)
            {
                RetainInternal(child);
            }
        }

        private static void RetainInternal(string path)
        {
            if (!assetWithReferences.TryGetValue(path, out var value))
            {
                value = 0;
            }

            value += 1;
            assetWithReferences[path] = value;
        }

        public static int Release(string path)
        {

            var result = ReleaseInternal(path);
            var children = GetDependencies(path);
            foreach (var child in children)
            {
                ReleaseInternal(child);
            }

            return result;
        }

        private static int ReleaseInternal(string path)
        {
            if (!assetWithReferences.TryGetValue(path, out var value))
            {
                return 0;
            }

            value -= 1;
            assetWithReferences[path] = value;
            return value;
        }
        
        #endregion

        #region 加载资源

        public Object LoadSync(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData = null, int priority = 0)
        {
            throw new NotImplementedException();
        }

        public void LoadAsync(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData = null, int priority = 0)
        {
            //TODO
            string path = assetName;
            if (cachedAsset.TryGetValue(path, out var Obj))
            {
                return;
            }

            if (progressing.TryGetValue(path, out var task))
            {
                return;
            }

            var newTask = TaskManager.Instance.CreateTask<LoadAssetTask>();
            
            
            
            progressing.Add(path, newTask);
        }

        public T LoadSync<T>(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData = null, int priority = 0) where T : Object
        {
            throw new NotImplementedException();
        }

        public void LoadAsync<T>(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData = null, int priority = 0) where T : Object
        {
            throw new NotImplementedException();
        }

        public void UnloadAsset(Object asset)
        {
            throw new NotImplementedException();
        }

        private void OnLoadAssetComplete(string path, Object asset)
        {
            cachedAsset.Add(path, asset);
        }

        private void OnLoadAssetBundleComplete(string path, Object bundle)
        {
            cachedBundle.Add(path, bundle);
        }

        #endregion

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            
        }
    }
}