using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEditor;
using UnityEngine;
using Framework.Runtime;
using System;

namespace Framework.Editor
{
    /// <summary>
    /// 资源集合。
    /// </summary>
    public sealed class ResourceCollection
    {
        /// <summary>
        /// 场景的后缀
        /// </summary>
        private const string SceneExtension = ".unity";

        /// <summary>
        /// 正则表达式，检查资源名字
        /// </summary>
        private static readonly Regex ResourceNameRegex = new Regex(@"^([A-Za-z0-9\._-]+/)*[A-Za-z0-9\._-]+$");

        /// <summary>
        /// 正则表达式，检查命名变量
        /// </summary>
        private static readonly Regex ResourceVariantRegex = new Regex(@"^[a-z0-9_-]+$");

        /// <summary>
        /// 配置路径
        /// </summary>
        private readonly string m_ConfigurationPath;
        private readonly SortedDictionary<string, Resource> m_Resources;
        private readonly SortedDictionary<string, Asset> m_Assets;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ResourceCollection()
        {
            m_ConfigurationPath = Path.Combine(Application.dataPath, "Framework/Configs/ResourceCollection.xml");
            m_Resources = new SortedDictionary<string, Resource>();
            m_Assets = new SortedDictionary<string, Asset>();
        }

        /// <summary>
        /// 资源包数量
        /// </summary>
        public int ResourceCount
        {
            get
            {
                return m_Resources.Count;
            }
        }

        /// <summary>
        /// 资源数量
        /// </summary>
        public int AssetCount
        {
            get
            {
                return m_Assets.Count;
            }
        }

        public event GameFrameworkAction<int, int> OnLoadingResource = null;

        public event GameFrameworkAction<int, int> OnLoadingAsset = null;

        public event GameFrameworkAction OnLoadCompleted = null;

        public void Clear()
        {
            m_Resources.Clear();
            m_Assets.Clear();
        }

        /// <summary>
        /// 加载资源集和资源
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {
            Clear();

            if (!File.Exists(m_ConfigurationPath))
            {
                return false;
            }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(m_ConfigurationPath);
                XmlNode xmlRoot = xmlDocument.SelectSingleNode("UnityGameFramework");
                XmlNode xmlCollection = xmlRoot.SelectSingleNode("ResourceCollection");
                XmlNode xmlResources = xmlCollection.SelectSingleNode("Resources");
                XmlNode xmlAssets = xmlCollection.SelectSingleNode("Assets");

                XmlNodeList xmlNodeList = null;
                XmlNode xmlNode = null;
                int count = 0;

                xmlNodeList = xmlResources.ChildNodes;
                count = xmlNodeList.Count;
                for (int i = 0; i < count; i++)
                {
                    if (OnLoadingResource != null)
                    {
                        OnLoadingResource(i, count);
                    }

                    xmlNode = xmlNodeList.Item(i);
                    if (xmlNode.Name != "Resource")
                    {
                        continue;
                    }

                    string name = xmlNode.Attributes.GetNamedItem("Name").Value;
                    string variant = xmlNode.Attributes.GetNamedItem("Variant") != null ? xmlNode.Attributes.GetNamedItem("Variant").Value : null;
                    byte loadType = 0;
                    if (xmlNode.Attributes.GetNamedItem("LoadType") != null)
                    {
                        byte.TryParse(xmlNode.Attributes.GetNamedItem("LoadType").Value, out loadType);
                    }

                    bool packed = false;
                    if (xmlNode.Attributes.GetNamedItem("Packed") != null)
                    {
                        bool.TryParse(xmlNode.Attributes.GetNamedItem("Packed").Value, out packed);
                    }

                    string[] resourceGroups = xmlNode.Attributes.GetNamedItem("ResourceGroups") != null ? xmlNode.Attributes.GetNamedItem("ResourceGroups").Value.Split(',') : new string[0];
                    if (!AddResource(name, variant, (LoadType)loadType, packed, resourceGroups))
                    {
                        Debug.LogWarning(string.Format("Can not add resource '{0}'.", GetResourceFullName(name, variant)));
                        continue;
                    }
                }

                xmlNodeList = xmlAssets.ChildNodes;
                count = xmlNodeList.Count;
                for (int i = 0; i < count; i++)
                {
                    if (OnLoadingAsset != null)
                    {
                        OnLoadingAsset(i, count);
                    }

                    xmlNode = xmlNodeList.Item(i);
                    if (xmlNode.Name != "Asset")
                    {
                        continue;
                    }

                    string guid = xmlNode.Attributes.GetNamedItem("Guid").Value;
                    string name = xmlNode.Attributes.GetNamedItem("ResourceName").Value;
                    string variant = xmlNode.Attributes.GetNamedItem("ResourceVariant") != null ? xmlNode.Attributes.GetNamedItem("ResourceVariant").Value : null;
                    if (!AssignAsset(guid, name, variant))
                    {
                        Debug.LogWarning(string.Format("Can not assign asset '{0}' to resource '{1}'.", guid, GetResourceFullName(name, variant)));
                        continue;
                    }
                }

                if (OnLoadCompleted != null)
                {
                    OnLoadCompleted();
                }

                return true;
            }
            catch
            {
                File.Delete(m_ConfigurationPath);
                if (OnLoadCompleted != null)
                {
                    OnLoadCompleted();
                }

                return false;
            }
        }

