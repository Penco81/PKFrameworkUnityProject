using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PKFramework.Runtime;
using PKFramework.Runtime.Pool;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

namespace PKFramework.Runtime.Event
{
    public abstract class EventParamsBase : IRecyclable
    {
        public abstract void Init();

        public abstract void Clear();
    }

    public class EventCenter
    {
        private class EventNode : IRecyclable
        {
            public string key;
            public ulong index;
            public Action<EventParamsBase> callback;

            public void Clear()
            {
                key = null;
                index = 0;
                callback = null;
            }
        }

        private Dictionary<string, List<ulong>> key2HandlerDict = new Dictionary<string, List<ulong>>();
        private Dictionary<ulong, Action<EventParamsBase>> eventDict = new Dictionary<ulong, Action<EventParamsBase>>();

        private List<EventNode> needAddList = new List<EventNode>();
        private List<ulong> needRemoveList = new List<ulong>();
        
        private ulong index = 0;

        public void Update()
        {
            if (needAddList.Count > 0)
            {
                foreach (var node in needAddList)
                {
                    List<ulong> list;
                    if(!key2HandlerDict.TryGetValue(node.key, out list))
                    {
                        list = ListPool<ulong>.Get();
                        key2HandlerDict.Add(node.key, list);
                    }
                    
                    eventDict.Add(node.index, node.callback);
                    list.Add(node.index);
                    ReferencePool.Release(node);
                }
                needAddList.Clear();
            }

            if (needRemoveList.Count > 0)
            {
                foreach (var handler in needRemoveList)
                {
                    if(!eventDict.TryGetValue(handler, out var callback))
                    {
                        PKLogger.LogError($"Unsubscribe nonexistent handler: {handler}");
                        continue;
                    }

                    eventDict.Remove(handler);
                }
                needRemoveList.Clear();
            }
        }

        public ulong Subscribe(string key, Action<EventParamsBase> callback)
        {
            ulong handler = index;
            EventNode en = ReferencePool.Acquire<EventNode>();
            en.callback = callback;
            en.key = key;
            en.index = index;
            needAddList.Add(en);
            
            index++;
            return handler;
        }

        public void UnSubscribe(ulong handler)
        {
            needRemoveList.Add(handler);
        }

        public void Dispatch<T>(string key, T eventParams) where T : EventParamsBase
        {
            List<ulong> list;
            List<ulong> newList = ListPool<ulong>.Get();
            if(key2HandlerDict.TryGetValue(key, out list))
            {
                for(int i = 0; i < list.Count; i++)
                {
                    if (eventDict.TryGetValue(list[i], out var callback))
                    {
                        callback.Invoke(eventParams);
                        newList.Add(list[i]);
                    }
                }

                key2HandlerDict[key] = newList;
                ListPool<ulong>.Release(list);
            }
        }

        public void Clear()
        {
            eventDict.Clear();
            key2HandlerDict.Clear();
            needAddList.Clear();
            needRemoveList.Clear();
        }
    }

}
