using UnityEngine;
using UnityEditor;
using System;

namespace Framework.Runtime
{
    public enum WebRequestTaskStatus : byte
    {
        /// <summary>
        /// 准备请求。
        /// </summary>
        Todo = 0,

        /// <summary>
        /// 请求中。
        /// </summary>
        Doing,

        /// <summary>
        /// 请求完成。
        /// </summary>
        Done,

        /// <summary>
        /// 请求错误。
        /// </summary>
        Error
    } 
}