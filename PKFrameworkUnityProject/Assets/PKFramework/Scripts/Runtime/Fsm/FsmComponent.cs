﻿
using System;
using System.Collections.Generic;
using PKFramework.Runtime.Singleton;

namespace PKFramework.Runtime.Fsm
{
    /// <summary>
    /// 有限状态机管理器。
    /// </summary>
    public class FsmComponent : MonoSingleton<FsmComponent>, IFsmComponent
    {
        private readonly Dictionary<TypeNamePair, FsmBase> fsms;
        private readonly List<FsmBase> tempFsms;

        /// <summary>
        /// 初始化有限状态机管理器的新实例。
        /// </summary>
        public FsmComponent()
        {
            fsms = new Dictionary<TypeNamePair, FsmBase>();
            tempFsms = new List<FsmBase>();
        }

        /// <summary>
        /// 获取有限状态机数量。
        /// </summary>
        public int Count
        {
            get
            {
                return fsms.Count;
            }
        }
        
        private void Update()
        {
            tempFsms.Clear();
            if (fsms.Count <= 0)
            {
                return;
            }

            foreach (KeyValuePair<TypeNamePair, FsmBase> fsm in fsms)
            {
                tempFsms.Add(fsm.Value);
            }

            foreach (FsmBase fsm in tempFsms)
            {
                if (fsm.IsDestroyed)
                {
                    continue;
                }

                fsm.Update();
            }
        }

        /// <summary>
        /// 检查是否存在有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <returns>是否存在有限状态机。</returns>
        public bool HasFsm<T>() where T : class
        {
            return InternalHasFsm(new TypeNamePair(typeof(T)));
        }

        /// <summary>
        /// 检查是否存在有限状态机。
        /// </summary>
        /// <param name="ownerType">有限状态机持有者类型。</param>
        /// <returns>是否存在有限状态机。</returns>
        public bool HasFsm(Type ownerType)
        {
            if (ownerType == null)
            {
                throw new Exception("Owner type is invalid.");
            }

            return InternalHasFsm(new TypeNamePair(ownerType));
        }

        /// <summary>
        /// 检查是否存在有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <param name="name">有限状态机名称。</param>
        /// <returns>是否存在有限状态机。</returns>
        public bool HasFsm<T>(string name) where T : class
        {
            return InternalHasFsm(new TypeNamePair(typeof(T), name));
        }

        /// <summary>
        /// 检查是否存在有限状态机。
        /// </summary>
        /// <param name="ownerType">有限状态机持有者类型。</param>
        /// <param name="name">有限状态机名称。</param>
        /// <returns>是否存在有限状态机。</returns>
        public bool HasFsm(Type ownerType, string name)
        {
            if (ownerType == null)
            {
                throw new Exception("Owner type is invalid.");
            }

            return InternalHasFsm(new TypeNamePair(ownerType, name));
        }

        /// <summary>
        /// 获取有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <returns>要获取的有限状态机。</returns>
        public IFsm<T> GetFsm<T>() where T : class
        {
            return (IFsm<T>)InternalGetFsm(new TypeNamePair(typeof(T)));
        }

        /// <summary>
        /// 获取有限状态机。
        /// </summary>
        /// <param name="ownerType">有限状态机持有者类型。</param>
        /// <returns>要获取的有限状态机。</returns>
        public FsmBase GetFsm(Type ownerType)
        {
            if (ownerType == null)
            {
                throw new Exception("Owner type is invalid.");
            }

            return InternalGetFsm(new TypeNamePair(ownerType));
        }

        /// <summary>
        /// 获取有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <param name="name">有限状态机名称。</param>
        /// <returns>要获取的有限状态机。</returns>
        public IFsm<T> GetFsm<T>(string name) where T : class
        {
            return (IFsm<T>)InternalGetFsm(new TypeNamePair(typeof(T), name));
        }

        /// <summary>
        /// 获取有限状态机。
        /// </summary>
        /// <param name="ownerType">有限状态机持有者类型。</param>
        /// <param name="name">有限状态机名称。</param>
        /// <returns>要获取的有限状态机。</returns>
        public FsmBase GetFsm(Type ownerType, string name)
        {
            if (ownerType == null)
            {
                throw new Exception("Owner type is invalid.");
            }

            return InternalGetFsm(new TypeNamePair(ownerType, name));
        }

        /// <summary>
        /// 获取所有有限状态机。
        /// </summary>
        /// <returns>所有有限状态机。</returns>
        public FsmBase[] GetAllFsms()
        {
            int index = 0;
            FsmBase[] results = new FsmBase[fsms.Count];
            foreach (KeyValuePair<TypeNamePair, FsmBase> fsm in fsms)
            {
                results[index++] = fsm.Value;
            }

            return results;
        }

        /// <summary>
        /// 获取所有有限状态机。
        /// </summary>
        /// <param name="results">所有有限状态机。</param>
        public void GetAllFsms(List<FsmBase> results)
        {
            if (results == null)
            {
                throw new Exception("Results is invalid.");
            }

            results.Clear();
            foreach (KeyValuePair<TypeNamePair, FsmBase> fsm in fsms)
            {
                results.Add(fsm.Value);
            }
        }

