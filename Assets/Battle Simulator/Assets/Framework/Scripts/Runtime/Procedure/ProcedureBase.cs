﻿using UnityEngine;

namespace Framework.Runtime
{
    /// <summary>
    /// 流程基类
    /// </summary>
    public class ProcedureBase : FsmState<ProcedureManager>
    {

        public override void OnEnter(Fsm<ProcedureManager> fsm)
        {
            base.OnEnter(fsm);
            Debug.Log("进入流程：" + GetType().FullName);
        }

        public override void OnLeave(Fsm<ProcedureManager> fsm, bool isShutdown)
        {
            base.OnLeave(fsm, isShutdown);
            Debug.Log("离开流程：" + GetType().FullName);
        }

    }

}