
using UnityEngine;
using Object = UnityEngine.Object;
namespace PKFramework.Runtime
{
    /// <summary>
    /// 加载资源状态。
    /// </summary>
    public enum LoadResourceStatus : byte
    {
        /// <summary>
        /// 加载资源完成。
        /// </summary>
        Success = 0,

        /// <summary>
        /// 资源不存在。
        /// </summary>
        NotExist,

        /// <summary>
        /// 资源尚未准备完毕。
        /// </summary>
        NotReady,

        /// <summary>
        /// 依赖资源错误。
        /// </summary>
        DependencyError,

        /// <summary>
        /// 资源类型错误。
        /// </summary>
        TypeError,

        /// <summary>
        /// 加载资源错误。
        /// </summary>
        AssetError
    }
    
    /// <summary>
    /// 加载资源成功回调函数。
    /// </summary>
    /// <param name="assetName">要加载的资源名称。</param>
    /// <param name="asset">已加载的资源。</param>
    /// <param name="duration">加载持续时间。</param>
    /// <param name="userData">用户自定义数据。</param>
    public delegate void LoadAssetSuccessCallback(string assetName, Object asset, float duration, object userData);
        
    /// <summary>
    /// 加载资源失败回调函数。
    /// </summary>
    /// <param name="assetName">要加载的资源名称。</param>
    /// <param name="status">加载资源状态。</param>
    /// <param name="errorMessage">错误信息。</param>
    /// <param name="userData">用户自定义数据。</param>
    public delegate void LoadAssetFailureCallback(string assetName, LoadResourceStatus status, string errorMessage, object userData);
        
    /// <summary>
    /// 加载资源更新回调函数。
    /// </summary>
    /// <param name="assetName">要加载的资源名称。</param>
    /// <param name="progress">加载资源进度。</param>
    /// <param name="userData">用户自定义数据。</param>
    public delegate void LoadAssetUpdateCallback(string assetName, float progress, object userData);
        
    /// <summary>
    /// 加载资源时加载依赖资源回调函数。
    /// </summary>
    /// <param name="assetName">要加载的资源名称。</param>
    /// <param name="dependencyAssetName">被加载的依赖资源名称。</param>
    /// <param name="loadedCount">当前已加载依赖资源数量。</param>
    /// <param name="totalCount">总共加载依赖资源数量。</param>
    /// <param name="userData">用户自定义数据。</param>
    public delegate void LoadAssetDependencyAssetCallback(string assetName, string dependencyAssetName, int loadedCount, int totalCount, object userData);

    
    /// <summary>
    /// 加载资源回调函数集。
    /// </summary>
    public sealed class LoadAssetCallbacks
    {
        public readonly LoadAssetSuccessCallback loadAssetSuccessCallback;
        public readonly LoadAssetFailureCallback loadAssetFailureCallback;
        public readonly LoadAssetUpdateCallback loadAssetUpdateCallback;
        public readonly LoadAssetDependencyAssetCallback loadAssetDependencyAssetCallback;
        
        public LoadAssetCallbacks(LoadAssetSuccessCallback load1 = null, LoadAssetFailureCallback load2 = null, 
            LoadAssetUpdateCallback load3 = null, LoadAssetDependencyAssetCallback load4 = null)
        {
            loadAssetSuccessCallback = load1;
            loadAssetFailureCallback = load2;
            loadAssetUpdateCallback = load3;
            loadAssetDependencyAssetCallback = load4;
        }
    }
}
