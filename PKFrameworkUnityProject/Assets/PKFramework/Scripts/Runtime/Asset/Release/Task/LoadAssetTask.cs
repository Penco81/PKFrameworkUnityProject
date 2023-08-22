using System;
using System.Collections.Generic;
using PKFramework.Runtime.Asset;
using PKFramework.Runtime.Pool;

namespace PKFramework.Runtime.Asset
{
    public class LoadAssetTask : LoadTaskBase
    {
        public IAssetHandler handler { get; } = RuntimeAssetHandler.CreateInstance();
        
        public Object asset { get; set; }
        public Object[] assets { get; set; }
        public bool isAll { get; private set; }
        public ManifestAsset info { get; private set; }
        public Type type { get; private set; }

        protected override void OnDispose()
        {
            
        }
        
        protected override void OnStart()
        {
            handler.OnStart(this);
            ReleaseAssetLoader.Retain(Path);
        }

        protected override void OnWaitForCompletion()
        {
            handler.WaitForCompletion(this);
        }

        protected override void OnUpdated()
        {
            handler.Update(this);
        }
    }
}