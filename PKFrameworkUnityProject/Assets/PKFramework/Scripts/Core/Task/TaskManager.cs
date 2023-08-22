using System;
using System.Collections.Generic;
using PKFramework.Core.Manager;
using PKFramework.Runtime.Pool;
using PKFramework.Runtime.Singleton;
using PKFramework.Runtime.Task;
using UnityEngine;

namespace PKFramework.Runtime.Task
{
    public class TaskManager : ManagerBase<TaskManager>
    {
        private readonly Dictionary<string, TaskQueue> _Queues = new Dictionary<string, TaskQueue>();
        private readonly List<TaskQueue> Queues = new List<TaskQueue>();
        private readonly Queue<TaskQueue> Append = new Queue<TaskQueue>();
        private float _realtimeSinceStartup;
        
        private byte maxRequests = 10;
        
        private float maxUpdateTimeSlice = 1 / 60f;

        private bool autoSlicing = true;
        public bool AutoSlicing { get; set; } = true;
        public bool Working => Queues.Exists(o => o.Working);
        public bool Busy => AutoSlicing && Time.realtimeSinceStartup - _realtimeSinceStartup > MaxUpdateTimeSlice;
        public float MaxUpdateTimeSlice { get; set; }
        public byte MaxRequests { get; set; } = 10;

        public T CreateTask<T>() where T : TaskBase, new()
        {
            T task = ReferencePool.Acquire<T>();
            Enqueue(task);
            return task;
        }

        private void Start()
        {
            AutoSlicing = autoSlicing;
            MaxUpdateTimeSlice = maxUpdateTimeSlice;
            MaxRequests = maxRequests;
        }

        private void Update()
        {
            _realtimeSinceStartup = Time.realtimeSinceStartup;
            while (Append.Count > 0)
            {
                var item = Append.Dequeue();
                Queues.Add(item);
            }

            foreach (var queue in Queues)
                if (!queue.Update())
                    break;
        }

        public void Enqueue(TaskBase task)
        {
            var key = task.GetType().Name;
            if (!_Queues.TryGetValue(key, out var queue))
            {
                queue = new TaskQueue() {Key = key, MaxRequests = MaxRequests};
                _Queues.Add(key, queue);
                Append.Enqueue(queue);
                // TODO: 这里可以考虑给 Task 加个优先级。
            }

            queue.Enqueue(task);
        }
    }
}