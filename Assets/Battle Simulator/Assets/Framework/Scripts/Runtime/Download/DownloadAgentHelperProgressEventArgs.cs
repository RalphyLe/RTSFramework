using UnityEngine;
using UnityEditor;

namespace Framework.Runtime
{
    /// <summary>
    /// 下载代理辅助器下载进度事件。
    /// </summary>
    public sealed class DownloadAgentHelperProgressEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 初始化下载代理辅助器下载进度事件的新实例。
        /// </summary>
        public DownloadAgentHelperProgressEventArgs()
        {
            DownloadProgess = 0;
        }

        /// <summary>
        /// 获取当前下载进度
        /// </summary>
        public float DownloadProgess
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DownloadAgentHelperProgressEventArgs Create(float downloadProgess)
        {
            if (downloadProgess <= 0)
            {
                Debug.LogError("downloadProgess is invalid.");
            }

            DownloadAgentHelperProgressEventArgs downloadAgentHelperDownloadProgressChangedEventArgs = ReferencePool.Acquire<DownloadAgentHelperProgressEventArgs>();
            downloadAgentHelperDownloadProgressChangedEventArgs.DownloadProgess = downloadProgess;
            return downloadAgentHelperDownloadProgressChangedEventArgs;
        }

        /// <summary>
        /// 清理下载代理辅助器错误事件。
        /// </summary>
        /// <summary>
        /// 清理下载代理辅助器更新数据大小事件。
        /// </summary>
        public override void Clear()
        {
            DownloadProgess = 0;
        }
    }
}