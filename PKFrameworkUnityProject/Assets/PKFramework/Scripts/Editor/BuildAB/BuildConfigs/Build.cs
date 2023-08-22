using System;
using UnityEditor;
using UnityEngine;

namespace PKFramework.Editor.BuildAsset
{
    //Build相关参数
    [Serializable]
    public class BuildParameters
    {
        //Build的版本号
        public int buildNumber;
        //是否优化此次构建
        public bool optimizeDependentAssets = true;
        //强制重新构建
        public bool forceRebuild;
        //构建选项
        public BuildAssetBundleOptions options = BuildAssetBundleOptions.ChunkBasedCompression;
        //该构建项所持有的group
        public BuildGroup[] groups;
        //scriptableObject的名字
        public string name { get; set; }
    }

    //Build的配置Object
    [CreateAssetMenu(menuName = "AssetBuild/" + nameof(Build), fileName = nameof(Build))]
    public class Build : ScriptableObject
    {
        public BuildParameters parameters;
    }
}