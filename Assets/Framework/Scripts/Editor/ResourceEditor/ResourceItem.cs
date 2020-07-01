using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public partial class ResourceEditor : EditorWindow
    {
        /// <summary>
        /// 资源集窗口管理类
        /// </summary>
        private class ResourceItem
        {
            //图标
            private static Texture s_CachedUnknownIcon = null;
            private static Texture s_CachedAssetIcon = null;
            private static Texture s_CachedSceneIcon = null;
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="name">资源名</param>
            /// <param name="resource">管理的资源集</param>
            /// <param name="folder">资源文件夹</param>
            public ResourceItem(string name, Resource resource, ResourceFolder folder)
            {
                if (resource == null)
                {
                    Debug.LogError("Resource is invalid.");
                }

                if (folder == null)
                {
                    Debug.LogError("Resource folder is invalid.");
                }

                Name = name;
                Resource = resource;
                Folder = folder;
            }

            /// <summary>
            /// 获取资源集名
            /// </summary>
            public string Name
            {
                get;
                private set;
            }
            /// <summary>
            /// 获取管理的资源
            /// </summary>
            public Resource Resource
            {
                get;
                private set;
            }

            /// <summary>
            /// 资源文件夹
            /// </summary>
            public ResourceFolder Folder
            {
                get;
                private set;
            }

            /// <summary>
            /// 获取资源加载的根目录
            /// </summary>
            public string FromRootPath
            {
                get
                {
                    return Folder.Folder == null ? Name : string.Format("{0}/{1}", Folder.FromRootPath, Name);
                }
            }

            /// <summary>
            /// 获取资源路径深度
            /// </summary>
            public int Depth
            {
                get
                {
                    return Folder != null ? Folder.Depth + 1 : 0;
                }
            }

            public Texture Icon
            {
                get
                {
                    if (Resource.IsLoadFromBinary)
                    {
                        Asset asset = Resource.GetFirstAsset();
                        if (asset != null)
                        {
                            return AssetDatabase.GetCachedIcon(AssetDatabase.GUIDToAssetPath(asset.Guid));
                        }
                    }
                    else
                    {
                        switch (Resource.AssetType)
                        {
                            case AssetType.Asset:
                                return CachedAssetIcon;

                            case AssetType.Scene:
                                return CachedSceneIcon;
                        }
                    }

                    return CachedUnknownIcon;
                }
            }

            private static Texture CachedUnknownIcon
            {
                get
                {
                    if (s_CachedUnknownIcon == null)
                    {
                        string iconName = null;
#if UNITY_2018_3_OR_NEWER
                        iconName = "GameObject Icon";
#else
                        iconName = "Prefab Icon";
#endif
                        s_CachedUnknownIcon = GetIcon(iconName);
                    }

                    return s_CachedUnknownIcon;
                }
            }

            private static Texture CachedAssetIcon
            {
                get
                {
                    if (s_CachedAssetIcon == null)
                    {
                        string iconName = null;
#if UNITY_2018_3_OR_NEWER
                        iconName = "Prefab Icon";
#else
                        iconName = "PrefabNormal Icon";
#endif
                        s_CachedAssetIcon = GetIcon(iconName);
                    }

                    return s_CachedAssetIcon;
                }
            }

            private static Texture CachedSceneIcon
            {
                get
                {
                    if (s_CachedSceneIcon == null)
                    {
                        s_CachedSceneIcon = GetIcon("SceneAsset Icon");
                    }

                    return s_CachedSceneIcon;
                }
            }

            private static Texture GetIcon(string iconName)
            {
                return EditorGUIUtility.IconContent(iconName).image;
            }
        }
    }
}