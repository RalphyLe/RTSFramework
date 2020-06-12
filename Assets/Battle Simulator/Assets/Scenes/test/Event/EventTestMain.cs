using Framework.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTestMain : MonoBehaviour
{


    private void Start()
    {
        //订阅事件
        FrameworkEntry.Instance.GetManager<EventManager>().Subscribe(1, EventTestMethod);
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            EventTestArgs e = ReferencePool.Acquire<EventTestArgs>();

            //派发事件
            FrameworkEntry.Instance.GetManager<EventManager>().Fire(this, e.Fill("EventArgs"));
        }
    }


    /// <summary>
        /// 事件处理方法
        /// </summary>
    private void EventTestMethod(object sender, GlobalEventArgs e)
    {
        EventTestArgs args = e as EventTestArgs;
        Debug.Log(args.m_Name);
    }
}

