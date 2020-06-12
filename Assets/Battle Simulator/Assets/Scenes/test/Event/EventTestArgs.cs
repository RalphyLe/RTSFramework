using Framework.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTestArgs : GlobalEventArgs
{

    public string m_Name;


    public override int Id
    {
        get
        {
            return 1;
        }
    }

    public override void Clear()
    {
        m_Name = string.Empty;
    }


    /// <summary>
        /// 事件填充
        /// </summary>
    public EventTestArgs Fill(string name)
    {
        m_Name = name;

        return this;
    }
}
