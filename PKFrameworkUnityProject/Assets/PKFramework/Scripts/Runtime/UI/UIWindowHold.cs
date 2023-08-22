using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace PKFramework.Runtime.UI
{
    /// <summary>
    /// 持有界面的asset
    /// </summary>
    public static class UIWindowHold
    {
        public static Dictionary<string, GameObject> path2Go = new Dictionary<string, GameObject>();
        public static Dictionary<GameObject, string> go2Path = new Dictionary<GameObject, string>();

        public static GameObject Get(string assetPath, Transform parent)
        {
            GameObject go;
            path2Go.TryGetValue(assetPath, out go);
            if (go == null)
            {
                go = Load(assetPath);
                path2Go.Add(assetPath, go);
                go2Path.Add(go, assetPath);
            }

            //TODO 引用计数+1
            return GameObject.Instantiate(go, parent);
        }

        public static void Release(GameObject go)
        {
            GameObject.Destroy(go);
            //TODO 引用计数-1
        }

        private static GameObject Load(string assetPath)
        {
            //TODO 引用计数+1
            GameObject go = AssetComponent.Instance.LoadSync<GameObject>(assetPath);
            return go;
        }

        public static void Clear()
        {
            path2Go.Clear();
            //TODO 资源引用计数-1
        }
    }
}