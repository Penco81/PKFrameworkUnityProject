using PKFramework.Runtime.Pool;
using PKFramework.Runtime.Task;

namespace PKFramework.Runtime.Asset
{
    public abstract class LoadTaskBase : TaskBase, IRecyclable
    {
        private int _refCount;
        public string Path { get; set; }

        protected override void OnCompleted()
        {
            PKLogger.LogMessage($"Load {GetType().Name} {Path} {Result}.");
        }

        public void Release()
        {
            if (_refCount == 0)
            {
                PKLogger.LogError($"Release {GetType().Name} {Path} too many times {_refCount}.");
                return;
            } 

            _refCount--;
            if (_refCount > 0) return;
            
        }

        protected abstract void OnDispose();

        public void WaitForCompletion()
        {
            if (IsDone) 
                return;

            if (Status == TaskStatus.Wait) 
                Start();

            OnWaitForCompletion();
        }

        protected virtual void OnWaitForCompletion()
        {
        }
    }
}