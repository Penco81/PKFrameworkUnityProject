using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace PKFramework.Editor.BuildAsset
{
    /// <summary>
    ///根据buildGroup收集资源
    /// </summary>
    public class CollectAssets : IBuildJobStep
    {
        public void Start(BuildJob job)
        {
            foreach (var group in job.parameters.groups)
            {
                if (group == null)
                {
                    PKLogger.LogWarning($"Group is missing in build {job.parameters.name}");
                    continue;
                }

                group.build = job.parameters.name;
                var assets = Settings.Collect(group);
                foreach (var asset in assets)
                {
                    //获取包名
                    asset.bundle = Settings.PackAsset(asset);
                    job.AddAsset(asset);
                }
            }
            PKLogger.LogMessage("CollectAssets Complete");
        }
    }
}