using System;
using System.Collections.Generic;
using PKFramework.Runtime.Event;

namespace PKFramework.Core.Data
{
    public abstract class DataNode
    {
        private DataCenter _center;
        
        private string _name;
        
        public DataCenter Center => _center;

        public string Name => _name;

        public virtual void OnValueChange(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                PKLogger.LogError($"Try to Dispatch a null name event in datanode. Name {_name}");
                return;
            }

            EventParamsBase param = null;
            string centerName = _center?.Name ?? "";
            string nodeName = _name ?? "";
            string path = Utility.Text.Format("{0}.{1}.{2}", centerName, nodeName, name);
            EventManager.Instance.Dispatch(EventManager.EventCenterType.Data, path, param);
        }
    }
}