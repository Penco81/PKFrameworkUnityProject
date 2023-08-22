using PKFramework.Runtime.UI;
using UnityEngine;

[UIWindow("Test/GameAsset/Prefabs/UIWindows/Window1", UILayer.CommonUI, false)]
public class Window1 : UIWindow
{


    public override void SetUiMapPath()
    {
        paths.Add("Text");
    }

    public Window1(UIConfig uiConfig, Transform parent, GameObject go, ulong uniqId) : base(uiConfig, parent, go, uniqId)
    {
    }
}