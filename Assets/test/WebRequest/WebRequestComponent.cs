using UnityEngine;
using UnityEditor;
using Framework.Runtime;
using Framework;

public class WebRequestComponent : MonoBehaviour
{
    private const int DefaultPriority = 0;

    private WebRequestManager m_WebRequestManager = null;
    private string m_WebRequestAgentHelperTypeName = "Framework.Runtime.UnityWebRequestAgentHelper";
    private WebRequestAgentHelperBase m_CustomWebRequestAgentHelper = null;
    private int m_WebRequestAgentHelperCount = 1;
    private float m_Timeout = 30f;

    /// <summary>
    /// 获取或设置 Web 请求超时时长，以秒为单位。
    /// </summary>
    public float Timeout
    {
        get
        {
            return m_WebRequestManager.Timeout;
        }
        set
        {
            m_WebRequestManager.Timeout = m_Timeout = value;
        }
    }

    private  void Awake()
    {

        m_WebRequestManager = FrameworkEntry.Instance.GetManager<WebRequestManager>();
        if (m_WebRequestManager == null)
        {
            Debug.LogError("Web request manager is invalid.");
            return;
        }

        m_WebRequestManager.Timeout = m_Timeout;
        m_WebRequestManager.WebRequestStart += OnWebRequestStart;
        m_WebRequestManager.WebRequestSuccess += OnWebRequestSuccess;
        m_WebRequestManager.WebRequestFailure += OnWebRequestFailure;
    }

    private void Start()
    {
        for (int i = 0; i < m_WebRequestAgentHelperCount; i++)
        {
            AddWebRequestAgentHelper(i);
        }
    }

    /// <summary>
    /// 增加 Web 请求代理辅助器。
    /// </summary>
    /// <param name="index">Web 请求代理辅助器索引。</param>
    private void AddWebRequestAgentHelper(int index)
    {
        System.Reflection.Assembly assembly = System.Reflection.Assembly.Load("Assembly-CSharp");

        WebRequestAgentHelperBase webRequestAgentHelper = assembly.CreateInstance(m_WebRequestAgentHelperTypeName) as WebRequestAgentHelperBase;
        if (webRequestAgentHelper == null)
        {
            Debug.LogError("Can not create web request agent helper.");
            return;
        }

        //webRequestAgentHelper.name = string.Format("Web Request Agent Helper - {0}", index.ToString());

        m_WebRequestManager.AddWebRequestAgentHelper(webRequestAgentHelper);
    }

    /// <summary>
    /// 增加 Web 请求任务。
    /// </summary>
    /// <param name="webRequestUri">Web 请求地址。</param>
    /// <param name="wwwForm">WWW 表单。</param>
    /// <param name="userData">用户自定义数据。</param>
    /// <returns>新增 Web 请求任务的序列编号。</returns>
    public int AddWebRequest(string webRequestUri, WWWForm wwwForm, object userData)
    {
        return AddWebRequest(webRequestUri, null, wwwForm, DefaultPriority, userData);
    }

    /// <summary>
    /// 增加 Web 请求任务。
    /// </summary>
    /// <param name="webRequestUri">Web 请求地址。</param>
    /// <param name="postData">要发送的数据流。</param>
    /// <param name="wwwForm">WWW 表单。</param>
    /// <param name="priority">Web 请求任务的优先级。</param>
    /// <param name="userData">用户自定义数据。</param>
    /// <returns>新增 Web 请求任务的序列编号。</returns>
    private int AddWebRequest(string webRequestUri, byte[] postData, WWWForm wwwForm, int priority, object userData)
    {
        return m_WebRequestManager.AddWebRequest(webRequestUri, postData, priority, WWWFormInfo.Create(wwwForm, userData));
    }

    private void OnWebRequestStart(object sender, WebRequestStartEventArgs e)
    {
        //m_EventComponent.Fire(this, WebRequestStartEventArgs.Create(e));
    }

    private void OnWebRequestSuccess(object sender, WebRequestSuccessEventArgs e)
    {
        //m_EventComponent.Fire(this, WebRequestSuccessEventArgs.Create(e));
    }

    private void OnWebRequestFailure(object sender, WebRequestFailureEventArgs e)
    {
        Debug.LogWarning(string.Format("Web request failure, web request serial id '{0}', web request uri '{1}', error message '{2}'.", e.SerialId.ToString(), e.WebRequestUri, e.ErrorMessage));
        //m_EventComponent.Fire(this, WebRequestFailureEventArgs.Create(e));
    }
}