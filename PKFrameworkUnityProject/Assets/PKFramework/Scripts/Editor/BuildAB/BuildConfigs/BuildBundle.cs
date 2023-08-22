using System;
using System.Collections.Generic;

namespace PKFramework.Editor.BuildAsset
{
    //ab的抽象信息结构
    [Serializable]
    public class BuildBundle
    {
        //包id
        public int id;
        //包依赖
        public int[] deps = Array.Empty<int>();
        //缓存目录下的包名
        public string group;
        //该包的hash
        public string hash;
        //数据目录下的包名
        public string file;
        //包大小，单位字节
        public ulong size;
        //包持有的asset路径名
        public List<string> assets = new List<string>();
    }
}