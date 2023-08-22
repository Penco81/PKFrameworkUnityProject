using PKFramework.Runtime.Singleton;
using PKFramework.Core;

namespace PKFramework.Core.Manager
{
    
    public abstract class ManagerBase<T> : Singleton<T>, IManager where T : class, new()
    {

        protected ManagerBase():base()
        {
            
        }


        public virtual void Update(float elapseSeconds, float realElapseSeconds)
        {
            
        }

        public virtual void Shutdown()
        {
            
        }
    }
}