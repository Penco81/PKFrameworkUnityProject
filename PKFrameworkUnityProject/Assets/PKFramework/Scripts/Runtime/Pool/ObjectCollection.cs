using System.Collections.Generic;
using UnityEngine;

namespace PKFramework.Runtime.Pool
{
    public class ObjectCollection
    {
        private Dictionary<string, GameObject> pathGo;

        public ObjectCollection()
        {
            pathGo = new Dictionary<string, GameObject>();
        }
        
        
    }
}