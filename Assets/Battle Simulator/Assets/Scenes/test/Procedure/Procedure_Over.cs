using Framework.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Procedure_Over : ProcedureBase
{
    // Start is called before the first frame update
    public override void OnUpdate(Fsm<ProcedureManager> fsm, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);
        if (Input.GetMouseButtonDown(0))
        {
            ChangeState<Procedure_Start>(fsm);
        }
    }
}
