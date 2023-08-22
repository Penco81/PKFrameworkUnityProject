namespace PKFramework.Core.DataTable
{
    public abstract class DataRowBase
    {
        public abstract int Id { get; }
        
        public abstract string this[string name] { get; }

        public abstract bool ParseDataRow(string dataRowString);

        public abstract bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length);
    }
}