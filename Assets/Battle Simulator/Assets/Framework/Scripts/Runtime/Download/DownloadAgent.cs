﻿using UnityEngine;
using UnityEditor;
using System.IO;
using System;

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

        public GameFrameworkAction<DownloadAgent> DownloadAgentStart;
        public GameFrameworkAction<DownloadAgent, int> DownloadAgentUpdate;
        public GameFrameworkAction<DownloadAgent, int> DownloadAgentSuccess;
        public GameFrameworkAction<DownloadAgent, string> DownloadAgentFailure;

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

            DownloadAgentStart = null;
            DownloadAgentUpdate = null;
            DownloadAgentSuccess = null;
            DownloadAgentFailure = null;
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

        public void Reset()
        {
            throw new System.NotImplementedException();
        }

        public void Shutdown()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 开始处理下载任务。
        /// </summary>
        /// <param name="task">要处理的下载任务。</param>
        /// <returns>开始处理任务的状态。</returns>
        public void Start(DownloadTask task)
        {
            if (task == null)
            {
                Debug.LogError("Task is invalid.");
            }

            m_Task = task;

            m_Task.Status = DownloadTaskStatus.Doing;
            string downloadFile = string.Format("{0}.download", m_Task.DownloadPath);

            try
            {
                if (File.Exists(downloadFile))
                {
                    m_FileStream = File.OpenWrite(downloadFile);
                    m_FileStream.Seek(0, SeekOrigin.End);
                    m_StartLength = m_SavedLength = (int)m_FileStream.Length;
                    m_DownloadedLength = 0;
                }
                else
                {
                    string directory = Path.GetDirectoryName(m_Task.DownloadPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    m_FileStream = new FileStream(downloadFile, FileMode.Create, FileAccess.Write);
                    m_StartLength = m_SavedLength = m_DownloadedLength = 0;
                }

                if (DownloadAgentStart != null)
                {
                    DownloadAgentStart(this);
                }

                if (m_StartLength > 0)
                {
                    m_Helper.Download(m_Task.DownloadUri, m_StartLength, m_Task.UserData);
                }
                else
                {
                    m_Helper.Download(m_Task.DownloadUri, m_Task.UserData);
                }
                //return StartTaskStatus.CanResume;
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.Message);
                //DownloadAgentHelperErrorEventArgs downloadAgentHelperErrorEventArgs = DownloadAgentHelperErrorEventArgs.Create(false, exception.ToString());
                //OnDownloadAgentHelperError(this, downloadAgentHelperErrorEventArgs);
                //ReferencePool.Release(downloadAgentHelperErrorEventArgs);
                //return StartTaskStatus.UnknownError;
            }
        }

        /// <summary>
        /// 下载代理轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (m_Task.Status == DownloadTaskStatus.Doing)
            {
                m_WaitTime += realElapseSeconds;
                if (m_WaitTime >= m_Task.Timeout)
                {
                    //DownloadAgentHelperErrorEventArgs downloadAgentHelperErrorEventArgs = DownloadAgentHelperErrorEventArgs.Create(false, "Timeout");
                    //OnDownloadAgentHelperError(this, downloadAgentHelperErrorEventArgs);
                    //ReferencePool.Release(downloadAgentHelperErrorEventArgs);
                }
            }
        }

        private void OnDownloadAgentHelperUpdateBytes(object sender, DownloadAgentHelperUpdateBytesEventArgs e)
        {
            m_WaitTime = 0f;
            try
            {
                m_FileStream.Write(e.GetBytes(), e.Offset, e.Length);
                m_WaitFlushSize += e.Length;
                m_SavedLength += e.Length;

                if (m_WaitFlushSize >= m_Task.FlushSize)
                {
                    m_FileStream.Flush();
                    m_WaitFlushSize = 0;
                }
            }
            catch (Exception exception)
            {
                DownloadAgentHelperErrorEventArgs downloadAgentHelperErrorEventArgs = DownloadAgentHelperErrorEventArgs.Create(false, exception.ToString());
                OnDownloadAgentHelperError(this, downloadAgentHelperErrorEventArgs);
                ReferencePool.Release(downloadAgentHelperErrorEventArgs);
            }
        }

        private void OnDownloadAgentHelperUpdateLength(object sender, DownloadAgentHelperUpdateLengthEventArgs e)
        {
            m_WaitTime = 0f;
            m_DownloadedLength += e.DeltaLength;
            if (DownloadAgentUpdate != null)
            {
                DownloadAgentUpdate(this, e.DeltaLength);
            }
        }

        private void OnDownloadAgentHelperComplete(object sender, DownloadAgentHelperCompleteEventArgs e)
        {
            m_WaitTime = 0f;
            m_DownloadedLength = e.Length;
            if (m_SavedLength != CurrentLength)
            {
                Debug.LogError("Internal download error.");
            }

            m_Helper.Reset();
            m_FileStream.Close();
            m_FileStream = null;

            if (File.Exists(m_Task.DownloadPath))
            {
                File.Delete(m_Task.DownloadPath);
            }

            File.Move(string.Format("{0}.download", m_Task.DownloadPath), m_Task.DownloadPath);

            m_Task.Status = DownloadTaskStatus.Done;

            if (DownloadAgentSuccess != null)
            {
                DownloadAgentSuccess(this, e.Length);
            }

            m_Task.Done = true;
        }

        private void OnDownloadAgentHelperError(object sender, DownloadAgentHelperErrorEventArgs e)
        {
            m_Helper.Reset();
            if (m_FileStream != null)
            {
                m_FileStream.Close();
                m_FileStream = null;
            }

            if (e.DeleteDownloading)
            {
                File.Delete(string.Format("{0}.download", m_Task.DownloadPath));
            }

            m_Task.Status = DownloadTaskStatus.Error;

            if (DownloadAgentFailure != null)
            {
                DownloadAgentFailure(this, e.ErrorMessage);
            }

            m_Task.Done = true;
        }
    } 
}