using System;
using System.Collections.Generic;
using UnityEngine;

namespace PKFramework.Runtime.UI
{
    /// <summary>
    /// 子窗口，UIWindow或UISubWindow控制
    /// </summary>
    public abstract class UISubWindow : UIWindowBase
    {
        protected RectTransform uiParent;
        public UISubWindow(Transform parent, GameObject go, UIWindowParam param = null) : base(parent, go, param)
        {
            uiParent = parent.GetComponent<RectTransform>();
            if (uiParent == null)
            {
                PKLogger.LogError($"uiParent is null. Type name: {this.GetType().Name}");
            }
        }

        protected T CreateSubWindow<T>(UIWindowParam param = null) where T : UISubWindow
        {
            //TODO
            UISubWindow subWindow = null;
            return subWindow as T;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}