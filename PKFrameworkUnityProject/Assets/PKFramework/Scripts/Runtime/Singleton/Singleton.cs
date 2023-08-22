using System.Threading;

namespace PKFramework.Runtime.Singleton
{
    /// <summary>
    /// 全局单例对象（线程安全），使用lock
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> where T : class, new()
    {
        protected static T instance = default(T);
        private static object mLock = new object();
        
        public static T Instance
        {
            get
            {
                lock (mLock)
                {
                    if (null == instance)
                    {
                        instance = new T();
                    }
                }
                
                return instance;
            }
        }

        protected Singleton()
        {

        }

        public virtual void Release()
        {
            if (instance != null)
            {
                instance = null;
            }
        }
    }
}