﻿using UnityEngine;

namespace Framework.Runtime
{
    /// <summary>
    /// 下载代理辅助器完成事件。
    /// </summary>
    public sealed class DownloadAgentHelperCompleteEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 初始化下载代理辅助器完成事件的新实例。
        /// </summary>
        public DownloadAgentHelperCompleteEventArgs()
        {
            Length = 0;
        }

        /// <summary>
        /// 获取下载的数据大小。
        /// </summary>
        public int Length
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建下载代理辅助器完成事件。
        /// </summary>
        /// <param name="length">下载的数据大小。</param>
        /// <returns>创建的下载代理辅助器完成事件。</returns>
        public static DownloadAgentHelperCompleteEventArgs Create(int length)
        {
            if (length < 0)
            {
                Debug.LogError("Length is invalid.");
            }

            DownloadAgentHelperCompleteEventArgs downloadAgentHelperCompleteEventArgs = ReferencePool.Acquire<DownloadAgentHelperCompleteEventArgs>();
            downloadAgentHelperCompleteEventArgs.Length = length;
            return downloadAgentHelperCompleteEventArgs;
        }

        /// <summary>
        /// 清理下载代理辅助器完成事件。
        /// </summary>
        public override void Clear()
        {
            Length = 0;
        }
    }
}