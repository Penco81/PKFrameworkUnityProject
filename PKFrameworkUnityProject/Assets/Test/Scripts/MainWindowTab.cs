using PKFramework.Runtime.UI;
using UnityEngine;

[UISubWindow("Test/GameAsset/Prefabs/UIWindows/MainWindowTab")]
public class MainWindowTab : UISubWindow
{

    public override void SetUiMapPath()
    {
        paths.Add("Text");
    }

    public MainWindowTab(Transform parent, GameObject go, UIWindowParam param) : base(parent, go, param)
    {
    }
}