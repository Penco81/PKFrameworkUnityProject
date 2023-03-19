using System;
using System.Collections.Generic;

namespace PKFramework.Runtime.Singleton
{
    public class SchedulerComponent : MonoSingleton<SchedulerComponent>
    {
        private List<IScheduler> _schedulers = new List<IScheduler>();

        private Dictionary<string, IScheduler> _schedulersDict = new Dictionary<string, IScheduler>(); 
        private void Update()
        {
            foreach (var scheduler in _schedulers)
            {
                scheduler.Update();
            }
        }

        public void CreateScheduler(string name)
        {
            if (_schedulersDict.TryGetValue(name, out var scheduler))
            {
                PKLogger.LogError($"Duplicate Scheduler {name} ");
                return;
            }
            _schedulersDict.Add(name, new Scheduler(name));
        }

        public void RemoveScheduler()
        {
            if (!_schedulersDict.TryGetValue(name, out var scheduler))
            {
                PKLogger.LogError($"Scheduler {name} is null");
                return;
            }

            _schedulersDict.Remove(name);
            _schedulers.Remove(scheduler);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (var scheduler in _schedulers)
            {
                scheduler.Clear();
            }
            _schedulers.Clear();
        }
    }
}