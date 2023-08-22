using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PKFramework.Editor.BuildAsset
{
    //一系列打包流程的执行
    public class BuildJob
    {
        //所有需要打包的资产信息
        public readonly List<BuildAsset> bundledAssets = new List<BuildAsset>();
        //所有ab包的信息
        public readonly List<BuildBundle> bundles = new List<BuildBundle>();
        public readonly List<string> changes = new List<string>();
        public string error { get; set; }

        public BuildJob(BuildParameters buildParameters)
        {
            parameters = buildParameters;
        }

        public BuildParameters parameters { get; }

        public void AddAsset(BuildAsset asset)
        {
            bundledAssets.Add(asset);
        }

        public static BuildJob StartNew(BuildParameters parameters, params IBuildJobStep[] steps)
        {
            var job = new BuildJob(parameters);
            job.Start(steps);
            return job;
        }

        public void Start(params IBuildJobStep[] steps)
        {
            foreach (var step in steps)
            {
                var sw = new Stopwatch();
                sw.Start();
                try
                {
                    step.Start(this);
                }
                catch (Exception e)
                {
                    PKLogger.LogError($"{e.Message}:{e.StackTrace}");
                    error = e.Message;
                }

                sw.Stop();
                PKLogger.LogMessage($"{step.GetType().Name} for {parameters.name} {(string.IsNullOrEmpty(error) ? "success" : "failed")} with {sw.ElapsedMilliseconds / 1000f}s.");
                if (!string.IsNullOrEmpty(error)) break;
            }
        }

        public void TreatError(string e)
        {
            error = e;
            PKLogger.LogError($"{error}");
        }
    }
}