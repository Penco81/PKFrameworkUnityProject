using UnityEngine;

namespace PKFramework.Runtime.Asset
{
    public class UpdateInfo : ScriptableObject
    {
        public static readonly string Filename = $"{nameof(UpdateInfo).ToLower()}.json";
        // 版本文件的文件名
        public string file;
        // 版本文件文件内容的 hash
        public string hash;
        //版本文件的大小
        public ulong size;
        //游戏版本
        public string version;
    }
}