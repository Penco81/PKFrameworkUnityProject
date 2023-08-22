using System.IO;
using UnityEngine;

namespace PKFramework.Editor.BuildAsset
{
    /// <summary>
    /// 保存所有buildAsset信息到json，并且存到缓存目录
    /// </summary>
    public class SaveBuildAssets : IBuildJobStep
    {
        public void Start(BuildJob job)
        {
            var buildAssets = ScriptableObject.CreateInstance<BuildAssets>();
            buildAssets.bundledAssets = job.bundledAssets.ToArray();
            var json = JsonUtility.ToJson(buildAssets);
            var path = Settings.GetCachePath(job.parameters.name + ".json");
            File.WriteAllText(path, json);
        }
    }
}