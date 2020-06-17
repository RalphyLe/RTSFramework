using System.Collections.Generic;

namespace Framework.Editor
{
    /// <summary>
    /// 资源包
    /// </summary>
    public sealed class Resource
    {
        private readonly List<Asset> m_Assets;
        private readonly List<string> m_ResourceGroups;

        private Resource(string name, string variant, LoadType loadType, bool packed, string[] resourceGroups)
        {
            m_Assets = new List<Asset>();
            m_ResourceGroups = new List<string>();

            Name = name;
            Variant = variant;
            AssetType = AssetType.Unknown;
            LoadType = loadType;
            Packed = packed;

            foreach (string resourceGroup in resourceGroups)
            {
                AddResourceGroup(resourceGroup);
            }
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
        /// 后缀
        /// </summary>
        public string Variant
        {
            get;
            private set;
        }

        /// <summary>
        /// 全名
        /// </summary>
        public string FullName
        {
            get
            {
                return Variant != null ? string.Format("{0}.{1}", Name, Variant) : Name;
            }
        }

        /// <summary>
        /// 资源类型
        /// </summary>
        public AssetType AssetType
        {
            get;
            private set;
        }

        /// <summary>
        /// 检查加载类型
        /// </summary>
        public bool IsLoadFromBinary
        {
            get
            {
                return LoadType == LoadType.LoadFromBinary || LoadType == LoadType.LoadFromBinaryAndQuickDecrypt || LoadType == LoadType.LoadFromBinaryAndDecrypt;
            }
        }

        /// <summary>
        /// 加载类型
        /// </summary>
        public LoadType LoadType
        {
            get;
            set;
        }

        /// <summary>
        /// 打包标记
        /// </summary>
        public bool Packed
        {
            get;
            set;
        }

        /// <summary>
        /// 创建资源包
        /// </summary>
        /// <param name="name"></param>
        /// <param name="variant"></param>
        /// <param name="loadType"></param>
        /// <param name="packed"></param>
        /// <param name="resourceGroups"></param>
        /// <returns></returns>
        public static Resource Create(string name, string variant, LoadType loadType, bool packed, string[] resourceGroups)
        {
            return new Resource(name, variant, loadType, packed, resourceGroups);
        }

        /// <summary>
        /// 获取资源包中的所有资源
        /// </summary>
        /// <returns></returns>
        public Asset[] GetAssets()
        {
            return m_Assets.ToArray();
        }

        /// <summary>
        /// 获取第一个资源
        /// </summary>
        /// <returns></returns>
        public Asset GetFirstAsset()
        {
            return m_Assets.Count > 0 ? m_Assets[0] : null;
        }

        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="name"></param>
        /// <param name="variant"></param>
        public void Rename(string name, string variant)
        {
            Name = name;
            Variant = variant;
        }

        /// <summary>
        /// 封装资源
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="isScene"></param>
        public void AssignAsset(Asset asset, bool isScene)
        {
            if (asset.Resource != null)
            {
                asset.Resource.UnassignAsset(asset);
            }

            AssetType = isScene ? AssetType.Scene : AssetType.Asset;
            asset.Resource = this;
            m_Assets.Add(asset);
            m_Assets.Sort(AssetComparer);
        }

        /// <summary>
        /// 移除资源
        /// </summary>
        /// <param name="asset"></param>
        public void UnassignAsset(Asset asset)
        {
            asset.Resource = null;
            m_Assets.Remove(asset);
            if (m_Assets.Count <= 0)
            {
                AssetType = AssetType.Unknown;
            }
        }

        /// <summary>
        /// 获取所有资源组合
        /// </summary>
        /// <returns></returns>
        public string[] GetResourceGroups()
        {
            return m_ResourceGroups.ToArray();
        }

        /// <summary>
        /// 检查是否包含资源组合
        /// </summary>
        /// <param name="resourceGroup"></param>
        /// <returns></returns>
        public bool HasResourceGroup(string resourceGroup)
        {
            if (string.IsNullOrEmpty(resourceGroup))
            {
                return false;
            }

            return m_ResourceGroups.Contains(resourceGroup);
        }

        /// <summary>
        /// 添加资源组合
        /// </summary>
        /// <param name="resourceGroup"></param>
        public void AddResourceGroup(string resourceGroup)
        {
            if (string.IsNullOrEmpty(resourceGroup))
            {
                return;
            }

            if (m_ResourceGroups.Contains(resourceGroup))
            {
                return;
            }

            m_ResourceGroups.Add(resourceGroup);
            m_ResourceGroups.Sort();
        }
        /// <summary>
        /// 移除资源组合
        /// </summary>
        /// <param name="resourceGroup"></param>
        /// <returns></returns>
        public bool RemoveResourceGroup(string resourceGroup)
        {
            if (string.IsNullOrEmpty(resourceGroup))
            {
                return false;
            }

            return m_ResourceGroups.Remove(resourceGroup);
        }

        public void Clear()
        {
            foreach (Asset asset in m_Assets)
            {
                asset.Resource = null;
            }

            m_Assets.Clear();
            m_ResourceGroups.Clear();
        }

        /// <summary>
        /// 资源排序回调方法
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private int AssetComparer(Asset a, Asset b)
        {
            return a.Guid.CompareTo(b.Guid);
        }
    }
}