        /// <summary>
        /// 创建有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <param name="owner">有限状态机持有者。</param>
        /// <param name="states">有限状态机状态集合。</param>
        /// <returns>要创建的有限状态机。</returns>
        public IFsm<T> CreateFsm<T>(T owner, params FsmState<T>[] states) where T : class
        {
            return CreateFsm(string.Empty, owner, states);
        }

        /// <summary>
        /// 创建有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <param name="name">有限状态机名称。</param>
        /// <param name="owner">有限状态机持有者。</param>
        /// <param name="states">有限状态机状态集合。</param>
        /// <returns>要创建的有限状态机。</returns>
        public IFsm<T> CreateFsm<T>(string name, T owner, params FsmState<T>[] states) where T : class
        {
            TypeNamePair typeNamePair = new TypeNamePair(typeof(T), name);
            if (HasFsm<T>(name))
            {
                throw new Exception(Utility.Text.Format("Already exist FSM '{0}'.", typeNamePair.ToString()));
            }

            Fsm<T> fsm = Fsm<T>.Create(name, owner, states);
            fsms.Add(typeNamePair, fsm);
            return fsm;
        }

        /// <summary>
        /// 创建有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <param name="owner">有限状态机持有者。</param>
        /// <param name="states">有限状态机状态集合。</param>
        /// <returns>要创建的有限状态机。</returns>
        public IFsm<T> CreateFsm<T>(T owner, List<FsmState<T>> states) where T : class
        {
            return CreateFsm(string.Empty, owner, states);
        }

        /// <summary>
        /// 创建有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <param name="name">有限状态机名称。</param>
        /// <param name="owner">有限状态机持有者。</param>
        /// <param name="states">有限状态机状态集合。</param>
        /// <returns>要创建的有限状态机。</returns>
        public IFsm<T> CreateFsm<T>(string name, T owner, List<FsmState<T>> states) where T : class
        {
            TypeNamePair typeNamePair = new TypeNamePair(typeof(T), name);
            if (HasFsm<T>(name))
            {
                throw new Exception(Utility.Text.Format("Already exist FSM '{0}'.", typeNamePair));
            }

            Fsm<T> fsm = Fsm<T>.Create(name, owner, states);
            fsms.Add(typeNamePair, fsm);
            return fsm;
        }

        /// <summary>
        /// 销毁有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <returns>是否销毁有限状态机成功。</returns>
        public bool DestroyFsm<T>() where T : class
        {
            return InternalDestroyFsm(new TypeNamePair(typeof(T)));
        }

        /// <summary>
        /// 销毁有限状态机。
        /// </summary>
        /// <param name="ownerType">有限状态机持有者类型。</param>
        /// <returns>是否销毁有限状态机成功。</returns>
        public bool DestroyFsm(Type ownerType)
        {
            if (ownerType == null)
            {
                throw new Exception("Owner type is invalid.");
            }

            return InternalDestroyFsm(new TypeNamePair(ownerType));
        }

        /// <summary>
        /// 销毁有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <param name="name">要销毁的有限状态机名称。</param>
        /// <returns>是否销毁有限状态机成功。</returns>
        public bool DestroyFsm<T>(string name) where T : class
        {
            return InternalDestroyFsm(new TypeNamePair(typeof(T), name));
        }

        /// <summary>
        /// 销毁有限状态机。
        /// </summary>
        /// <param name="ownerType">有限状态机持有者类型。</param>
        /// <param name="name">要销毁的有限状态机名称。</param>
        /// <returns>是否销毁有限状态机成功。</returns>
        public bool DestroyFsm(Type ownerType, string name)
        {
            if (ownerType == null)
            {
                throw new Exception("Owner type is invalid.");
            }

            return InternalDestroyFsm(new TypeNamePair(ownerType, name));
        }

        /// <summary>
        /// 销毁有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <param name="fsm">要销毁的有限状态机。</param>
        /// <returns>是否销毁有限状态机成功。</returns>
        public bool DestroyFsm<T>(IFsm<T> fsm) where T : class
        {
            if (fsm == null)
            {
                throw new Exception("FSM is invalid.");
            }

            return InternalDestroyFsm(new TypeNamePair(typeof(T), fsm.Name));
        }

        /// <summary>
        /// 销毁有限状态机。
        /// </summary>
        /// <param name="fsm">要销毁的有限状态机。</param>
        /// <returns>是否销毁有限状态机成功。</returns>
        public bool DestroyFsm(FsmBase fsm)
        {
            if (fsm == null)
            {
                throw new Exception("FSM is invalid.");
            }

            return InternalDestroyFsm(new TypeNamePair(fsm.OwnerType, fsm.Name));
        }

        private bool InternalHasFsm(TypeNamePair typeNamePair)
        {
            return fsms.ContainsKey(typeNamePair);
        }

        private FsmBase InternalGetFsm(TypeNamePair typeNamePair)
        {
            FsmBase fsm = null;
            if (fsms.TryGetValue(typeNamePair, out fsm))
            {
                return fsm;
            }

            return null;
        }

        private bool InternalDestroyFsm(TypeNamePair typeNamePair)
        {
            FsmBase fsm = null;
            if (fsms.TryGetValue(typeNamePair, out fsm))
            {
                fsm.Shutdown();
                return fsms.Remove(typeNamePair);
            }

            return false;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (KeyValuePair<TypeNamePair, FsmBase> fsm in fsms)
            {
                fsm.Value.Shutdown();
            }

            fsms.Clear();
            tempFsms.Clear();
        }
    }
}