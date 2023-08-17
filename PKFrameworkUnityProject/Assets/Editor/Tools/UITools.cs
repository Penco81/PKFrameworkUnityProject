using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Clipboard = System.Windows.Forms.Clipboard;

[InitializeOnLoad]
public class UITools
{
    const string kUILayerName = "UI";
    
    static UITools()
    {
        ObjectFactory.componentWasAdded += ComponentWasAdded;
    }

    /// <summary>
    /// 覆盖原有的创建Text
    /// </summary>
    [UnityEditor.MenuItem("GameObject/UI/Text")]
    public static void CreateText()
    {
        Text text = CreateComponent<Text>();
        ComponentOptimizing.OptimizingText(text);
    }

    /// <summary>
    /// 覆盖原有的创建Image
    /// </summary>
    public static void CreateImage()
    {
        Image image = CreateComponent<Image>();
        ComponentOptimizing.OptimizingImage(image);
    }

    /// <summary>
    /// 覆盖原有的创建RawImage
    /// </summary>
    public static void CreateRawImage()
    {
        RawImage rawImage = CreateComponent<RawImage>();
        ComponentOptimizing.OptimizingRawImage(rawImage);
    }
    
    private static T CreateComponent<T>() where T : Component
    {
        string name = typeof(T).Name;
        GameObject go = new GameObject(name);
        Transform parent = GetUIParent();
        go.transform.SetParent(parent, false);
        T com = go.AddComponent<T>();
        return com;
    }
    
    private static Transform GetUIParent()
    {
        Transform select = Selection.activeTransform;
        if (select)
        {
            if (select.GetComponentInParent<Canvas>())
            {
                return select;
            }
            else
            {
                Canvas canvas = select.GetComponentInChildren<Canvas>();
                if (canvas == null)
                {
                    canvas = InstanceCanvas(select);
                }
                return canvas.transform;
            }
        }
        else
        {
            Canvas canvas = GetCanvasInScene();
            if (canvas == null)
            {
                canvas = InstanceCanvas();
            }
            return canvas.transform;
        }
    }
    
    /// <summary>
    /// 参考PrefabStageUtility.HandleUIReparentingIfNeeded
    /// </summary>
    /// <returns></returns>
    static Canvas GetCanvasInScene()
    {
        // TODO 为什么会找不到？
        //Canvas canvas = Object.FindObjectOfType<Canvas>();
        Scene scene = SceneManager.GetActiveScene();
        foreach (GameObject go in scene.GetRootGameObjects())
        {
            // Do not search for Canvas's under the prefab root since we want to
            // have a Canvas for the prefab root
            var canvas = go.GetComponentInChildren<Canvas>();
            if (canvas != null)
                return canvas;
        }
        return null;
    }

    /// <summary>
    /// 参考PrefabStageUtility.HandleUIReparentingIfNeeded
    /// </summary>
    /// <returns></returns>
    private static Canvas InstanceCanvas(Transform parent = null)
    {
        // Create canvas root for the UI
        GameObject root = EditorUtility.CreateGameObjectWithHideFlags("Canvas", HideFlags.DontSave);
        root.layer = LayerMask.NameToLayer(kUILayerName);
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        root.AddComponent<CanvasScaler>();
        if (parent)
        {
            canvas.transform.SetParent(parent, false);
        }
        return canvas;
    }

    public static void ComponentWasAdded(Component com)
    {
        var text = com as Text;
        if (text != null)
        {
            ComponentOptimizing.OptimizingText(text);
        }
        
        var image = com as Image;
        if (image != null)
        {
            ComponentOptimizing.OptimizingImage(image);
        }
        
        var rawImage = com as RawImage;
        if (rawImage != null)
        {
            ComponentOptimizing.OptimizingRawImage(rawImage);
        }
    }

    static string GetRelativePath(Transform node)
    {
        string path = "";
        bool firstFlag = true;
        Transform parent = node;
        while(parent != null && (!parent.name.Contains("Panel") && !parent.name.Contains("Block") && !parent.name.Contains("Window")))
        {
            path = parent.name + (firstFlag? "" : "/") + path;
            firstFlag = false;
            parent = parent.parent;
        }
        return path;
    }

    [UnityEditor.MenuItem("GameObject/复制UI相对路径")]
    public static void CopyRelativePath()
    {
        Transform select = Selection.activeTransform;
        
        if (select)
        {
            var path = GetRelativePath(select);
            
            Clipboard.SetText(path);
        }
        else
        {
            Debug.LogError("请选择一个节点");
        }
    }

    enum UICom
    {
        GameObject,
        Transform,
        Text,
        Image,
        Button,
    }

