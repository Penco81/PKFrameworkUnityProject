
using System;
using PKFramework.Runtime.Fsm;
using PKFramework.Runtime.Singleton;

namespace PKFramework.Runtime.Procedure
{
    /// <summary>
    /// 流程管理器。
    /// </summary>
    internal sealed class ProcedureManager : MonoSingleton<ProcedureManager>, IProcedureManager
    {
        private IFsmComponent fsmComponent;
        private IFsm<IProcedureManager> procedureFsm;

        /// <summary>
        /// 初始化流程管理器的新实例。
        /// </summary>
        public ProcedureManager()
        {
            fsmComponent = null;
            procedureFsm = null;
        }

        /// <summary>
        /// 获取当前流程。
        /// </summary>
        public ProcedureBase CurrentProcedure
        {
            get
            {
                if (procedureFsm == null)
                {
                    throw new Exception("You must initialize procedure first.");
                }

                return (ProcedureBase)procedureFsm.CurrentState;
            }
        }

        void Update()
        {
        }

        /// <summary>
        /// 关闭并清理流程管理器。
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (fsmComponent != null)
            {
                if (procedureFsm != null)
                {
                    fsmComponent.DestroyFsm(procedureFsm);
                    procedureFsm = null;
                }

                fsmComponent = null;
            }
        }

        /// <summary>
        /// 初始化流程管理器。
        /// </summary>
        /// <param name="fsmComponent">有限状态机管理器。</param>
        /// <param name="procedures">流程管理器包含的流程。</param>
        public void Initialize(IFsmComponent fsmComponent, params ProcedureBase[] procedures)
        {
            if (fsmComponent == null)
            {
                throw new Exception("FSM manager is invalid.");
            }

            this.fsmComponent = fsmComponent;
            procedureFsm = this.fsmComponent.CreateFsm(this, procedures);
        }

        /// <summary>
        /// 开始流程。
        /// </summary>
        /// <typeparam name="T">要开始的流程类型。</typeparam>
        public void StartProcedure<T>() where T : ProcedureBase
        {
            if (procedureFsm == null)
            {
                throw new Exception("You must initialize procedure first.");
            }

            procedureFsm.Start<T>();
        }

        /// <summary>
        /// 开始流程。
        /// </summary>
        /// <param name="procedureType">要开始的流程类型。</param>
        public void StartProcedure(Type procedureType)
        {
            if (procedureFsm == null)
            {
                throw new Exception("You must initialize procedure first.");
            }

            procedureFsm.Start(procedureType);
        }

        /// <summary>
        /// 是否存在流程。
        /// </summary>
        /// <typeparam name="T">要检查的流程类型。</typeparam>
        /// <returns>是否存在流程。</returns>
        public bool HasProcedure<T>() where T : ProcedureBase
        {
            if (procedureFsm == null)
            {
                throw new Exception("You must initialize procedure first.");
            }

            return procedureFsm.HasState<T>();
        }

        /// <summary>
        /// 是否存在流程。
        /// </summary>
        /// <param name="procedureType">要检查的流程类型。</param>
        /// <returns>是否存在流程。</returns>
        public bool HasProcedure(Type procedureType)
        {
            if (procedureFsm == null)
            {
                throw new Exception("You must initialize procedure first.");
            }

            return procedureFsm.HasState(procedureType);
        }

        /// <summary>
        /// 获取流程。
        /// </summary>
        /// <typeparam name="T">要获取的流程类型。</typeparam>
        /// <returns>要获取的流程。</returns>
        public ProcedureBase GetProcedure<T>() where T : ProcedureBase
        {
            if (procedureFsm == null)
            {
                throw new Exception("You must initialize procedure first.");
            }

            return procedureFsm.GetState<T>();
        }

        /// <summary>
        /// 获取流程。
        /// </summary>
        /// <param name="procedureType">要获取的流程类型。</param>
        /// <returns>要获取的流程。</returns>
        public ProcedureBase GetProcedure(Type procedureType)
        {
            if (procedureFsm == null)
            {
                throw new Exception("You must initialize procedure first.");
            }

            return (ProcedureBase)procedureFsm.GetState(procedureType);
        }
    }
}
