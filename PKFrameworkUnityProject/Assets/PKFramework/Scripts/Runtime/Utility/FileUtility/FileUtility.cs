using System.IO;
using UnityEngine;

namespace PKFramework.Runtime
{
    public static class FileUtility
    {
        private static string _persistentDataPath = null;
        private static string _assetRootPath = null;
        private static string _assetRootStreamAssetPath = null;
        
        /// <summary>
        /// 资源更新读取根目录
        /// </summary>
        /// <returns></returns>
        public static string AssetRoot
        {
            get
            {
                if (string.IsNullOrEmpty(_assetRootPath))
                {
                    _assetRootPath = Path.Combine(PersistentDataPath, "PKFramework");
                }

                if (!Directory.Exists(_assetRootPath))
                {
                    Directory.CreateDirectory(_assetRootPath);
                }

                return _assetRootPath.FixPath();
            }
        }
        
        /// <summary>
        /// 资源更新读取StreamAsset根目录
        /// </summary>
        /// <returns></returns>
        public static string AssetRootInStreamAsset
        {
            get
            {
                if (string.IsNullOrEmpty(_assetRootStreamAssetPath))
                {
                    _assetRootStreamAssetPath = Path.Combine(Application.streamingAssetsPath, "TEngine");
                }
                return _assetRootStreamAssetPath.FixPath();
            }
        }
        
        /// <summary>
        /// 持久化数据存储路径
        /// </summary>
        public static string PersistentDataPath
        {
            get
            {
                if (string.IsNullOrEmpty(_persistentDataPath))
                {
#if UNITY_EDITOR
                    _persistentDataPath = Application.dataPath + "/../PKFrameworkPersistentDataPath";
                    if (!Directory.Exists(_persistentDataPath))
                    {
                        Directory.CreateDirectory(_persistentDataPath);
                    }
#else
                    _persistentDataPath = Application.persistentDataPath;
#endif
                }
                return _persistentDataPath.FixPath();
            }
        }
        
        public static string FixPath(this string str)
        {
            str = str.Replace("\\", "/");
            return str;
        }
        
        public static string GetAssetBundlePathInVersion(string bundlename)
        {
            //默认用外部目录
            string path = $"{AssetRoot}/AssetBundles/{bundlename}";
            if (!File.Exists(path))
            {
                path = $"{AssetRootInStreamAsset}/AssetBundles/{bundlename}";
            }

            return path;
        }
        
    }
}