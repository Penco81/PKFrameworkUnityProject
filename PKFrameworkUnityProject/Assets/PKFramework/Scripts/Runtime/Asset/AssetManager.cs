using System;
using System.Collections.Generic;
using PKFramework.Core.Manager;
using PKFramework.Runtime.Singleton;
using PKFramework.Scripts.Runtime.Base;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PKFramework.Runtime.Asset
{
    public class AssetManager : ManagerBase<AssetManager>
    {
        private static readonly Dictionary<string, AssetData> _assetDataDict = new Dictionary<string, AssetData>();
        private static readonly Dictionary<string, AssetBundleData> _abDataDict = new Dictionary<string, AssetBundleData>();

        //常驻的资源路径
        private static readonly HashSet<string> whiteList = new HashSet<string>();
        
        

        public void Init(Manifest manifest)
        {
            
        }

        public void LoadSync()
        {
            
        }

        public void LoadAsync(Action<GameObject> complete)
        {
            
        }

        public void UnloadAsset(Object asset)
        {
            UnusedAssets.Enqueue(asset);
        }

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            base.Update(elapseSeconds, realElapseSeconds);
            
            if (UnusedAssets.Count > 0)
            {
                while (UnusedAssets.Count > 0)
                {
                    var item = UnusedAssets.Dequeue();
                    Resources.UnloadAsset(item);
                }
                
                //TODO
                Resources.UnloadUnusedAssets();
            }
        }
    }
}