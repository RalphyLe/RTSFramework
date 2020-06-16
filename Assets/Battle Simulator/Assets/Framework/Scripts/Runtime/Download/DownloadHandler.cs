using System;
using System.Net;
using UnityEngine;
#if UNITY_5_4_OR_NEWER
using UnityEngine.Networking;
#else
using UnityEngine.Experimental.Networking;
#endif

namespace Framework.Runtime
{
    public partial class UnityWebRequestDownloadAgentHelper : DownloadAgentHelperBase, IDisposable
    {
        private sealed class DownloadHandler : DownloadHandlerScript
        {
            private readonly UnityWebRequestDownloadAgentHelper m_Owner;
            private ulong contentLength = 0;
            public DownloadHandler(UnityWebRequestDownloadAgentHelper owner)
                : base(owner.m_CachedBytes)
            {
                m_Owner = owner;
            }

            protected override void ReceiveContentLengthHeader(ulong contentLength)
            {
                base.ReceiveContentLengthHeader(contentLength);
                this.contentLength = contentLength;
            }

            protected override bool ReceiveData(byte[] data, int dataLength)
            {
                if (m_Owner != null && m_Owner.m_UnityWebRequest != null && dataLength > 0)
                {
                    DownloadAgentHelperUpdateBytesEventArgs downloadAgentHelperUpdateBytesEventArgs = DownloadAgentHelperUpdateBytesEventArgs.Create(data, 0, dataLength);
                    m_Owner.m_DownloadAgentHelperUpdateBytesEventHandler(this, downloadAgentHelperUpdateBytesEventArgs);
                    ReferencePool.Release(downloadAgentHelperUpdateBytesEventArgs);

                    DownloadAgentHelperUpdateLengthEventArgs downloadAgentHelperUpdateLengthEventArgs = DownloadAgentHelperUpdateLengthEventArgs.Create(dataLength);
                    m_Owner.m_DownloadAgentHelperUpdateLengthEventHandler(this, downloadAgentHelperUpdateLengthEventArgs);
                    ReferencePool.Release(downloadAgentHelperUpdateLengthEventArgs);

                    DownloadAgentHelperProgressEventArgs downloadAgentHelperProgressEventArgs = DownloadAgentHelperProgressEventArgs.Create(contentLength);
                    m_Owner.m_DownloadAgentHelperProgressEventHandler(this, downloadAgentHelperProgressEventArgs);
                    ReferencePool.Release(downloadAgentHelperProgressEventArgs);
                }

                return base.ReceiveData(data, dataLength);
            }

            //protected override float GetProgress()
            //{
            //    return this.contentLength==0 ? 0 : m_Owner.
            //}
        }
    }
}
