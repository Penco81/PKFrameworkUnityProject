using System;
using System.Collections.Generic;
using UnityEngine;

namespace PKFramework.Editor.BuildAsset
{
    //构建日志
    [Serializable]
    public class BuildRecord
    {
        //构建的名字
        public string name;
        //平台
        public string platform;
        //资源的改变
        public string[] changes;
        //大小
        public ulong size;
        //构建的时间戳
        public long timestamp;
    }

    public class BuildRecords : ScriptableObject, ISerializationCallbackReceiver
    {
        public static string Filename = "BuildRecords.json";
        public List<BuildRecord> data = new List<BuildRecord>();

        private Dictionary<string, BuildRecord> _data = new Dictionary<string, BuildRecord>();

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            _data.Clear();
            foreach (var record in data)
            {
                _data[record.name] = record;
            }
        }

        public void Set(string file, string[] changes, ulong size)
        {
            if (!_data.TryGetValue(file, out var value))
            {
                value = new BuildRecord()
                {
                    name = file,
                    changes = changes,
                    size = size,
                    timestamp = DateTime.Now.ToFileTime(),
                };
                _data[file] = value;
                data.Add(value);
            }
            else
            {
                PKLogger.LogWarning($"Record {file} Exist.");
            }
        }

        public bool TryGetValue(string file, out BuildRecord value)
        {
            return _data.TryGetValue(file, out value);
        }
    }
}