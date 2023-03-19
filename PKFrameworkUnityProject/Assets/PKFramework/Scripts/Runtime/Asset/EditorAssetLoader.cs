using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Device.Application;
using Object = UnityEngine.Object;

namespace PKFramework.Runtime
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
                loadAssetCallbacks.LoadAssetFailureCallback?.Invoke(assetName, LoadResourceStatus.NotExist, null, userData);
            }
            else
            {
                loadAssetCallbacks.LoadAssetSuccessCallback?.Invoke(assetName, obj, 0, userData);
            }

            return obj;
        }

        public Object LoadAsync(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData = null, int priority = 0)
        {
            string path = AssetHelper.EditorPathPrefix + assetName;
            var obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            if (obj == null)
            {
                PKLogger.LogError($"Try to load a null asset, path: {path}");
                loadAssetCallbacks.LoadAssetFailureCallback?.Invoke(assetName, LoadResourceStatus.NotExist, null, userData);
            }
            else
            {
                loadAssetCallbacks.LoadAssetSuccessCallback?.Invoke(assetName, obj, 0, userData);
            }
            return obj;
        }

        public T LoadSync<T>(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData = null, int priority = 0) where T : Object 
        {
            string extension = AssetHelper.GetAssetTypeExtension(typeof(T));
            string path = AssetHelper.EditorPathPrefix + assetName + extension;
            var obj = AssetDatabase.LoadAssetAtPath<T>(path);
            if (obj == null)
            {
                PKLogger.LogError($"Try to load a null asset, path: {path} type: {typeof(T).Name}");
                loadAssetCallbacks.LoadAssetFailureCallback?.Invoke(assetName, LoadResourceStatus.NotExist, null, userData);
            }
            else
            {
                loadAssetCallbacks.LoadAssetSuccessCallback?.Invoke(assetName, obj, 0, userData);
            }
            return obj;
        }

        public T LoadAsync<T>(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData = null, int priority = 0) where T : Object
        {
            string extension = AssetHelper.GetAssetTypeExtension(typeof(T));
            string path = AssetHelper.EditorPathPrefix + assetName + extension;
            var obj = AssetDatabase.LoadAssetAtPath<T>(path);
            if (obj == null)
            {
                PKLogger.LogError($"Try to load a null asset, path: {path} type: {typeof(T).Name}");
                loadAssetCallbacks.LoadAssetFailureCallback?.Invoke(assetName, LoadResourceStatus.NotExist, null, userData);
            }
            else
            {
                loadAssetCallbacks.LoadAssetSuccessCallback?.Invoke(assetName, obj, 0, userData);
            }
            return obj;
        }

        public void UnloadAsset(Object asset)
        {
            
        }
    }
}