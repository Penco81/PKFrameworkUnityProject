using System;
using System.Collections;
using PKFramework.Runtime.Pool;
using UnityEditor.PackageManager.Requests;

namespace PKFramework.Runtime.Task
{
    public abstract class TaskBase : IEnumerator, IRecyclable
    {
        public enum TaskResult
        {
            Default,
            Success,
            Failed,
            Cancelled
        }

        public enum TaskStatus
        {
            Wait,
            Processing,
            Complete
        }

        public Action<TaskBase> Completed; 
        public TaskResult Result { get; protected set; } = TaskResult.Default;
        public TaskStatus Status { get; protected set; } = TaskStatus.Wait;
        public bool IsDone => Status == TaskStatus.Complete;
        public float Progress { get; set; }
        public string Error { get; protected set; }

        public TaskBase()
        {
        }

        public bool MoveNext()
        {
            return !IsDone;
        }

        public void Reset()
        {
            Completed = null;
            Status = TaskStatus.Wait;
        }

        public object Current => null;
        
        public bool Update()
        {
            if (IsDone) 
                return false;
            OnUpdated();
            return true;
        }

        public void SetResult(TaskResult value, string msg = null)
        {
            Progress = 1;
            Result = value;
            Status = Result == TaskResult.Default ? TaskStatus.Wait : TaskStatus.Complete;
            Error = msg;
        }

        public void Start()
        {
            if (Status != TaskStatus.Wait) return;
            Status = TaskStatus.Processing;
            OnStart();
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnUpdated()
        {
        }

        protected virtual void OnCompleted()
        {
        }

        public void Complete()
        {
            OnCompleted();
            var saved = Completed;
            Completed?.Invoke(this);
            Completed -= saved;
            Clear();
        }

        public void Cancel()
        {
            SetResult(TaskResult.Cancelled);
        }

        public void Clear()
        {
            ReferencePool.Release(this);
        }
    }
}