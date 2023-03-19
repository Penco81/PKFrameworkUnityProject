using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PKFramework
{

    public struct UIConfig
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

    public enum UIName
    {
        Test = 0,
    }

    public class UIManager
    {
        public Dictionary<UIName, UIConfig> uiDict = new Dictionary<UIName, UIConfig>{
            //eg.
            //{UIName.Test, new UIConfig("")},
        };

        public Stack<BasePanel> uiStack;
        
        public void Open(UIName uiName)
        {
            UIConfig uiConfig;
            if(!uiDict.TryGetValue(uiName, out uiConfig))
            {
                PKLogger.LogError($"UIPanel doesn't exist in dict, name is {uiName}");
                return;
            }
        }

        public void OpenSync()
        {

        }

        public void Close()
        {
            
        }

        public void GetUIPanel()
        {

        }

        public void GetTopsideUIPanel()
        {

        }

        public void GetRootUIPanel()
        {

        }

        //关闭最上层的
        public void CloseTopsideUIPanel()
        {

        }

        //关闭在该界面上层的
        public void CloseTopUIPanel(int panel)
        {
            
        }

        public void CloseAll(UILayer layer)
        {

        }

        public void RestoreUI()
        {

        }

        public void Clear()
        {

        }
    }
}

