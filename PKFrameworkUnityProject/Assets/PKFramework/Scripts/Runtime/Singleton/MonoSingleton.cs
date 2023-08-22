using UnityEngine;

namespace PKFramework.Runtime.Singleton
{
    /// <summary>
    /// 线程安全，因为Mono始终在一个线程上执行
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    T result = FindObjectOfType<T>();
                    if (result == null)
                    {
                        GameObject obj = new GameObject(nameof(T));
                        result = obj.AddComponent<T>();
                        DontDestroyOnLoad(obj);
                    }

                    instance = result;
                }

                return instance;
            }
        }

        protected virtual void OnDestroy()
        {
            instance = null;
        }
    }
}