        /// <summary>
        /// 保存配置文件
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));

                XmlElement xmlRoot = xmlDocument.CreateElement("UnityGameFramework");
                xmlDocument.AppendChild(xmlRoot);

                XmlElement xmlCollection = xmlDocument.CreateElement("ResourceCollection");
                xmlRoot.AppendChild(xmlCollection);

                XmlElement xmlResources = xmlDocument.CreateElement("Resources");
                xmlCollection.AppendChild(xmlResources);

                XmlElement xmlAssets = xmlDocument.CreateElement("Assets");
                xmlCollection.AppendChild(xmlAssets);

                XmlElement xmlElement = null;
                XmlAttribute xmlAttribute = null;

                foreach (Resource resource in m_Resources.Values)
                {
                    xmlElement = xmlDocument.CreateElement("Resource");
                    xmlAttribute = xmlDocument.CreateAttribute("Name");
                    xmlAttribute.Value = resource.Name;
                    xmlElement.Attributes.SetNamedItem(xmlAttribute);

                    if (resource.Variant != null)
                    {
                        xmlAttribute = xmlDocument.CreateAttribute("Variant");
                        xmlAttribute.Value = resource.Variant;
                        xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    }

                    xmlAttribute = xmlDocument.CreateAttribute("LoadType");
                    xmlAttribute.Value = ((byte)resource.LoadType).ToString();
                    xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    xmlAttribute = xmlDocument.CreateAttribute("Packed");
                    xmlAttribute.Value = resource.Packed.ToString();
                    xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    string[] resourceGroups = resource.GetResourceGroups();
                    if (resourceGroups.Length > 0)
                    {
                        xmlAttribute = xmlDocument.CreateAttribute("ResourceGroups");
                        xmlAttribute.Value = string.Join(",", resourceGroups);
                        xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    }

                    xmlResources.AppendChild(xmlElement);
                }

                foreach (Asset asset in m_Assets.Values)
                {
                    xmlElement = xmlDocument.CreateElement("Asset");
                    xmlAttribute = xmlDocument.CreateAttribute("Guid");
                    xmlAttribute.Value = asset.Guid;
                    xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    xmlAttribute = xmlDocument.CreateAttribute("ResourceName");
                    xmlAttribute.Value = asset.Resource.Name;
                    xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    if (asset.Resource.Variant != null)
                    {
                        xmlAttribute = xmlDocument.CreateAttribute("ResourceVariant");
                        xmlAttribute.Value = asset.Resource.Variant;
                        xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    }

                    xmlAssets.AppendChild(xmlElement);
                }

                string configurationDirectoryName = Path.GetDirectoryName(m_ConfigurationPath);
                if (!Directory.Exists(configurationDirectoryName))
                {
                    Directory.CreateDirectory(configurationDirectoryName);
                }

                xmlDocument.Save(m_ConfigurationPath);
                AssetDatabase.Refresh();
                return true;
            }
            catch
            {
                if (File.Exists(m_ConfigurationPath))
                {
                    File.Delete(m_ConfigurationPath);
                }

                return false;
            }
        }

        /// <summary>
        /// 获取所有资源集
        /// </summary>
        /// <returns></returns>
        public Resource[] GetResources()
        {
            return m_Resources.Values.ToArray();
        }

        /// <summary>
        /// 获取指定名字的资源集
        /// </summary>
        /// <param name="name">资源集名</param>
        /// <param name="variant">后缀</param>
        /// <returns></returns>
        public Resource GetResource(string name, string variant)
        {
            if (!IsValidResourceName(name, variant))
            {
                return null;
            }

            Resource resource = null;
            if (m_Resources.TryGetValue(GetResourceFullName(name, variant).ToLower(), out resource))
            {
                return resource;
            }

            return null;
        }

        /// <summary>
        /// 查询资源集是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <param name="variant"></param>
        /// <returns></returns>
        public bool HasResource(string name, string variant)
        {
            if (!IsValidResourceName(name, variant))
            {
                return false;
            }

            return m_Resources.ContainsKey(GetResourceFullName(name, variant).ToLower());
        }

        /// <summary>
        /// 添加资源集
        /// </summary>
        /// <param name="name"></param>
        /// <param name="variant"></param>
        /// <param name="loadType"></param>
        /// <param name="packed"></param>
        /// <returns></returns>
        public bool AddResource(string name, string variant, LoadType loadType, bool packed)
        {
            return AddResource(name, variant, loadType, packed, new string[0]);
        }

        /// <summary>
        /// 添加资源集
        /// </summary>
        /// <param name="name"></param>
        /// <param name="variant"></param>
        /// <param name="loadType"></param>
        /// <param name="packed"></param>
        /// <param name="resourceGroups"></param>
        /// <returns></returns>
        public bool AddResource(string name, string variant, LoadType loadType, bool packed, string[] resourceGroups)
        {
            if (!IsValidResourceName(name, variant))
            {
                return false;
            }

            if (!IsAvailableResourceName(name, variant, null))
            {
                return false;
            }

            Resource resource = Resource.Create(name, variant, loadType, packed, resourceGroups);
            m_Resources.Add(resource.FullName.ToLower(), resource);

            return true;
        }

        /// <summary>
        /// 资源集重命名
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="oldVariant"></param>
        /// <param name="newName"></param>
        /// <param name="newVariant"></param>
        /// <returns></returns>
        public bool RenameResource(string oldName, string oldVariant, string newName, string newVariant)
        {
            if (!IsValidResourceName(oldName, oldVariant) || !IsValidResourceName(newName, newVariant))
            {
                return false;
            }

            Resource resource = GetResource(oldName, oldVariant);
            if (resource == null)
            {
                return false;
            }

            if (oldName == newName && oldVariant == newVariant)
            {
                return true;
            }

            if (!IsAvailableResourceName(newName, newVariant, resource))
            {
                return false;
            }

            m_Resources.Remove(resource.FullName.ToLower());
            resource.Rename(newName, newVariant);
            m_Resources.Add(resource.FullName.ToLower(), resource);

            return true;
        }

        /// <summary>
        /// 移除资源集
        /// </summary>
        /// <param name="name"></param>
        /// <param name="variant"></param>
        /// <returns></returns>
        public bool RemoveResource(string name, string variant)
        {
            if (!IsValidResourceName(name, variant))
            {
                return false;
            }

            Resource resource = GetResource(name, variant);
            if (resource == null)
            {
                return false;
            }

            Asset[] assets = resource.GetAssets();
            resource.Clear();
            m_Resources.Remove(resource.FullName.ToLower());
            foreach (Asset asset in assets)
            {
                m_Assets.Remove(asset.Guid);
            }

            return true;
        }

        /// <summary>
        /// 设置资源集加载方式
        /// </summary>
        /// <param name="name"></param>
        /// <param name="variant"></param>
        /// <param name="loadType"></param>
        /// <returns></returns>
        public bool SetResourceLoadType(string name, string variant, LoadType loadType)
        {
            if (!IsValidResourceName(name, variant))
            {
                return false;
            }

            Resource resource = GetResource(name, variant);
            if (resource == null)
            {
                return false;
            }

            if ((loadType == LoadType.LoadFromBinary || loadType == LoadType.LoadFromBinaryAndQuickDecrypt || loadType == LoadType.LoadFromBinaryAndDecrypt) && resource.GetAssets().Length > 1)
            {
                return false;
            }

            resource.LoadType = loadType;
            return true;
        }

        /// <summary>
        /// 设置资源集pack标志
        /// </summary>
        /// <param name="name"></param>
        /// <param name="variant"></param>
        /// <param name="packed"></param>
        /// <returns></returns>
        public bool SetResourcePacked(string name, string variant, bool packed)
        {
            if (!IsValidResourceName(name, variant))
            {
                return false;
            }

            Resource resource = GetResource(name, variant);
            if (resource == null)
            {
                return false;
            }

            resource.Packed = packed;
            return true;
        }

        /// <summary>
        /// 获取所有资源
        /// </summary>
        /// <returns></returns>
        public Asset[] GetAssets()
        {
            return m_Assets.Values.ToArray();
        }

        /// <summary>
        /// 获取资源集的所有资源
        /// </summary>
        /// <param name="name">资源集名</param>
        /// <param name="variant"></param>
        /// <returns></returns>
        public Asset[] GetAssets(string name, string variant)
        {
            if (!IsValidResourceName(name, variant))
            {
                return new Asset[0];
            }

            Resource resource = GetResource(name, variant);
            if (resource == null)
            {
                return new Asset[0];
            }

            return resource.GetAssets();
        }

        /// <summary>
        /// 通过guid获取资源
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public Asset GetAsset(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }

            Asset asset = null;
            if (m_Assets.TryGetValue(guid, out asset))
            {
                return asset;
            }

            return null;
        }

        /// <summary>
        /// 通过guid查找资源是否存在
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool HasAsset(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return false;
            }

            return m_Assets.ContainsKey(guid);
        }

        /// <summary>
        /// 将资源分配到资源集
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="name">资源集名</param>
        /// <param name="variant">资源集后缀</param>
        /// <returns></returns>
        public bool AssignAsset(string guid, string name, string variant)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return false;
            }

            if (!IsValidResourceName(name, variant))
            {
                return false;
            }

            Resource resource = GetResource(name, variant);
            if (resource == null)
            {
                return false;
            }

            string assetName = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(assetName))
            {
                return false;
            }

            Asset[] assetsInResource = resource.GetAssets();
            foreach (Asset assetInResource in assetsInResource)
            {
                if (assetInResource.Name == assetName)
                {
                    continue;
                }

                if (assetInResource.Name.ToLower() == assetName.ToLower())
                {
                    return false;
                }
            }

            bool isScene = assetName.EndsWith(SceneExtension);
            if (isScene && resource.AssetType == AssetType.Asset || !isScene && resource.AssetType == AssetType.Scene)
            {
                return false;
            }

            Asset asset = GetAsset(guid);
            if (resource.IsLoadFromBinary && assetsInResource.Length > 0 && asset != assetsInResource[0])
            {
                return false;
            }

            if (asset == null)
            {
                asset = Asset.Create(guid);
                m_Assets.Add(asset.Guid, asset);
            }

            resource.AssignAsset(asset, isScene);

            return true;
        }

        /// <summary>
        /// 解除资源的分配
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool UnassignAsset(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return false;
            }

            Asset asset = GetAsset(guid);
            if (asset != null)
            {
                asset.Resource.UnassignAsset(asset);
                m_Assets.Remove(asset.Guid);
            }

            return true;
        }
        /// <summary>
        /// 获取资源全名
        /// </summary>
        /// <param name="name"></param>
        /// <param name="variant"></param>
        /// <returns></returns>
        private string GetResourceFullName(string name, string variant)
        {
            return !string.IsNullOrEmpty(variant) ? string.Format("{0}.{1}", name, variant) : name;
        }

        /// <summary>
        /// 检查资源名的规范性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="variant"></param>
        /// <returns></returns>
        private bool IsValidResourceName(string name, string variant)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (!ResourceNameRegex.IsMatch(name))
            {
                return false;
            }

            if (variant != null && !ResourceVariantRegex.IsMatch(variant))
            {
                return false;
            }

            return true;
        }

        private bool IsAvailableResourceName(string name, string variant, Resource current)
        {
            Resource found = GetResource(name, variant);
            if (found != null && found != current)
            {
                return false;
            }

            string[] foundPathNames = name.Split('/');
            foreach (Resource resource in m_Resources.Values)
            {
                if (current != null && resource == current)
                {
                    continue;
                }

                if (resource.Name == name)
                {
                    if (resource.Variant == null && variant != null)
                    {
                        return false;
                    }

                    if (resource.Variant != null && variant == null)
                    {
                        return false;
                    }
                }

                if (resource.Name.Length > name.Length
                    && resource.Name.IndexOf(name, StringComparison.CurrentCultureIgnoreCase) == 0
                    && resource.Name[name.Length] == '/')
                {
                    return false;
                }

                if (name.Length > resource.Name.Length
                    && name.IndexOf(resource.Name, StringComparison.CurrentCultureIgnoreCase) == 0
                    && name[resource.Name.Length] == '/')
                {
                    return false;
                }

                string[] pathNames = resource.Name.Split('/');
                for (int i = 0; i < foundPathNames.Length - 1 && i < pathNames.Length - 1; i++)
                {
                    if (foundPathNames[i].ToLower() != pathNames[i].ToLower())
                    {
                        break;
                    }

                    if (foundPathNames[i] != pathNames[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}