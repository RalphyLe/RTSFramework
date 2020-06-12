﻿using System;

namespace Framework.Runtime
{
    /**//// <summary>
        /// 全局事件基类（继承该类的事件类才能被事件池管理）
        /// </summary>
    public abstract class GlobalEventArgs : EventArgs, IReference
    {
        // <summary>
        /// 事件类型ID
        /// </summary>
        public abstract int Id { get; }

        public abstract void Clear();
    }
}