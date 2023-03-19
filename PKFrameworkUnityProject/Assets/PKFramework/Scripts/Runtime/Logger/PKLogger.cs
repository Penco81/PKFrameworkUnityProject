using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PKFramework
{
    public static class PKLogger
    {
        private const string TAG = "[PKFramework]";
        
        public static void LogError(object msg, Object context = null)
        {
            Debug.LogError($"{TAG} {msg}", context);
        }

        public static void LogWarning(object msg, Object context = null)
        {
            Debug.LogWarning($"{TAG} {msg}", context);
        }

        public static void LogMessage(object msg, Object context = null)
        {
            Debug.Log($"{TAG} {msg}", context);
        }
    }
}

