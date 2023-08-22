using System.Collections.Generic;
using PKFramework.Core.Net.Data;

namespace PKFramework.Core.Data
{
    public class DataCenter
    {
        private string _name;
        private Dictionary<string, DataNode> dataMap;
        
        public string Name => _name;

        public Dictionary<string, DataNode> DataMap => dataMap;

        public DataCenter(string name, PackBase pack = null)
        {
            _name = name;
            InitData();
        }

        public virtual void InitData()
        {
            
        }

        public virtual void AddDataNode()
        {
            
        }

        public virtual void DeleteDataNode()
        {
            
        }

        public virtual DataNode GetDataNode()
        {
            return null;
        }

        public void Clear()
        {
            dataMap.Clear();
        }
    }
}