namespace PKFramework.Editor.BuildAsset
{
    public enum BundleMode
    {
        //根据group打包到一起
        PackTogether,
        
        //按分组的 Entries 中的 Entry 的名字打包。如果是文件则不包含后缀
        PackByEntry,
        
        //每个文件打一个包，包含后缀
        PackByFile,
        
        //按照每个文件夹打包
        PackByFolder,

        //按自定义包名的返回打包，如果没有实现默认按文件打包。
        PackByCustom
    }

    //构建Player资源分离模式
    public enum PlayerAssetsSplitMode
    {
        SplitByAssetPacksWithInstallTime,
        //包括所有资源
        IncludeAllAssets,
        //不包括任何资源
        ExcludeAllAssets
    }
}