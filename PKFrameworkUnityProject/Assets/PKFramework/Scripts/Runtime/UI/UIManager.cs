using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using PKFramework.Runtime.Base;
using PKFramework.Runtime.Singleton;
using PKFramework.Runtime.UI;
using PKFramework.Scripts.Runtime.Base;
using UnityEngine;

namespace PKFramework.Runtime.UI
{

    public enum UILayer:byte
    {
        //场景UI
        Scene = 0,
        //UI背景
        BackGround,
        //常用UI界面
        CommonUI,
        //顶层UI界面，比如新手引导界面
        TopUI,
        //游戏消息弹出，比如小tips，跑马灯
        Toast,
        //应用层消息，版本号，错误提示等
        System,
    }
    
    public class UIConfig
    {
        public UILayer uiLayer;
        public bool isFullScreen;

        public bool isMultiple;

        public string path;

        public UIConfig(string path, UILayer uiLayer = UILayer.CommonUI, bool isFullScreen = false, bool isMultiple = false)
        {
            this.path = path;
            this.uiLayer = uiLayer;
            this.isFullScreen = isFullScreen;
            this.isMultiple = isMultiple;
        }
    }

    public enum WindowState : byte
    {
        Hidden,
        Active,
    }

    public class UIManager : Singleton<UIManager>, IFrameworkManager
    {
        //所有UI 的路径
        private Dictionary<Type, UIConfig> uiConfigs = new Dictionary<Type, UIConfig>();

        private Dictionary<UILayer, UIWindowStack> uiStacks = new Dictionary<UILayer, UIWindowStack>();

        public int StackInterval => UIComponent.Instance.uiStackOrderInterval;

        public int WindowInterval => UIComponent.Instance.uiWindowInterval;
        
        public Transform UIRoot => UIComponent.Instance.UIRoot;
        
        public RectTransform UIRootRect => UIComponent.Instance.RectTrans;
        
        public Camera Camera => UIComponent.Instance.Camera;

        private ulong index = 0;

        public UIManager():base()
        {
            RegisterUIWindow();
        }

        private UIWindowStack GetUIStack(UILayer layer)
        {
            if (!uiStacks.TryGetValue(layer, out var stack))
            {
                int order = (byte)layer * StackInterval;
                GameObject go = new GameObject(layer.ToString());
                go.layer = LayerMask.NameToLayer("UI");
                RectTransform rectTrans = go.AddComponent<RectTransform>();
                go.transform.SetParent(UIRoot);

                rectTrans.anchorMin = Vector2.zero;
                rectTrans.anchorMax = Vector2.one;
                rectTrans.localScale = Vector3.one;
                rectTrans.sizeDelta = Vector2.one;
                stack = new UIWindowStack(order, go.transform);
                uiStacks.Add(layer, stack);
            }

            return stack;
        }

        public T OpenSync<T>() where T : UIWindow
        {
            if (!uiConfigs.TryGetValue(typeof(T), out var config))
            {
                PKLogger.LogError($"Do not have UIWindow. Type: {typeof(T).Name}");
                return null;
            }

            var stack = GetUIStack(config.uiLayer);
            
            //如果不是多实例的窗口，并且已有实例，则找到并提到最顶层
            if (!config.isMultiple)
            {
                
                if (stack.CheckWindow<T>())
                {
                    return stack.RefocusWindow<T>();
                }
            }

            GameObject go = UIWindowHold.Get(config.path, stack.Parent);
            Transform parent = UIRoot;
            ulong uniqId = index++;
            T window = Activator.CreateInstance(typeof(T), config, parent, go, uniqId) as T;
            stack.AddWindow(window);

            return window;
        }

        public T OpenAsync<T>() where T : UIWindow
        {
            if (!uiConfigs.TryGetValue(typeof(T), out var config))
            {
                PKLogger.LogError($"Do not have UIWindow. Type: {typeof(T).Name}");
            }

            var stack = GetUIStack(config.uiLayer);
            
            //如果不是多实例的窗口，并且已有实例，则找到并提到最顶层
            if (!config.isMultiple)
            {
                
                if (stack.CheckWindow<T>())
                {
                    return stack.RefocusWindow<T>();
                }
            }

            GameObject go = UIWindowHold.Get(config.path, stack.Parent);
            Transform parent = null;
            ulong uniqId = index++;
            T window = Activator.CreateInstance(typeof(T), config, parent, go, uniqId) as T;
            stack.AddWindow(window);

            return window;
        }

        public void Close(UIWindow window, ulong uniqId)
        {
            if (!uiConfigs.TryGetValue(window.GetType(), out var config))
            {
                PKLogger.LogError($"Do not have UIWindow. Type: {window.GetType().Name}");
                return;
            }

            var stack = GetUIStack(config.uiLayer);
            stack.CloseWindow(uniqId);
        }

        public void CloseAllWindow<T>() where T : UIWindow
        {
            if (!uiConfigs.TryGetValue(typeof(T), out var config))
            {
                PKLogger.LogError($"Do not have UIWindow. Type: {typeof(T).Name}");
                return;
            }

            var stack = GetUIStack(config.uiLayer);
            stack.CloseAllTypeWindow<T>();
        }

        public T GetWindow<T>() where T : UIWindow
        {
            if (!uiConfigs.TryGetValue(typeof(T), out var config))
            {
                PKLogger.LogError($"Do not have UIWindow. Type: {typeof(T).Name}");
                return null;
            }
            var stack = GetUIStack(config.uiLayer);
            return stack.GetWindow<T>();
        }

        public UIWindow GetTopUI(UILayer layer = UILayer.CommonUI)
        {
            var stack = GetUIStack(layer);
            return stack.GetTopWindow();
        }

        public void GetRootWindow()
        {

        }

        //关闭在该界面上层的
        public void CloseTopWindow(UILayer layer)
        {
            var stack = GetUIStack(layer);
            stack.CloseTopWindow();
        }

        public void CloseAll(UILayer layer)
        {

        }

        public void RestoreWindow()
        {

        }

        public void Clear()
        {
            foreach (var pair in uiStacks)
            {
                pair.Value.Clear();
            }
        }

        #region 生命周期
        public void Awake()
        {
            
        }

        public void Update()
        {
            
        }

        public void Destroy()
        {
            
        }
        
        #endregion

        //TODO
        public void RegisterUIWindow()
        {

            Assembly asm = Assembly.GetExecutingAssembly();
            Type[] types = asm.GetExportedTypes();
            foreach (var type in types)
            {
                if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(UIWindow)))
                {
                    var attr = type.GetCustomAttribute(typeof(UIWindowAttribute)) as UIWindowAttribute;
                    if (attr != null)
                    {
                        Debug.Log(attr.Config);
                        uiConfigs.Add(type, attr.Config);
                    }
                }

            }
        }

    }
}

