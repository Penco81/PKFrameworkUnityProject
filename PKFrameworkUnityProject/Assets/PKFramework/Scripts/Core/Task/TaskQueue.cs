using System;
using System.Collections;
using System.Collections.Generic;

namespace PKFramework.Runtime.Task
{
    public class TaskQueue
    {
        private readonly List<TaskBase> processing = new List<TaskBase>();
        private readonly Queue<TaskBase> queue = new Queue<TaskBase>();
        
        public string Key { get; set; }
        public byte MaxRequests { get; set; } = 10;
        public bool Working => processing.Count > 0 || queue.Count > 0;

        public void Enqueue(TaskBase request)
        {
            queue.Enqueue(request);
        }

        public bool Update()
        {
            while (queue.Count > 0 && (processing.Count < MaxRequests || MaxRequests == 0))
            {
                var item = queue.Dequeue();
                processing.Add(item);
                if (item.Status == TaskBase.TaskStatus.Wait) 
                    item.Start();
                if (TaskManager.Instance.Busy) 
                    return false;
            }

            for (var index = 0; index < processing.Count; index++)
            {
                var item = processing[index];
                if (item.Update()) 
                    continue;
                processing.RemoveAt(index);
                index--;
                item.Complete();
                if (TaskManager.Instance.Busy) 
                    return false;
            }
            return true;
        }
    }
}