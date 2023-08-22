using PKFramework.Runtime.Pool;

namespace PKFramework.Runtime.Base
{
    public abstract class ParamBase : IRecyclable
    {
        public virtual void Clear()
        {
            ReferencePool.Release(this);
        }
    }
}