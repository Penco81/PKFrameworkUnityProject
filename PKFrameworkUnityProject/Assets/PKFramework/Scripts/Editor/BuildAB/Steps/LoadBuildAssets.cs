using System.IO;
using UnityEngine;

namespace PKFramework.Editor.BuildAsset
{
    /// <summary>
    /// 通过缓存加载已经构建的资源
    /// </summary>
    public class LoadBuildAssets : IBuildJobStep
    {
        public void Start(BuildJob job)
        {
            var path = Settings.GetCachePath(job.parameters.name + ".json");
            if (!File.Exists(path))
            {
                job.error = $"File not found {path}.";
                return;
            }

            var buildAssets = ScriptableObject.CreateInstance<BuildAssets>();
            JsonUtility.FromJsonOverwrite(File.ReadAllText(path), buildAssets);
            job.bundledAssets.AddRange(buildAssets.bundledAssets);
        }
    }
}