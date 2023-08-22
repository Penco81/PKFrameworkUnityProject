using PKFramework.Runtime.UI;
using UnityEngine;

[UIWindow("Test/GameAsset/Prefabs/UIWindows/Window2", UILayer.CommonUI, true)]
public class Window2 : UIWindow
{


    public override void SetUiMapPath()
    {
        paths.Add("Text");
    }

    public Window2(UIConfig uiConfig, Transform parent, GameObject go, ulong uniqId) : base(uiConfig, parent, go, uniqId)
    {
    }
}