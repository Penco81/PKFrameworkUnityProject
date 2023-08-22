using PKFramework.Runtime;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Device.Application;
using Object = UnityEngine.Object;

namespace PKFramework.Editor
{
    public class EditorAssetLoader : IAssetLoader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="loadAssetCallbacks"></param>
        /// <param name="userData"></param>
        /// <param name="priority"></param> 优先级高的优先加载
        public Object LoadSync(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData = null, int priority = 0)
        {
            string path = AssetHelper.EditorPathPrefix + assetName;
            var obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            if (obj == null)
            {
                PKLogger.LogError($"Try to load a null asset, path: {path}");
                loadAssetCallbacks?.loadAssetFailureCallback?.Invoke(assetName, LoadResourceStatus.NotExist, null, userData);
            }
            else
            {
                loadAssetCallbacks?.loadAssetSuccessCallback?.Invoke(assetName, obj, 0, userData);
            }

            return obj;
        }

        public void LoadAsync(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData = null, int priority = 0)
        {
            string path = AssetHelper.EditorPathPrefix + assetName;
            var obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            if (obj == null)
            {
                PKLogger.LogError($"Try to load a null asset, path: {path}");
                loadAssetCallbacks?.loadAssetFailureCallback?.Invoke(assetName, LoadResourceStatus.NotExist, null, userData);
            }
            else
            {
                loadAssetCallbacks?.loadAssetSuccessCallback?.Invoke(assetName, obj, 0, userData);
            }
        }

        public T LoadSync<T>(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData = null, int priority = 0) where T : Object 
        {
            string extension = AssetHelper.GetAssetTypeExtension(typeof(T));
            string path = AssetHelper.EditorPathPrefix + assetName + extension;
            var obj = AssetDatabase.LoadAssetAtPath<T>(path);
            if (obj == null)
            {
                PKLogger.LogError($"Try to load a null asset, path: {path} type: {typeof(T).Name}");
                loadAssetCallbacks?.loadAssetFailureCallback?.Invoke(assetName, LoadResourceStatus.NotExist, null, userData);
            }
            else
            {
                loadAssetCallbacks?.loadAssetSuccessCallback?.Invoke(assetName, obj, 0, userData);
            }
            return obj;
        }

        public void LoadAsync<T>(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData = null, int priority = 0) where T : Object
        {
            string extension = AssetHelper.GetAssetTypeExtension(typeof(T));
            string path = AssetHelper.EditorPathPrefix + assetName + extension;
            var obj = AssetDatabase.LoadAssetAtPath<T>(path);
            if (obj == null)
            {
                PKLogger.LogError($"Try to load a null asset, path: {path} type: {typeof(T).Name}");
                loadAssetCallbacks?.loadAssetFailureCallback?.Invoke(assetName, LoadResourceStatus.NotExist, null, userData);
            }
            else
            {
                loadAssetCallbacks?.loadAssetSuccessCallback?.Invoke(assetName, obj, 0, userData);
            }
        }

        public void UnloadAsset(Object asset)
        {
            
        }

        public void Start()
        {
            throw new System.NotImplementedException();
        }

        public void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}