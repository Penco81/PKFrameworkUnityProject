namespace PKFramework.Runtime.Pool
{
    public interface IRecyclable
    {
        /// <summary>
        /// 回收入池
        /// </summary>
        public void Clear();
    }
}