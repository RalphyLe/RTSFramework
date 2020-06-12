using UnityEngine;
using UnityEditor;
using System.IO;

namespace Framework.Runtime
{
    public class DownloadAgent : ITaskAgent<DownloadTask>
    {
        private readonly IDownloadAgentHelper m_Helper;
        private DownloadTask m_Task;
        private FileStream m_FileStream;
        private int m_WaitFlushSize;
        private float m_WaitTime;
        private int m_StartLength;
        private int m_DownloadedLength;
        private int m_SavedLength;
        private bool m_Disposed;

        //public GameFrameworkAction<DownloadAgent> DownloadAgentStart;
        //public GameFrameworkAction<DownloadAgent, int> DownloadAgentUpdate;
        //public GameFrameworkAction<DownloadAgent, int> DownloadAgentSuccess;
        //public GameFrameworkAction<DownloadAgent, string> DownloadAgentFailure;

        /// <summary>
        /// 初始化下载代理的新实例。
        /// </summary>
        /// <param name="downloadAgentHelper">下载代理辅助器。</param>
        public DownloadAgent(IDownloadAgentHelper downloadAgentHelper)
        {
            if (downloadAgentHelper == null)
            {
                Debug.LogError("Download agent helper is invalid.");
                return;
            }

            m_Helper = downloadAgentHelper;
            m_Task = null;
            m_FileStream = null;
            m_WaitFlushSize = 0;
            m_WaitTime = 0f;
            m_StartLength = 0;
            m_DownloadedLength = 0;
            m_SavedLength = 0;
            m_Disposed = false;

            //DownloadAgentStart = null;
            //DownloadAgentUpdate = null;
            //DownloadAgentSuccess = null;
            //DownloadAgentFailure = null;
        }

        /// <summary>
        /// 获取下载任务。
        /// </summary>
        public DownloadTask Task
        {
            get
            {
                return m_Task;
            }
        }

        /// <summary>
        /// 获取已经等待时间。
        /// </summary>
        public float WaitTime
        {
            get
            {
                return m_WaitTime;
            }
        }

        /// <summary>
        /// 获取开始下载时已经存在的大小。
        /// </summary>
        public int StartLength
        {
            get
            {
                return m_StartLength;
            }
        }

        /// <summary>
        /// 获取本次已经下载的大小。
        /// </summary>
        public int DownloadedLength
        {
            get
            {
                return m_DownloadedLength;
            }
        }

        /// <summary>
        /// 获取当前的大小。
        /// </summary>
        public int CurrentLength
        {
            get
            {
                return m_StartLength + m_DownloadedLength;
            }
        }

        /// <summary>
        /// 获取已经存盘的大小。
        /// </summary>
        public int SavedLength
        {
            get
            {
                return m_SavedLength;
            }
        }

        /// <summary>
        /// 初始化下载代理。
        /// </summary>
        public void Initialize()
        {
            m_Helper.DownloadAgentHelperUpdateBytes += OnDownloadAgentHelperUpdateBytes;
            m_Helper.DownloadAgentHelperUpdateLength += OnDownloadAgentHelperUpdateLength;
            m_Helper.DownloadAgentHelperComplete += OnDownloadAgentHelperComplete;
            m_Helper.DownloadAgentHelperError += OnDownloadAgentHelperError;
        }

        public void Initialize()
        {
            throw new System.NotImplementedException();
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }

        public void Shutdown()
        {
            throw new System.NotImplementedException();
        }

        public void Start(DownloadTask task)
        {
            throw new System.NotImplementedException();
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            throw new System.NotImplementedException();
        }
    } 
}