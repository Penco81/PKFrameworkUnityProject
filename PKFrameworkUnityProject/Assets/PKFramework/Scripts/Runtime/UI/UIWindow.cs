using UnityEngine;

namespace PKFramework.Runtime.UI
{
    /// <summary>
    /// UI窗口，UIManager控制
    /// </summary>
    public abstract class UIWindow : UISubWindow
    {
        protected Canvas canvas;
        protected UIConfig uiConfig;
        protected WindowState state = WindowState.Hidden;
        protected int order = 0;
        protected ulong uniqId;

        public WindowState State => state;
        
        public ulong UniqId => uniqId;
        
        public UIConfig UIConfig => uiConfig;

        public GameObject Go => go;
        
        public UIWindow(UIConfig uiConfig, Transform parent, GameObject go, ulong uniqId, UIWindowParam param = null) : base(parent, go, param)
        {
            this.uniqId = uniqId;
            if (uiConfig == null)
            {
                PKLogger.LogError($"UIConfig is null. Type name: {this.GetType().Name}");
            }
            this.uiConfig = uiConfig;
            canvas = go.GetComponent<Canvas>();
            if (canvas == null)
            {
                PKLogger.LogError($"Canvas is null. Type name: {this.GetType().Name}");
            }
            
            canvas.overridePixelPerfect = true;
            canvas.overrideSorting = true;
            canvas.sortingOrder = order;
            canvas.worldCamera = uiManager.Camera;
        }

        /// <summary>
        /// 被遮挡
        /// </summary>
        public virtual void OnCover()
        {
            //TODO Debug
            PKLogger.LogMessage($"Cover {this.GetType()}");
        }

        /// <summary>
        /// 位于顶层
        /// </summary>
        public virtual void OnFocus()
        {
            //TODO
            go?.SetActive(true);
            PKLogger.LogMessage($"OnFocus {this.GetType()}");
        }

        /// <summary>
        /// 被隐藏
        /// </summary>
        public virtual void OnHide()
        {
            state = WindowState.Hidden;
            go?.SetActive(false);
            PKLogger.LogMessage($"OnHide {this.GetType()}");
        }
        
        /// <summary>
        /// 激活
        /// </summary>
        public virtual void OnActive()
        {
            go?.SetActive(true);
            PKLogger.LogMessage($"OnActive {this.GetType()}");
        }

        public virtual void OnChangeOrder(int order)
        {
            if (canvas == null)
            {
                PKLogger.LogError($"Canvas is null. Type name: {this.GetType().Name}");
                return;
            }

            canvas.sortingOrder = order;
            
            //TODO 界面的特效全部设置层级
        }
    }
}