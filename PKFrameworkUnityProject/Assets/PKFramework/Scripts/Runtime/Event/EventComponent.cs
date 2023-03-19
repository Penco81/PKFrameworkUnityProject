using System;
using System.Collections.Generic;
using PKFramework.Runtime.Singleton;

namespace PKFramework.Runtime.Event
{
    public class EventComponent : MonoSingleton<EventComponent>
    {
        private Dictionary<EventCenterType, EventCenter> allEventCenters =
            new Dictionary<EventCenterType, EventCenter>();

        public enum EventCenterType
        {
            UI,
            GameLogic,
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

        protected override void OnDestroy() 
        {
            base.OnDestroy();
            foreach (var pair in allEventCenters)
            {
                pair.Value.Clear();
            }
            allEventCenters.Clear();
        }
    }
}