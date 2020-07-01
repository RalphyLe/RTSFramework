using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public partial class ResourceEditor : EditorWindow
    {
        private class ResourceFolder
        {
            private static Texture s_CachedIcon = null;

            /// <summary>
            /// 子文件夹
            /// </summary>
            private readonly List<ResourceFolder> m_Folders;
            private readonly List<ResourceItem> m_Items;

            /// <summary>
            /// 资源包文件夹构造函数
            /// </summary>
            /// <param name="name"></param>
            /// <param name="folder"></param>
            public ResourceFolder(string name, ResourceFolder folder)
            {
                m_Folders = new List<ResourceFolder>();
                m_Items = new List<ResourceItem>();

                Name = name;
                Folder = folder;
            }

            /// <summary>
            /// 名字
            /// </summary>
            public string Name
            {
                get;
                private set;
            }

            /// <summary>
            /// 父文件夹
            /// </summary>
            public ResourceFolder Folder
            {
                get;
                private set;
            }

            /// <summary>
            /// 获取从根目录开始的相对目录
            /// </summary>
            public string FromRootPath
            {
                get
                {
                    return Folder == null ? string.Empty : (Folder.Folder == null ? Name : string.Format("{0}/{1}", Folder.FromRootPath, Name));
                }
            }

            /// <summary>
            /// 相对根目录的深度
            /// </summary>
            public int Depth
            {
                get
                {
                    return Folder != null ? Folder.Depth + 1 : 0;
                }
            }

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

            public void Clear()
            {
                m_Folders.Clear();
                m_Items.Clear();
            }

            /// <summary>
            /// 获取所有的子目录
            /// </summary>
            /// <returns></returns>
            public ResourceFolder[] GetFolders()
            {
                return m_Folders.ToArray();
            }

            /// <summary>
            /// 获取子目录
            /// </summary>
            /// <param name="name">子目录名/param>
            /// <returns></returns>
            public ResourceFolder GetFolder(string name)
            {
                if (string.IsNullOrEmpty(name))
                {
                    Debug.LogError("Resource folder name is invalid.");
                }

                foreach (ResourceFolder folder in m_Folders)
                {
                    if (folder.Name == name)
                    {
                        return folder;
                    }
                }

                return null;
            }

            /// <summary>
            /// 添加子目录
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public ResourceFolder AddFolder(string name)
            {
                if (string.IsNullOrEmpty(name))
                {
                    Debug.LogError("Resource folder name is invalid.");
                }

                ResourceFolder folder = GetFolder(name);
                if (folder != null)
                {
                    Debug.LogError("Resource folder is already exist.");
                }

                folder = new ResourceFolder(name, this);
                m_Folders.Add(folder);

                return folder;
            }

            /// <summary>
            /// 获取目录下的所有资源包
            /// </summary>
            /// <returns></returns>
            public ResourceItem[] GetItems()
            {
                return m_Items.ToArray();
            }

            /// <summary>
            /// 后去资源包
            /// </summary>
            /// <param name="name">资源包名</param>
            /// <returns></returns>
            public ResourceItem GetItem(string name)
            {
                if (string.IsNullOrEmpty(name))
                {
                    Debug.LogError("Resource item name is invalid.");
                }

                foreach (ResourceItem item in m_Items)
                {
                    if (item.Name == name)
                    {
                        return item;
                    }
                }

                return null;
            }

            /// <summary>
            /// 添加资源包
            /// </summary>
            /// <param name="name"></param>
            /// <param name="resource"></param>
            public void AddItem(string name, Resource resource)
            {
                ResourceItem item = GetItem(name);
                if (item != null)
                {
                    Debug.LogError("Resource item is already exist.");
                }

                item = new ResourceItem(name, resource, this);
                m_Items.Add(item);
                m_Items.Sort(ResourceItemComparer);
            }

            /// <summary>
            /// 资源包比较函数
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            private int ResourceItemComparer(ResourceItem a, ResourceItem b)
            {
                return a.Name.CompareTo(b.Name);
            }
        }
    }
}