    static void GenLuaUIComString(Transform target, UICom uiCom)
    {
        
        if (target)
        {
            //eg: self.titleText = UtilsUI.GetText(transform, "main/title")
            string varName = target.name;
            string path = GetRelativePath(target);
            string res = string.Format($"self.{varName} = UtilsUI.Get{uiCom.ToString()}(transform, \"{path}\")");
            Clipboard.SetText(res);
        }
        else
        {
            Debug.LogError("请选择一个节点");
        }
    }

    [UnityEditor.MenuItem("GameObject/快速生成Lua代码/快生Lua组件绑定代码之GameObject")]
    public static void GenLuaGameObjectBind()
    {
        Transform select = Selection.activeTransform;
        GenLuaUIComString(select, UICom.GameObject);
    }
    
    [UnityEditor.MenuItem("GameObject/快速生成Lua代码/快生Lua组件绑定代码之Transform")]
    public static void GenLuaTransformBind()
    {
        Transform select = Selection.activeTransform;
        GenLuaUIComString(select, UICom.Transform);

    }
    
    [UnityEditor.MenuItem("GameObject/快速生成Lua代码/快生Lua组件绑定代码之Text")]
    public static void GenLuaTextBind()
    {
        Transform select = Selection.activeTransform;
        GenLuaUIComString(select, UICom.Text);
    }
    
    [UnityEditor.MenuItem("GameObject/快速生成Lua代码/快生Lua组件绑定代码之Image")]
    public static void GenLuaImageBind()
    {
        Transform select = Selection.activeTransform;
        GenLuaUIComString(select, UICom.Image);
    }
    
    [UnityEditor.MenuItem("GameObject/快速生成Lua代码/快生Lua组件绑定代码之Button")]
    public static void GenLuaButtonBind()
    {
        Transform select = Selection.activeTransform;
        GenLuaUIComString(select, UICom.Button);
    }
    
    [UnityEditor.MenuItem("GameObject/检测UI界面下是否有z不为0的节点")]
    public static void CheckZZero()
    {
        Transform select = Selection.activeTransform;
        if (select)
        {
            CheckChildZ(select, "");
        }
        else
        {
            Debug.LogError("请选择一个节点");
        }
    }

    static void CheckChildZ(Transform root, string path)
    {
        var childCount = root.childCount;
        for (int i = 0; i < childCount; i++)
        {
            var child = root.GetChild(i);
            string newPath = path + "/" + child.name;
            if (child.transform.localPosition.z != 0)
            {
                Debug.LogError($"节点路径 {newPath} z值不为0");
            }
            CheckChildZ(child, newPath);
        }
    }
    
    #region 创建标准预制体
    public static string normalTextPath = "Assets/Editor/MgUITemplate/Text.prefab";
    public static string titleTextPath = "Assets/Editor/MgUITemplate/Title.prefab";
    public static string normalButtonPath = "Assets/Editor/MgUITemplate/Button.prefab";
    public static string tabGroupPath = "Assets/Editor/MgUITemplate/TapGroup.prefab";
    public static string scrollViewPath = "Assets/Editor/MgUITemplate/ScrollView.prefab";

    static void CreateMgTemplate(Transform parent, string path)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError($"未加载到prefab，请检查路径以及prefab！ 路径： {path}");
            return;
        }

        var instance = GameObject.Instantiate(prefab, parent);
        instance.name = prefab.name;
    }

    [UnityEditor.MenuItem("GameObject/暮光预制体/创建Text")]
    public static void CreateMgNormalText()
    {
        Transform select = Selection.activeTransform;
        if (select)
        {
            CreateMgTemplate(select, normalTextPath);
        }
        else
        {
            Debug.LogError("请选择一个节点");
        }
    }
    
    [UnityEditor.MenuItem("GameObject/暮光预制体/创建Title")]
    public static void CreateMgTitleText()
    {
        Transform select = Selection.activeTransform;
        if (select)
        {
            CreateMgTemplate(select, titleTextPath);
        }
        else
        {
            Debug.LogError("请选择一个节点");
        }
    }
    
    [UnityEditor.MenuItem("GameObject/暮光预制体/创建Button")]
    public static void CreateMgNormalButton()
    {
        Transform select = Selection.activeTransform;
        if (select)
        {
            CreateMgTemplate(select, normalButtonPath);
        }
        else
        {
            Debug.LogError("请选择一个节点");
        }
    }
    
    [UnityEditor.MenuItem("GameObject/暮光预制体/创建TabGroup")]
    public static void CreateMgTabGroup()
    {
        Transform select = Selection.activeTransform;
        if (select)
        {
            CreateMgTemplate(select, tabGroupPath);
        }
        else
        {
            Debug.LogError("请选择一个节点");
        }
    }
    
    [UnityEditor.MenuItem("GameObject/暮光预制体/创建ScrollView")]
    public static void CreateMgScrollView()
    {
        Transform select = Selection.activeTransform;
        if (select)
        {
            CreateMgTemplate(select, scrollViewPath);
        }
        else
        {
            Debug.LogError("请选择一个节点");
        }
    }
    
    #endregion
}
