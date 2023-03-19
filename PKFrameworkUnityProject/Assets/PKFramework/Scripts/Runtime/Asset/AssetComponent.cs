using PKFramework.Runtime;
using PKFramework.Runtime.Singleton;
using UnityEngine;
using Object = UnityEngine.Object;
namespace PKFramework.Runtime
{
    public class AssetComponent : MonoSingleton<AssetComponent>
    {
        /// <summary>
        ///最大并行下载数量
        /// </summary>
        public int maxDownloads = 5;

        ///<summary>
        ///最大错误重试次数
        /// </summary>
        public int maxRetryTimes = 3;
        
        [Tooltip("updateinfo.json下载地址")]
        public string updateInfoURL = "http://127.0.0.1/";
        
        [Tooltip("包体下载地址")]
        public string downloadURL = "http://127.0.0.1/";
        
        [Tooltip("强更安装包下载地址")]
        public string playerDownloadURL = "http://127.0.0.1/";

        public AssetLoadMode mode = AssetLoadMode.Editor;

        private IAssetLoader loader;
        private IAssetLoader Loader
        {
            get
            {
                if (loader == null)
                {
                    switch (mode)
                    {
                        case AssetLoadMode.Editor:
                            loader = new EditorAssetLoader();
                            break;
                        case AssetLoadMode.EditorRelease:

                            break;
                        case AssetLoadMode.Simulation:

                            break;
                        case AssetLoadMode.Release:

                            break;
                        default:
                            loader = new EditorAssetLoader();
                            break;
                    }
                }

                return loader;
            }
        }

        /// <summary>
        /// 加载模式
        /// </summary>
        public enum AssetLoadMode
        {
            //使用AssetDataBase加载
            Editor = 0,
            //在本地使用ab加载
            EditorRelease,
            //在本地使用ab加载，并且模拟热更
            Simulation,
            //正式版模式
            Release,
        }

        public Object LoadSync(string assetName, LoadAssetCallbacks loadAssetCallbacks,
            object userData = null, int priority = 0)
        {
            return Loader.LoadAsync(assetName, loadAssetCallbacks, userData, priority);
        }

        public Object LoadAsync(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData = null,
            int priority = 0)
        {
            return Loader.LoadAsync(assetName, loadAssetCallbacks, userData, priority);
        }

        public T LoadSync<T>(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData = null, int priority = 0)
            where T : Object
        {
            return Loader.LoadSync<T>(assetName, loadAssetCallbacks, userData, priority);
        }

        public T LoadAsync<T>(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData = null,
            int priority = 0) where T : Object
        {
            return Loader.LoadAsync<T>(assetName, loadAssetCallbacks, userData, priority);
        }

        void UnloadAsset(Object asset)
        {
            
        }
    }
}