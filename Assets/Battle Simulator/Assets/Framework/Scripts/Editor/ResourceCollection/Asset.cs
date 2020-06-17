﻿using UnityEditor;

namespace Framework.Editor
{
    /// <summary>
    /// 资源。
    /// </summary>
    public sealed class Asset
    {
        private Asset(string guid, Resource resource)
        {
            Guid = guid;
            Resource = resource;
        }

        /// <summary>
        /// 获取Guid
        /// </summary>
        public string Guid
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取资源名字
        /// </summary>
        public string Name
        {
            get
            {
                return AssetDatabase.GUIDToAssetPath(Guid);
            }
        }

        /// <summary>
        /// 获取所属的资源包
        /// </summary>
        public Resource Resource
        {
            get;
            set;
        }

        /// <summary>
        /// 创建资源接口
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static Asset Create(string guid)
        {
            return new Asset(guid, null);
        }

        /// <summary>
        /// 创建资源接口
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static Asset Create(string guid, Resource resource)
        {
            return new Asset(guid, resource);
        }
    }
}