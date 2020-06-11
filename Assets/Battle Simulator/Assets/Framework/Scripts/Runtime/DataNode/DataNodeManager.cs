namespace Framework.Runtime
{
    /// <summary>
    /// 数据结点管理器
    /// </summary>
    public class DataNodeManager : ManagerBase
    {

        private static readonly string[] s_EmptyStringArray = new string[] { };

        /// <summary>
        /// 根结点
        /// </summary>
        public DataNode Root { get; private set; }

        /// <summary>
        /// 根结点名称
        /// </summary>
        private const string RootName = "<Root>";

        public DataNodeManager()
        {
            Root = new DataNode(RootName, null);
        }

        public override void Init()
        {

        }

        public override void Shutdown()
        {
            Root.Clear();
            Root = null;
        }

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {

        }

    }
}