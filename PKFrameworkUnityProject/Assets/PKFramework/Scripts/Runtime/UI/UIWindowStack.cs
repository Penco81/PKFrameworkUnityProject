using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace PKFramework.Runtime.UI
{
    public class UIWindowStack
    {
        private int baseOrder = 0;
        private Dictionary<ulong, UIWindow> windowDict;
        private List<UIWindow> windowList;
        private Transform parent;

        public Transform Parent => parent;

        public UIWindowStack(int order, Transform parent)
        {
            baseOrder = order;
            this.parent = parent;
            windowDict = new Dictionary<ulong, UIWindow>();
            windowList = new List<UIWindow>();
        }

        public bool CheckWindow<T>() where T : UIWindow
        {
            return CheckWindow(typeof(T));
        }
        
        private bool CheckWindow(Type t)
        {
            foreach (var w in windowList)
            {
                if (w.GetType() == t)
                {
                    return true;
                }
            }
            return false;
        }

        public T RefocusWindow<T>() where T : UIWindow
        {
            return RefocusWindow(typeof(T)) as T;
        }
        
        private UIWindow RefocusWindow(Type t)
        {
            if (windowList.Count <= 0)
            {
                PKLogger.LogError($"Do not find window. Type name: {t.Name}");
                return null;
            }

            int index = -1;
            for (int i = 0; i < windowList.Count; i++)
            {
                var w = windowList[i];
                if (w.GetType() == t)
                {
                    index = i;
                    break;
                }
            }

            if (index < 0)
            {
                PKLogger.LogError($"Do not find window. Type name: {t.Name}");
                return null;
            }

            var temp = windowList[index];
            windowList.RemoveAt(index);
            windowList.Add(temp);
            int interval = UIManager.Instance.WindowInterval;
            for (int i = index; i < windowList.Count; i++)
            {
                int newOrder = baseOrder + i * interval;
                windowList[i].OnChangeOrder(newOrder);
            }

            for (int i = index; i < windowList.Count - 1; i++)
            {
                if (temp.UIConfig.isFullScreen)
                {
                    windowList[i].OnHide();
                }
                else
                {
                    windowList[i].OnCover();
                }
            }
            
            temp.OnFocus();
            return temp;
        }

        public void CloseAllTypeWindow<T>() where T : UIWindow
        {
            if (windowList.Count <= 0)
                return;
            int index = -1;
            Type t = typeof(T);
            List<UIWindow> tempList = ListPool<UIWindow>.Get();
            for (int i = 0; i < windowList.Count; i++)
            {
                if (windowList[i].GetType() == t)
                {
                    tempList.Add(windowList[i]);
                    windowList.RemoveAt(i);
                }
            }
            
            int interval = UIManager.Instance.WindowInterval;
            for (int i = index; i < windowList.Count; i++)
            {
                int newOrder = baseOrder + i * interval;
                windowList[i].OnChangeOrder(newOrder);
            }

            var topW = windowList[windowList.Count - 1];
            for (int i = index; i < windowList.Count; i++)
            {
                if (topW.UIConfig.isFullScreen)
                {
                    windowList[i].OnHide();
                }
                else
                {
                    windowList[i].OnCover();
                }
            }

            foreach (var w in tempList)
            {
                UIWindowHold.Release(w.Go);
                w.OnDestroy();
            }
            tempList.Clear();
            ListPool<UIWindow>.Release(tempList);
        }

        public void CloseTopWindow()
        {
            int index = windowList.Count - 1;
            var temp = windowList[index];
            windowList.RemoveAt(index);
            windowDict.Remove(temp.UniqId);

            int newIndex = windowList.Count - 1;
            var topW = windowList[newIndex];

            if (temp.UIConfig.isFullScreen)
            {
                for (int i = newIndex - 1; i >= 0; i--)
                {
                    windowList[i].OnCover();
                    if (windowList[i].UIConfig.isFullScreen)
                    {
                        break;
                    }
                }
            }

            UIWindowHold.Release(temp.Go);
            temp.OnDestroy();
            
            topW.OnFocus();
        }

        public void CloseWindow(ulong uniqId)
        {
            if (windowList.Count <= 0)
                return;
            if (!windowDict.TryGetValue(uniqId, out var w))
            {
                PKLogger.LogError($"Try to close a null window. Uid: {uniqId}");
                return;
            }
            
            int index = -1;
            for (int i = 0; i < windowList.Count; i++)
            {
                if (windowList[i].UniqId == uniqId)
                {
                    index = i;
                    break;
                }
            }

            var temp = windowList[index];
            windowList.RemoveAt(index);
            windowDict.Remove(uniqId);
            
            int interval = UIManager.Instance.WindowInterval;
            for (int i = index; i < windowList.Count; i++)
            {
                int newOrder = baseOrder + i * interval;
                windowList[i].OnChangeOrder(newOrder);
            }

            var topW = windowList[windowList.Count - 1];
            for (int i = index; i < windowList.Count; i++)
            {
                if (topW.UIConfig.isFullScreen)
                {
                    windowList[i].OnHide();
                }
                else
                {
                    windowList[i].OnCover();
                }
            }
            
            topW.OnFocus();
            UIWindowHold.Release(temp.Go);
            temp.OnDestroy();
        }

        /// <summary>
        /// 可能用不到，因为能得到uniqId说明该UIWindow正被持有
        /// </summary>
        /// <param name="uniq"></param>
        /// <returns></returns>
        public UIWindow GetWindow(ulong uniq)
        {
            return null;
        }

        public T GetWindow<T>() where T : UIWindow
        {
            return GetWindow(typeof(T)) as T;
        }

        private UIWindow GetWindow(Type t)
        {
            if (windowList.Count <= 0)
            {
                PKLogger.LogError($"Try to access empty stack. Order: {baseOrder}");
                return null;
            }
            foreach (var w in windowList)
            {
                if (w.GetType() == t)
                {
                    return w;
                }
            }

            return null;
        }

        //TODO
        public T[] GetAllTypeWindow<T>() where T : UIWindow
        {
            return null;
        }

        public UIWindow GetTopWindow()
        {
            if (windowList.Count <= 0)
            {
                PKLogger.LogError($"Try to access empty stack. Order: {baseOrder}");
                return null;
            }

            return windowList[windowList.Count - 1];
        }

        public void AddWindow(UIWindow window)
        {
            for (int i = windowList.Count - 1; i >= 0; i--)
            {
                if (window.UIConfig.isFullScreen)
                {
                    windowList[i].OnHide();
                }
                else
                {
                    windowList[i].OnCover();
                }

                if (windowList[i].UIConfig.isFullScreen)
                {
                    break;
                }
            }
            
            windowDict.Add(window.UniqId, window);
            windowList.Add(window);
            int order = baseOrder + windowList.Count * UIManager.Instance.WindowInterval;
            window.OnChangeOrder(order);
            
            window.OnFocus();
        }

        public void Clear()
        {
            foreach (var w in windowList)
            {
                UIWindowHold.Release(w.Go);
                w.OnDestroy();
            }
            windowList.Clear();
            windowDict.Clear();
        }
    }
}