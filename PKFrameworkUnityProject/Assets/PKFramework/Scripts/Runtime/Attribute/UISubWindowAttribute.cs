using System;

namespace PKFramework.Runtime.UI
{
    public class UISubWindowAttribute : Attribute
    {
        public string Path
        {
            get;
            private set;
        }

        public UISubWindowAttribute(string path)
        {
            Path = path;
        }
    }
}