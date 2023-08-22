using System;

namespace PKFramework.Runtime.Asset
{
    /// <summary>
    /// 自定义的可以下载的信息文件
    /// </summary>
    [Serializable]
    public class Download
    {
        //名字
        public string name;
        public string hash;
        public ulong size;
        //文件名
        public string file { get; set; }
    }
}