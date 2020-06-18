namespace Framework.Runtime
{
    /**//// <summary>
        /// 下载进度事件。
        /// </summary>
    public sealed class DownloadProgressEventArgs : GlobalEventArgs
    {
        /// <summary>
        /// 初始化下载进度事件的新实例。
        /// </summary>
        public DownloadProgressEventArgs()
        {
            SerialId = 0;
            DownloadPath = null;
            DownloadUri = null;
            CurrentProgress = 0;
            UserData = null;
        }

        /// <summary>
        /// 获取下载任务的序列编号。
        /// </summary>
        public int SerialId
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取下载后存放路径。
        /// </summary>
        public string DownloadPath
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取下载地址。
        /// </summary>
        public string DownloadUri
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取当前进度。
        /// </summary>
        public float CurrentProgress
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取用户自定义数据。
        /// </summary>
        public object UserData
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建下载更新事件。
        /// </summary>
        /// <param name="serialId">下载任务的序列编号。</param>
        /// <param name="downloadPath">下载后存放路径。</param>
        /// <param name="downloadUri">下载地址。</param>
        /// <param name="CurrentProgress">当前进度。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>创建的下载进度事件。</returns>
        public static DownloadProgressEventArgs Create(int serialId, string downloadPath, string downloadUri, float CurrentProgress, object userData)
        {
            DownloadProgressEventArgs downloadProgressEventArgs = ReferencePool.Acquire<DownloadProgressEventArgs>();
            downloadProgressEventArgs.SerialId = serialId;
            downloadProgressEventArgs.DownloadPath = downloadPath;
            downloadProgressEventArgs.DownloadUri = downloadUri;
            downloadProgressEventArgs.CurrentProgress = CurrentProgress;
            downloadProgressEventArgs.UserData = userData;
            return downloadProgressEventArgs;
        }

        /// <summary>
        /// 清理下载进度事件。
        /// </summary>
        public override void Clear()
        {
            SerialId = 0;
            DownloadPath = null;
            DownloadUri = null;
            CurrentProgress = 0;
            UserData = null;
        }
    }
}