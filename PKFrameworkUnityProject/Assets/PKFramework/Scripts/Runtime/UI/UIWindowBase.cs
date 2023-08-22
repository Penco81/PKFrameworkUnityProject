using System.Collections.Generic;
using UnityEngine;

namespace PKFramework.Runtime.UI
{
    /// <summary>
    /// UI窗口类型的基类
    /// </summary>
    public abstract class UIWindowBase : UIBase
    {
        protected Transform parent;
        protected List<UIBase> items;
        protected UIWindowParam param;
        protected UIWindowBase(Transform parent, GameObject go, UIWindowParam param = null) : base(go)
        {
            if (parent == null)
            {
                PKLogger.LogError($"Parent is null. Type name: {this.GetType().Name}");
            }

            this.parent = parent;
            items = new List<UIBase>();
            this.param = param;
        }

        public void CreateItem()
        {
            
        }

        public virtual void Init()
        {
            
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            foreach (var item in items)
            {
                item.OnDestroy();
            }
            items.Clear();
        }
    }
}