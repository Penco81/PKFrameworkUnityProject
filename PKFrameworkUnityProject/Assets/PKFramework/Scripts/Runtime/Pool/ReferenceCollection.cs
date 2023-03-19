using System;
using System.Collections.Generic;

namespace PKFramework.Runtime.Pool
{
    /// <summary>
    /// 内存池收集器
    /// </summary>
    public sealed class ReferenceCollection
    {
        private readonly Queue<IRecyclable> references;
        private readonly Type referenceType;
        private int usingMemoryCount;
        private int acquireMemoryCount;
        private int releaseMemoryCount;
        private int addMemoryCount;
        private int removeMemoryCount;

        public ReferenceCollection(Type memoryType)
        {
            references = new Queue<IRecyclable>();
            referenceType = memoryType;
            usingMemoryCount = 0;
            acquireMemoryCount = 0;
            releaseMemoryCount = 0;
            addMemoryCount = 0;
            removeMemoryCount = 0;
        }

        public Type MemoryType
        {
            get
            {
                return referenceType;
            }
        }

        public int UnusedMemoryCount
        {
            get
            {
                return references.Count;
            }
        }

        public int UsingMemoryCount
        {
            get
            {
                return usingMemoryCount;
            }
        }

        public int AcquireMemoryCount
        {
            get
            {
                return acquireMemoryCount;
            }
        }

        public int ReleaseMemoryCount
        {
            get
            {
                return releaseMemoryCount;
            }
        }

        public int AddMemoryCount
        {
            get
            {
                return addMemoryCount;
            }
        }

        public int RemoveMemoryCount
        {
            get
            {
                return removeMemoryCount;
            }
        }

        public T Acquire<T>() where T : class, IRecyclable, new()
        {
            if (typeof(T) != referenceType)
            {
                PKLogger.LogError($"Acquire Type illegally. Type must be {referenceType.Name}, but the type is ${typeof(T).Name}");
            }

            usingMemoryCount++;
            acquireMemoryCount++;
            lock (references)
            {
                if (references.Count > 0)
                {
                    return (T)references.Dequeue();
                }
            }

            addMemoryCount++;
            T newT = new T();
            return newT;
        }

        public IRecyclable Acquire()
        {
            usingMemoryCount++;
            acquireMemoryCount++;
            lock (references)
            {
                if (references.Count > 0)
                {
                    return references.Dequeue();
                }
            }

            addMemoryCount++;
            var newT = (IRecyclable)Activator.CreateInstance(referenceType);
            return newT;
        }

        public void Release(IRecyclable memory)
        {
            memory.Clear();
            lock (references)
            {
                if (references.Contains(memory))
                {
                    PKLogger.LogError("The memory has been released.");
                }

                references.Enqueue(memory);
            }

            releaseMemoryCount++;
            usingMemoryCount--;
        }

        public void Add<T>(int count) where T : class, IRecyclable, new()
        {
            if (typeof(T) != referenceType)
            {
                PKLogger.LogError($"Add Type illegally. Type must be {referenceType.Name}, but the type is ${typeof(T).Name}");
            }

            lock (references)
            {
                addMemoryCount += count;
                while (count-- > 0)
                {
                    references.Enqueue(new T());
                }
            }
        }

        public void Add(int count)
        {
            lock (references)
            {
                addMemoryCount += count;
                while (count-- > 0)
                {
                    references.Enqueue((IRecyclable)Activator.CreateInstance(referenceType));
                }
            }
        }

        public void Remove(int count)
        {
            lock (references)
            {
                if (count > references.Count)
                {
                    count = references.Count;
                }

                removeMemoryCount += count;
                while (count-- > 0)
                {
                    references.Dequeue();
                }
            }
        }

        public void RemoveAll()
        {
            lock (references)
            {
                removeMemoryCount += references.Count;
                references.Clear();
            }
        }
    }
}