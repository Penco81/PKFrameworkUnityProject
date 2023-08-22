using System.Collections.Generic;
using System.Linq;
using System.Text;
using PKFramework.Runtime.Asset;

namespace PKFramework.Editor.BuildAsset
{
    //优化打包分组
    public class OptimizeDependentAssets : IBuildJobStep
    {
        public void Start(BuildJob job)
        {
            var bundledAssets = job.bundledAssets;
            var pathWithAssets = new Dictionary<string, BuildAsset>();
            
            foreach (var bundledAsset in bundledAssets) 
                pathWithAssets[bundledAsset.path] = bundledAsset;

            var references = new Dictionary<string, List<BuildAsset>>();
            
            //寻找不会被主动打成包，但是被多重引用的资源路径，打包时会把引用的资源一起打包在一起
            foreach (var asset in bundledAssets)
            {
                if (!BuildUtils.CheckReferences(asset)) 
                    continue;

                var dependencies = Settings.GetDependencies(asset.path);
                foreach (var dependency in dependencies)
                {
                    if (pathWithAssets.ContainsKey(dependency)) 
                        continue;

                    if (!references.TryGetValue(dependency, out var assets))
                    {
                        assets = new List<BuildAsset>();
                        references[dependency] = assets;
                    }

                    assets.Add(asset);
                }
            }

            if (references.Count <= 0) return;

            var group = Settings.GetAutoGroup();

            foreach (var pair in references)
            {
                var path = pair.Key;
                var assets = pair.Value;

                // 非公共依赖不主动分组 - beg
                if (assets.Count <= 1) 
                    continue;
                var bundles = new HashSet<string>(assets.ConvertAll(input => input.bundle)).ToList();
                if (bundles.Count <= 1) 
                    continue;

                // 这个是符合按需加载的最优策略：按依赖关系，尽可能把同时使用的打包到一起。
                bundles.Sort();
                var asset = Settings.GetAsset(path);
                asset.entry = path;
                asset.group = group;
                asset.addressMode = group.addressMode;
                var hash = AssetUtils.ComputeHash(Encoding.UTF8.GetBytes(string.Join("_", bundles.ToArray())));
                asset.bundle = Settings.PackAsset(asset.path, $"auto_{hash}", job.parameters.name);
                job.AddAsset(asset);
                pathWithAssets.Add(path, asset);
            }
        }
    }
}