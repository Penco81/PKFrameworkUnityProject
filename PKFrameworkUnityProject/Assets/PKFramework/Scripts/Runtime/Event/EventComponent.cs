using System;
using System.Collections.Generic;
using PKFramework.Core;
using PKFramework.Runtime.Singleton;

namespace PKFramework.Runtime.Event
{
    public class EventManager : Singleton<EventManager>, IManager
    {
        private Dictionary<EventCenterType, EventCenter> allEventCenters =
            new Dictionary<EventCenterType, EventCenter>();

        public enum EventCenterType
        {
            UI,
            GameLogic,
            Data
        }

        public ulong Subscribe<T>(EventCenterType ect, string key, Action<EventParamsBase> callback)
        {
            EventCenter ec;
            if (!allEventCenters.TryGetValue(ect, out ec))
            {
                ec = new EventCenter();
                allEventCenters.Add(ect, ec);
            }

            return ec.Subscribe(key, callback);
        }
        
        public void UnSubscribe(EventCenterType ect, ulong handler)
        {
            EventCenter ec;
            if (!allEventCenters.TryGetValue(ect, out ec))
            {
                PKLogger.LogError($"Try to access a null EventCenter {ect.ToString()}");
                return;
            }
            ec.UnSubscribe(handler);
        }

        public void Dispatch<T>(EventCenterType ect, string key, T eventParams) where T : EventParamsBase
        {
            EventCenter ec;
            if (!allEventCenters.TryGetValue(ect, out ec))
            {
                PKLogger.LogError($"Try to access a null EventCenter {ect.ToString()}");
                return;
            }
            ec.Dispatch(key, eventParams);
        }

        private void Update()
        {
            foreach (var pair in allEventCenters)
            {
                pair.Value.Update();
            }
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            
        }

        public void Shutdown()
        {
            
        }
    }
}