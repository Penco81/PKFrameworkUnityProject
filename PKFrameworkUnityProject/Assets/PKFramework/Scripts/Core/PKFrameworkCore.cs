using System;
using System.Collections.Generic;
using PKFramework.Core.Manager;
using Unity.VisualScripting;

namespace PKFramework.Core
{
    public interface IManager
    {
        public virtual int Priority
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// 游戏框架模块轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public abstract void Update(float elapseSeconds, float realElapseSeconds);

        /// <summary>
        /// 关闭并清理游戏框架模块。
        /// </summary>
        public abstract void Shutdown();
    }
    
    public static class PKFrameworkCore
    {
        private static List<IManager> _managers = new List<IManager>();

        public static void RegisterManager<T>(T manager) where T : IManager
        {
            _managers.Add(manager);
            //管理器根据优先级排序
            _managers.Sort((IManager a, IManager b) => a.Priority.CompareTo(b.Priority));
        }

        public static void Update(float elapseSeconds, float realElapseSeconds)
        {
            foreach (var manager in _managers)
            {
                manager.Update(elapseSeconds, realElapseSeconds);
            }
        }
        
        public static void Shutdown()
        {
            foreach (var manager in _managers)
            {
                manager.Shutdown();
            }
            _managers.Clear();
        }
    }
}