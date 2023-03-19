using System;
using System.Collections.Generic;

namespace PKFramework.Runtime.Pool
{
    /// <summary>
    /// 用于纯数据结构类的引用池
    /// </summary>
    public static class ReferencePool
    {
        private static readonly Dictionary<Type, ReferenceCollection> referenceCollections = new Dictionary<Type, ReferenceCollection>();

        /// <summary>
        /// 获取内存池种类的数量。
        /// </summary>
        public static int Count
        {
            get
            {
                return referenceCollections.Count;
            }
        }

        /// <summary>
        /// 清除所有内存池。
        /// </summary>
        public static void ClearAll()
        {
            lock (referenceCollections)
            {
                foreach (KeyValuePair<Type, ReferenceCollection> memoryCollection in referenceCollections)
                {
                    memoryCollection.Value.RemoveAll();
                }

                referenceCollections.Clear();
            }
        }

        /// <summary>
        /// 从内存池获取内存对象。
        /// </summary>
        /// <typeparam name="T">内存对象类型。</typeparam>
        /// <returns>内存对象。</returns>
        public static T Acquire<T>() where T : class, IRecyclable, new()
        {
            return GetReferenceCollection(typeof(T)).Acquire<T>();
        }

        /// <summary>
        /// 从内存池获取内存对象。
        /// </summary>
        /// <param name="referenceType">内存对象类型。</param>
        /// <returns>内存对象。</returns>
        public static IRecyclable Acquire(Type referenceType)
        {
            InternalCheckType(referenceType);
            return GetReferenceCollection(referenceType).Acquire();
        }

        /// <summary>
        /// 将内存对象归还内存池。
        /// </summary>
        /// <param name="reference">内存对象。</param>
        public static void Release(IRecyclable reference)
        {
            if (reference == null)
            {
                throw new Exception("ReferenceType is invalid.");
            }

            Type referenceType = reference.GetType();
            InternalCheckType(referenceType);
            GetReferenceCollection(referenceType).Release(reference);
        }

        /// <summary>
        /// 向内存池中追加指定数量的内存对象。
        /// </summary>
        /// <typeparam name="T">内存对象类型。</typeparam>
        /// <param name="count">追加数量。</param>
        public static void Add<T>(int count) where T : class, IRecyclable, new()
        {
            GetReferenceCollection(typeof(T)).Add<T>(count);
        }

        /// <summary>
        /// 向内存池中追加指定数量的内存对象。
        /// </summary>
        /// <param name="memoryType">内存对象类型。</param>
        /// <param name="count">追加数量。</param>
        public static void Add(Type memoryType, int count)
        {
            InternalCheckType(memoryType);
            GetReferenceCollection(memoryType).Add(count);
        }

        /// <summary>
        /// 从内存池中移除指定数量的内存对象。
        /// </summary>
        /// <typeparam name="T">内存对象类型。</typeparam>
        /// <param name="count">移除数量。</param>
        public static void Remove<T>(int count) where T : class, IRecyclable
        {
            GetReferenceCollection(typeof(T)).Remove(count);
        }

        /// <summary>
        /// 从内存池中移除指定数量的内存对象。
        /// </summary>
        /// <param name="memoryType">内存对象类型。</param>
        /// <param name="count">移除数量。</param>
        public static void Remove(Type memoryType, int count)
        {
            InternalCheckType(memoryType);
            GetReferenceCollection(memoryType).Remove(count);
        }

        /// <summary>
        /// 从内存池中移除所有的内存对象。
        /// </summary>
        /// <typeparam name="T">内存对象类型。</typeparam>
        public static void RemoveAll<T>() where T : class, IRecyclable
        {
            GetReferenceCollection(typeof(T)).RemoveAll();
        }

        /// <summary>
        /// 从内存池中移除所有的内存对象。
        /// </summary>
        /// <param name="memoryType">内存对象类型。</param>
        public static void RemoveAll(Type memoryType)
        {
            InternalCheckType(memoryType);
            GetReferenceCollection(memoryType).RemoveAll();
        }

        private static void InternalCheckType(Type referenceType)
        {

            if (!referenceType.IsClass || referenceType.IsAbstract)
            {
                PKLogger.LogError("ReferenceType is not a non-abstract class type.");
            }

            if (!typeof(IRecyclable).IsAssignableFrom(referenceType))
            {
                PKLogger.LogError($"ReferenceType '{referenceType.FullName}' is invalid.");
            }
        }

        private static ReferenceCollection GetReferenceCollection(Type referenceType)
        {
            if (referenceType == null)
            {
                PKLogger.LogError("ReferenceType is invalid.");
            }

            ReferenceCollection referenceCollection = null;
            lock (referenceCollections)
            {
                if (!referenceCollections.TryGetValue(referenceType, out referenceCollection))
                {
                    referenceCollection = new ReferenceCollection(referenceType);
                    referenceCollections.Add(referenceType, referenceCollection);
                }
            }

            return referenceCollection;
        }
    }
}