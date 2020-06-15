using UnityEngine;
using System.Collections;
using Framework.Runtime;
#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
public class DownloadTestMain : MonoBehaviour
{
    private const int DefaultPriority = 0;
    private string url = "https://ss0.bdstatic.com/70cFuHSh_Q1YnxGkpoWK1HF6hhy/it/u=3384032967,474817897&fm=26&gp=0.jpg";
    private string path = "./Assets/qq.jpg";
    private const int OneMegaBytes = 1024 * 1024;
    private DownloadManager m_DownloadManager = null;
    private string m_DownloadAgentHelperTypeName = "Framework.Runtime.UnityWebRequestDownloadAgentHelper";
    private DownloadAgentHelperBase m_CustomDownloadAgentHelper = null;
    private int m_DownloadAgentHelperCount = 3;
    private float m_Timeout = 30f;
    private int m_FlushSize = OneMegaBytes;

    private void Awake()
    {
        m_DownloadManager = FrameworkEntry.Instance.GetManager<DownloadManager>();
        if (m_DownloadManager == null)
        {
            Debug.LogError("Download manager is invalid.");
            return;
        }

        m_DownloadManager.DownloadStart += OnDownloadStart;
        m_DownloadManager.DownloadUpdate += OnDownloadUpdate;
        m_DownloadManager.DownloadSuccess += OnDownloadSuccess;
        m_DownloadManager.DownloadFailure += OnDownloadFailure;
        m_DownloadManager.DownloadProgress += OnDownloadProgress;
        m_DownloadManager.FlushSize = m_FlushSize;
        m_DownloadManager.Timeout = m_Timeout;
    }

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < m_DownloadAgentHelperCount; i++)
        {
            AddDownloadAgentHelper(i);
        }
    }

    public void OnDestroy()
    {
        m_DownloadManager.DownloadStart -= OnDownloadStart;
        m_DownloadManager.DownloadUpdate -= OnDownloadUpdate;
        m_DownloadManager.DownloadSuccess -= OnDownloadSuccess;
        m_DownloadManager.DownloadFailure -= OnDownloadFailure;
        m_DownloadManager.DownloadProgress -= OnDownloadProgress;
    }

    /// <summary>
    /// 增加下载代理辅助器。
    /// </summary>
    /// <param name="index">下载代理辅助器索引。</param>
    private void AddDownloadAgentHelper(int index)
    {
        DownloadAgentHelperBase downloadAgentHelper = new UnityWebRequestDownloadAgentHelper();
        if (downloadAgentHelper == null)
        {
            Debug.LogError("Can not create download agent helper.");
            return;
        }
        m_DownloadManager.AddDownloadAgentHelper(downloadAgentHelper);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_DownloadManager.AddDownload(path, url, DefaultPriority);
        }
    }

    private void OnDownloadStart(object sender, DownloadStartEventArgs e)
    {
        Debug.Log("开始下载");
        //m_EventComponent.Fire(this, DownloadStartEventArgs.Create(e));
    }

    private void OnDownloadUpdate(object sender, DownloadUpdateEventArgs e)
    {
        Debug.Log("已下载长度：" + e.CurrentLength);
        //m_EventComponent.Fire(this, DownloadUpdateEventArgs.Create(e));
    }

    private void OnDownloadProgress(object sender, DownloadProgressEventArgs e)
    {
        Debug.Log("已下载进度：" + e.CurrentProgress);
        //m_EventComponent.Fire(this, DownloadUpdateEventArgs.Create(e));
    }

    private void OnDownloadSuccess(object sender, DownloadSuccessEventArgs e)
    {
        Debug.Log("下载完毕");
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
        //m_EventComponent.Fire(this, DownloadSuccessEventArgs.Create(e));
    }

    private void OnDownloadFailure(object sender, DownloadFailureEventArgs e)
    {
        Debug.Log("下载失败");
        //Log.Warning("Download failure, download serial id '{0}', download path '{1}', download uri '{2}', error message '{3}'.", e.SerialId.ToString(), e.DownloadPath, e.DownloadUri, e.ErrorMessage);
        //m_EventComponent.Fire(this, DownloadFailureEventArgs.Create(e));
    }
}
