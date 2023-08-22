namespace PKFramework.Runtime.UI
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UIWindowAttribute : System.Attribute
    {
        public UIConfig Config
        {
            get;
            private set;
        }

        public UIWindowAttribute(string path, UILayer layer = UILayer.CommonUI, bool isFullScreen = true, bool isMultiple = false)
        {
            Config = new UIConfig(path, layer, isFullScreen, isMultiple);
        }
    }
}