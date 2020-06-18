using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// 资源源文件封装
    /// </summary>
    public class SourceAsset
    {
        private Texture m_CachedIcon;

        public SourceAsset(string guid, string path, string name, SourceFolder folder)
        {
            if (folder == null)
            {
                Debug.LogError("Source asset folder is invalid.");
            }

            Guid = guid;
            Path = path;
            Name = name;
            Folder = folder;
            m_CachedIcon = null;
        }
        /// <summary>
        /// 资源GUID
        /// </summary>
        public string Guid
        {
            get;
            private set;
        }

        /// <summary>
        /// 资源路径
        /// </summary>
        public string Path
        {
            get;
            private set;
        }

        /// <summary>
        /// 资源名字
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 资源文件夹
        /// </summary>
        public SourceFolder Folder
        {
            get;
            private set;
        }

        /// <summary>
        /// 相对根目录的绝对路径
        /// </summary>
        public string FromRootPath
        {
            get
            {
                return Folder.Folder == null ? Name : string.Format("{0}/{1}", Folder.FromRootPath, Name);
            }
        }

        /// <summary>
        /// 路径深度
        /// </summary>
        public int Depth
        {
            get
            {
                return Folder != null ? Folder.Depth + 1 : 0;
            }
        }

        /// <summary>
        /// 资源图标
        /// </summary>
        public Texture Icon
        {
            get
            {
                if (m_CachedIcon == null)
                {
                    m_CachedIcon = AssetDatabase.GetCachedIcon(Path);
                }

                return m_CachedIcon;
            }
        }
    }
}