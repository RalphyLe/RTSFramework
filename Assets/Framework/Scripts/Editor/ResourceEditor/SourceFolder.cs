using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    /// <summary>
    /// 资源编辑器中对unity工程源文件夹的管理类
    /// </summary>
    public class SourceFolder
    {
        private static Texture s_CachedIcon = null;

        /// <summary>
        /// 子文件夹
        /// </summary>
        private readonly List<SourceFolder> m_Folders;
        /// <summary>
        /// 当前路径下的unity工程源文件
        /// </summary>
        private readonly List<SourceAsset> m_Assets;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">文件夹名字</param>
        /// <param name="folder">父文件夹</param>
        public SourceFolder(string name, SourceFolder folder)
        {
            m_Folders = new List<SourceFolder>();
            m_Assets = new List<SourceAsset>();

            Name = name;
            Folder = folder;
        }

        /// <summary>
        /// 文件夹名
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 父文件夹
        /// </summary>
        public SourceFolder Folder
        {
            get;
            private set;
        }

        /// <summary>
        /// 从根目录开始的相对路径
        /// </summary>
        public string FromRootPath
        {
            get
            {
                return Folder == null ? string.Empty : (Folder.Folder == null ? Name : string.Format("{0}/{1}", Folder.FromRootPath, Name));
            }
        }

        /// <summary>
        /// 文件夹深度
        /// </summary>
        public int Depth
        {
            get
            {
                return Folder != null ? Folder.Depth + 1 : 0;
            }
        }

        /// <summary>
        /// 文件夹图标
        /// </summary>
        public static Texture Icon
        {
            get
            {
                if (s_CachedIcon == null)
                {
                    s_CachedIcon = AssetDatabase.GetCachedIcon("Assets");
                }

                return s_CachedIcon;
            }
        }

        /// <summary>
        /// 清除文件夹数据
        /// </summary>
        public void Clear()
        {
            m_Folders.Clear();
            m_Assets.Clear();
        }

        /// <summary>
        /// 获取所有子文件夹
        /// </summary>
        /// <returns></returns>
        public SourceFolder[] GetFolders()
        {
            return m_Folders.ToArray();
        }

        /// <summary>
        /// 查找文件夹
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SourceFolder GetFolder(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Source folder name is invalid.");
            }

            foreach (SourceFolder folder in m_Folders)
            {
                if (folder.Name == name)
                {
                    return folder;
                }
            }

            return null;
        }

        /// <summary>
        /// 添加子文件夹
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SourceFolder AddFolder(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Source folder name is invalid.");
            }

            SourceFolder folder = GetFolder(name);
            if (folder != null)
            {
                Debug.LogError("Source folder is already exist.");
            }

            folder = new SourceFolder(name, this);
            m_Folders.Add(folder);

            return folder;
        }

        public SourceAsset[] GetAssets()
        {
            return m_Assets.ToArray();
        }

        /// <summary>
        /// 查找资源
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SourceAsset GetAsset(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Source asset name is invalid.");
            }

            foreach (SourceAsset asset in m_Assets)
            {
                if (asset.Name == name)
                {
                    return asset;
                }
            }

            return null;
        }
        /// <summary>
        /// 添加资源
        /// </summary>
        /// <param name="guid">资源guid</param>
        /// <param name="path">资源路径，从根目录开始的相对路径</param>
        /// <param name="name">资源名字</param>
        /// <returns></returns>
        public SourceAsset AddAsset(string guid, string path, string name)
        {
            if (string.IsNullOrEmpty(guid))
            {
                Debug.LogError("Source asset guid is invalid.");
            }

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("Source asset path is invalid.");
            }

            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Source asset name is invalid.");
            }

            SourceAsset asset = GetAsset(name);
            if (asset != null)
            {
                Debug.LogError(string.Format("Source asset '{0}' is already exist.", name));
            }

            asset = new SourceAsset(guid, path, name, this);
            m_Assets.Add(asset);

            return asset;
        }
    }
}