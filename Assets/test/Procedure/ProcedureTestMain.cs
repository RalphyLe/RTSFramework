using Framework.Runtime;
using UnityEngine;

public class ProcedureTestMain : MonoBehaviour
{

    private void Start()
    {
        ProcedureManager procedureManager = FrameworkEntry.Instance.GetManager<ProcedureManager>();

        //添加入口流程
        Procedure_Start entranceProcedure = new Procedure_Start();
        procedureManager.AddProcedure(entranceProcedure);
        procedureManager.SetEntranceProcedure(entranceProcedure);

        //添加其他流程
        procedureManager.AddProcedure(new Procedure_Play());
        procedureManager.AddProcedure(new Procedure_Over());

        //创建流程状态机
        procedureManager.CreateProceduresFsm();
    }

}