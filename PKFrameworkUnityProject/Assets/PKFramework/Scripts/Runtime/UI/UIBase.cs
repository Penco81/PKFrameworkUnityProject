using System.Collections.Generic;
using UnityEngine;

namespace PKFramework.Runtime.UI
{
    /// <summary>
    /// 所有UI物体的基类
    /// </summary>
    public abstract class UIBase
    {

        protected GameObject go;
        protected RectTransform rectTrans;
        protected bool destroyed = false;
        protected UIManager uiManager;
        protected Dictionary<string, Transform> uiMap;
        protected List<string> paths;
        
        public GameObject Go
        {
            get => go;
        }

        public RectTransform RectTrans
        {
            get => rectTrans;
        }

        public bool Destroyed
        {
            get => destroyed;
            set => destroyed = value;
        }

        public UIManager UIManager
        {
            get => uiManager;
        }

        protected UIBase(GameObject go)
        {
            this.go = go;
            rectTrans = go.GetComponent<RectTransform>();
            if (rectTrans == null)
            {
                rectTrans = go.AddComponent<RectTransform>();
            }

            uiManager = UIManager.Instance;
            uiMap = new Dictionary<string, Transform>();
            paths = new List<string>();
            SetUiMapPath();
            RegisterUIMap();
        }

        public abstract void SetUiMapPath();

        private void RegisterUIMap()
        {
            foreach (var path in paths)
            {
                Transform trans = go.transform.Find(path);
                if (trans == null)
                {
                    PKLogger.LogWarning($"Transform is null. Path: {path}, Type: {GetType().Name}");
                }

                uiMap.Add(path, trans);
            }
        }

        public virtual void OnDestroy()
        {
            go = null;
            destroyed = true;
        }

    }
}