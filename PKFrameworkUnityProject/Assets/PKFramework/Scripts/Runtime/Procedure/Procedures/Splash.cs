using System.Collections;
using PKFramework.Runtime.Fsm;
using PKFramework.Runtime.Task;
using UnityEngine;

namespace PKFramework.Runtime.Procedure.Procedures
{
    /// <summary>
    /// 闪屏和初始化
    /// </summary>
    public class Splash : ProcedureBase
    {
        public override void OnEnter(IFsm<ProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            var _task = TaskManager.Instance.CreateTask<InitAssetTask>();
            _task.Completed = OnCompleted;
        }

        public override void OnUpdate(IFsm<ProcedureManager> procedureOwner)
        {
            base.OnUpdate(procedureOwner);
        }

        public void OnCompleted(TaskBase task)
        {
            var procedureOwner = ProcedureManager.Instance.procedureFsm;
            ChangeState<CheckUpdate>(procedureOwner);
        }

    }
}