using PKFramework.Runtime.Asset;
using UnityEngine;

namespace PKFramework.Editor.BuildAsset
{
    //包组的设置
    [CreateAssetMenu(menuName = "AssetBuild/" + nameof(BuildGroup), fileName = nameof(BuildGroup))]
    public class BuildGroup : ScriptableObject
    {
        [Tooltip("打包策略")]
        public BundleMode bundleMode = BundleMode.PackByFile;
        
        [Tooltip("寻址模式")]
        public AddressMode addressMode = AddressMode.LoadByPath;
        
        [Tooltip("条目")]
        public Object[] entries;
        
        [Tooltip("过滤")]
        public string filter;
        
        //所属build的名字
        public string build { get; set; }
    }
}