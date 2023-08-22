using System.Collections.Generic;
using UnityEngine;

namespace PKFramework.Runtime.Asset
{
    public class PlayerAssets : ScriptableObject
    {
        public static readonly string Filename = $"{nameof(PlayerAssets).ToLower()}.json";
        //PlayerSettings.bundleVersion
        public string version;

        public string updateInfoURL;
        public string downloadURL;
        public byte maxRetryTimes;
        public byte maxDownloads;
        
        //包含的bundle hash
        public List<string> data = new List<string>();

        public bool Contains(string key)
        {
            return data.Contains(key);
        }
    }
}