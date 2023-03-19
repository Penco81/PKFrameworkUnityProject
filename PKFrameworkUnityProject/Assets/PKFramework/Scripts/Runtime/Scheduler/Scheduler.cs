
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PKFramework.Runtime
{
    public interface IScheduler
    {

        float TimeScale { get; set; }

        bool IsPause { get; set; }

        float Interval { get; set; }

        void Update();

        ulong Schedule(float duration, Action<Object> callback, Object param = null);

        bool UnSchedule(ulong handler);

        ulong Delay(float delay, Action<Object> callback, Object param = null);

        void Clear();
    }

    public class Scheduler : IScheduler
    {
        class TimeNode
        {
            public ulong index;
            public Action<Object> callback;
            public Object param;
            public bool isRepeat;
            public float delay;
            public float duration;
        }

        private ulong index = 0;

        private bool isPause = false;

        private float timeScale = 1;

        private float intervalTime = 0;
        
        private string name;

        //0表示1帧
        private float interval = 0;
        
        delegate void TimeHandler(Object param);

        private List<TimeNode> needAddList = new List<TimeNode>();

        private List<TimeNode> needRemoveList = new List<TimeNode>();

        private Dictionary<ulong, TimeNode> dict = new Dictionary<ulong, TimeNode>();

        public float TimeScale 
        { 
            get
            {
                return timeScale;
            }
            set
            {
                timeScale = value;
            }
        }

        public bool IsPause
        {
            get
            {
                return isPause;
            }
            set
            {
                isPause = value;
            }
        }

        public float Interval
        {
            get
            {
                return interval;
            }
            set
            {
                interval = value;
            }
        }

        public Scheduler(string name)
        {
            this.name = String.IsNullOrEmpty(name)? "default" : name;
        }

        /// <summary>
        /// 添加新结点，移除旧结点
        /// </summary>
        private void Refresh()
        {
            if (needAddList.Count > 0)
            {
                foreach (var newNode in needAddList)
                {
                    if (dict.TryGetValue(newNode.index, out var node))
                    {
                        PKLogger.LogError($"{name} Scheduler duplicate timeNode");
                    }
                    else
                    {
                        dict.Add(newNode.index, node);
                    }
                }
                
                needAddList.Clear();
            }

            if (needRemoveList.Count > 0)
            {
                foreach (var oldNode in needRemoveList)
                {
                    if (!dict.TryGetValue(oldNode.index, out var node))
                    {
                        PKLogger.LogError($"{name} Scheduler remove a null timeNode");
                    }
                    else
                    {
                        dict.Remove(oldNode.index);
                    }
                }
                needRemoveList.Clear();
            }
        }

        public void Update()
        {
            Refresh();
            float deltaTime = Time.deltaTime * timeScale;
            intervalTime -= deltaTime;
            foreach (var node in dict)
            {
                node.Value.delay -= deltaTime;
            }

            if (intervalTime <= 0)
            {
                intervalTime = interval;
                foreach (var node in dict)
                {
                    if(node.Value.delay <= 0)
                    {
                        node.Value.callback(node.Value.param);
                        if (node.Value.isRepeat)
                        {
                            node.Value.delay = node.Value.duration;
                        }
                        else
                        {
                            UnSchedule(node.Key);
                        }
                    }
                }
            }
        }

        public ulong Schedule(float duration, Action<Object> callback, Object param = null)
        {
            index++;
            needAddList.Add(new TimeNode{index = index, duration = duration, delay = 0, callback = callback, isRepeat = true, param = null});
            return index;
        }

        public bool UnSchedule(ulong handler)
        {
            if (!dict.TryGetValue(handler, out var node))
            {
                PKLogger.LogError($"{name} Scheduler remove a null timeNode");
                return false;
            }
            else
            {
                dict.Remove(handler);
                return true;
            }
        }

        public ulong Delay(float delay, Action<Object> callback, Object param = null)
        {
            index++;
            needAddList.Add(new TimeNode{index = index, duration = 0, delay = delay, callback = callback, isRepeat = true, param = null});
            return index;
        }

        public void Clear()
        {
            needAddList.Clear();
            needRemoveList.Clear();
            dict.Clear();
        }
    }
}