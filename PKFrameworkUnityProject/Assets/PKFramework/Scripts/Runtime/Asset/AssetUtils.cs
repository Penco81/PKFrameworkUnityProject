
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using AddressMode = PKFramework.Runtime.Asset.AddressMode;

namespace PKFramework.Runtime.Asset
{
    public static class AssetUtils
    {
        public const string Bundles = "Bundles";
        
        private static readonly double[] ByteUnits = {1073741824.0, 1048576.0, 1024.0, 1};

        private static readonly string[] ByteUnitsNames = {"GB", "MB", "KB", "B"};
        
        private static readonly Dictionary<string, string> AddressWithPaths = new Dictionary<string, string>();
        
        //streamingAsset的位置
        public static string PlayerDataPath { get; set; } = $"{Application.streamingAssetsPath}/{Bundles}";
        
        //下载的文件所在的位置
        public static string DownloadDataPath { get; set; } = $"{Application.persistentDataPath}/{Bundles}";
        
        public static Platform PlatformEnum { get; set; } = GetPlatform();
        
        //UpdateInfo.json文件的下载地址
        public static string UpdateInfoURL { get; set; }
        //资源的下载地址
        public static string DownloadURL { get; set; }
        
        public static Versions Versions { get; set; } = ScriptableObject.CreateInstance<Versions>();
        
        public static PlayerAssets PlayerAssets { get; set; } = ScriptableObject.CreateInstance<PlayerAssets>();
        
        public static Platform Platform { get; set; } = GetPlatform();

        public static string Protocol { get; } = GetProtocol();

        /// <summary>
        /// streamingAssetPath下的路径
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetPlayerDataPathWithFileName(string filename)
        {
            return $"{PlayerDataPath}/{filename}";
        }
        
        /// <summary>
        /// persistentDataPath下的路径
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetDownloadDataPathWithFileName(string filename)
        {
            var path = $"{DownloadDataPath}/{filename}";
            CreateDirectoryIfNecessary(path);
            return path;
        }
        
        public static string GetPlayerDataPath(string filename)
        {
            return $"{PlayerDataPath}/{filename}";
        }
        
        public static string GetDownloadURLWithFileName(string filename)
        {
            return $"{GetDownloadURL()}/{filename}";
        }
        
        public static string GetPlayerDataURlWithFileName(string filename)
        {
            return $"{GetProtocol()}{PlayerDataPath}/{filename}";
        }
        
        public static string GetDownloadURL(string filename)
        {
            return $"{DownloadURL}/{filename}";
        }
        
        public static string GetPlayerDataURl(string filename)
        {
            return $"{Protocol}{GetPlayerDataPath(filename)}";
        }
        
        public static string GetUpdateInfoURL()
        {
            //return Launcher.Instance.assetConfig.updateInfoURL;
            return null;
        }
        
        public static string GetProtocol()
        {
            if (Application.platform == RuntimePlatform.OSXEditor ||
                Application.platform == RuntimePlatform.OSXPlayer ||
                Application.platform == RuntimePlatform.IPhonePlayer) return "file://";

            if (Application.platform == RuntimePlatform.WindowsEditor ||
                Application.platform == RuntimePlatform.WindowsPlayer)
                return "file:///";

            return string.Empty;
        }
        
        public static Platform GetPlatform()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return Platform.Android;
                case RuntimePlatform.WindowsPlayer:
                    return Platform.Windows;
                case RuntimePlatform.OSXPlayer:
                    return Platform.OSX;
                case RuntimePlatform.IPhonePlayer:
                    return Platform.iOS;
                case RuntimePlatform.WebGLPlayer:
                    return Platform.WebGL;
                case RuntimePlatform.LinuxPlayer:
                    return Platform.Linux;
                default:
                    return Platform.Default;
            }
        }
        
        public static void CreateDirectoryIfNecessary(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(dir) || Directory.Exists(dir)) return;

            Directory.CreateDirectory(dir);
        }
        
        public static string ToHash(byte[] data)
        {
            var sb = new StringBuilder();
            foreach (var t in data) sb.Append(t.ToString("x2"));

            return sb.ToString();
        }

        public static string ComputeHash(byte[] bytes)
        {
            var data = MD5.Create().ComputeHash(bytes);
            return ToHash(data);
        }

        public static string ComputeHash(string filename)
        {
            //TODO: 这里可以使用第三方的 hash 库优化性能。
            if (!File.Exists(filename)) return string.Empty;

            using (var stream = File.OpenRead(filename))
            {
                return ToHash(MD5.Create().ComputeHash(stream));
            }
        }
        
        public static string FormatBytes(ulong bytes)
        {
            var size = "0 B";
            if (bytes == 0) return size;

            for (var index = 0; index < ByteUnits.Length; index++)
            {
                var unit = ByteUnits[index];
                if (!(bytes >= unit)) continue;

                size = $"{bytes / unit:##.##} {ByteUnitsNames[index]}";
                break;
            }

            return size;
        }

        public static string GetDirectoryName(string path)
        {
            var dir = Path.GetDirectoryName(path);
            return dir?.Replace("\\", "/");
        }
        
        /// <summary>
        /// 加载资源配置的ScriptableObject，如果没有则创建一个
        /// </summary>
        /// <param name="filename"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T LoadOrCreateFromFile<T>(string filename) where T : ScriptableObject
        {
            if (!File.Exists(filename)) return ScriptableObject.CreateInstance<T>();

            var json = File.ReadAllText(filename);
            var asset = ScriptableObject.CreateInstance<T>();
            try
            {
                JsonUtility.FromJsonOverwrite(json, asset);
            }
            catch (Exception e)
            {
                PKLogger.LogError(e.Message);
                File.Delete(filename);
            }

            return asset;
        }

        public static void SetAddress(string assetPath, string address)
        {
            if (!AddressWithPaths.TryGetValue(address, out var value))
            {
                AddressWithPaths[address] = assetPath;
            }
            else
            {
                if (assetPath != value)
                {
                    PKLogger.LogWarning($"Failed to set address for {assetPath}, because the address {address} already mapping to {value}");
                }
            }
        }
        
        public static void GetActualPath(ref string path)
        {
            if (AddressWithPaths.TryGetValue(path, out var value))
            {
                path = value;
            }
        }

        public static bool TryGetAsset(ref string path, out ManifestAsset asset)
        {
            GetActualPath(ref path);

            if (Versions.TryGetAsset(path, out asset))
            {
                return true;
            }

            PKLogger.LogError($"File not found:{path}.");
            return false;
        }
        
        public static bool IsDownloaded(string path)
        {
            if (!TryGetAsset(ref path, out var asset)) return true;

            var bundles = asset.manifest.bundles;
            var bundle = bundles[asset.bundle];
            if (!IsDownloaded(bundle))
                return false;

            foreach (var dependency in bundle.deps)
                if (!IsDownloaded(bundles[dependency]))
                    return false;

            return true;
        }

        public static bool IsDownloaded(Download item)
        {
            var path = GetDownloadDataPathWithFileName(item.file);
            var file = new FileInfo(path);
            return file.Exists && file.Length == (long) item.size;
        }
        
        public static bool IsPlayerAsset(string key)
        {
            //if (OfflineMode) return true;
            return PlayerAssets != null && PlayerAssets.Contains(key);
        }
    }
}