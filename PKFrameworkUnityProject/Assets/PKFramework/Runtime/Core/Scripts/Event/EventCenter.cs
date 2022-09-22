using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PKFramework
{
    public class EventParamsBase
    {
        
    }

    public class EventCenter<T1, T2> where  T1:System.Enum where T2 : EventParamsBase 
    {

        public Dictionary<T1, List< Action<T2>> > eventDict;

        public void Subscribe(T1 key, Action<T2> callback)
        {
            List< Action<T2>> list;
            if(!eventDict.TryGetValue(key, out list))
            {
                list = new List<Action<T2>>();
                eventDict[key] = list;
            }
            else
            {
                if(list.Contains(callback))
                {
                    PKLoger.LogError("Duplicate subscribe same event");
                }
            }

            list.Add(callback);   
        }

        public void Unsubscribe(T1 key, Action<T2> callback)
        {
            List< Action<T2>> list;
            if(!eventDict.TryGetValue(key, out list))
            {
                PKLoger.LogError("Unsubscribe nonexistent eventType");
                return;
            }
            else
            {
                if(!list.Contains(callback))
                {
                    PKLoger.LogError("Unsubscribe nonexistent event");
                    return;
                }
            }

            list.Remove(callback);
        }

        public void Publish(T1 key, T2 eventParams)
        {
            List< Action<T2>> list;
            if(eventDict.TryGetValue(key, out list))
            {
                for(int i = 0; i < list.Count; i++)
                {
                    list[i].Invoke(eventParams);
                }
            }
        }

        public void Clear()
        {
            eventDict.Clear();
        }
    }

}
