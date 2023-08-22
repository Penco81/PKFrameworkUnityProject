using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PKFramework.Runtime
{
    public interface IAssetLoader
    {
        Object LoadSync(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData = null, int priority = 0);
        
        void LoadAsync(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData = null, int priority = 0);
        
        T LoadSync<T>(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData = null,  int priority = 0) where T : Object ;
        
        void LoadAsync<T>(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData = null,  int priority = 0) where T : Object ;

        void UnloadAsset(Object asset);

        void Start();

        void Update();
    }
}