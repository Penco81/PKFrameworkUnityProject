using System;
using PKFramework.Runtime.Singleton;
using UnityEngine;

namespace PKFramework.Runtime.UI
{
    public class UIComponent : MonoSingleton<UIComponent>
    {
        public int uiStackOrderInterval = 2000;
        public int uiWindowInterval = 40;

        private UIManager uiManager;

        private Transform uiRoot;

        public Transform UIRoot => uiRoot;

        private RectTransform rectTrans;

        public RectTransform RectTrans => rectTrans;

        private Camera camera;

        public Camera Camera => camera;
        
        private void Awake()
        {
            uiRoot = this.transform.Find("UIRoot");
            if (uiRoot == null)
            {
                PKLogger.LogError("UIRoot is null");
            }

            rectTrans = uiRoot.GetComponent<RectTransform>();
            if (rectTrans == null)
            {
                PKLogger.LogError("UIRoot rectTransform is null");
            }
            
            camera = this.transform.Find("UIRoot/UICamera")?.GetComponent<Camera>();
            if (camera == null)
            {
                PKLogger.LogError("UICamera is null");
            }
            
            uiManager = UIManager.Instance;
            uiManager.Awake();
        }
    }
}