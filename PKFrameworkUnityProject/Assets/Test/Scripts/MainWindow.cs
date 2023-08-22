using PKFramework.Runtime.UI;
using UnityEngine;

[UIWindow("Test/GameAsset/Prefabs/UIWindows/MainWindow")]
public class MainWindow : UIWindow
{


    public override void SetUiMapPath()
    {
        paths.Add("Text");
        paths.Add("Button");
        paths.Add("Button/Text");
    }

    public MainWindow(UIConfig uiConfig, Transform parent, GameObject go, ulong uniqId) : base(uiConfig, parent, go, uniqId)
    {
    }
}