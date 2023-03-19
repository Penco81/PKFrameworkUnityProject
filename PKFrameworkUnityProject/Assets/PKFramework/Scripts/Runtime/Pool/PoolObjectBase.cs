using System;

namespace PKFramework.Runtime.Pool
{
    public abstract class PoolObjectBase : IRecyclable
    {
        private string key;
        private bool loaded;

        public PoolObjectBase(string key)
        {
            
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public void Load()
        {
            throw new System.NotImplementedException();
        }
    }
}