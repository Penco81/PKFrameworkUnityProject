using System;
using UnityEngine;
using PKFramework.Runtime.Asset;

namespace PKFramework.Editor.BuildAsset
{
    //所有要打包的资源的信息数据结构
    [Serializable]
    public class BuildAsset
    {
        //路径
        public string path;
        //所属包名
        public string bundle;
        //该资产的类型
        public string type;
        //所属entry
        public string entry;
        //寻址模式
        public AddressMode addressMode = AddressMode.LoadByPath;
        //依赖的资源路径名
        public string[] dependencies = Array.Empty<string>();
        //所属的buildgroup
        public BuildGroup group;
    }
    
    public class BuildAssets : ScriptableObject
    {
        public BuildAsset[] bundledAssets = Array.Empty<BuildAsset>();
    }
}