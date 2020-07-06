using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Framework.Runtime
{

    /// <summary>
    /// 实体状态。
    /// </summary>
    public enum EntityStatus : byte
    {
        Unknown = 0,
        WillInit,
        Inited,
        WillShow,
        Showed,
        WillHide,
        Hidden,
        WillRecycle,
        Recycled
    }

    /// <summary>
    /// 实体信息
    /// </summary>
    public class EntityInfo : IReference
    {
        private IEntity m_Entity;
        private EntityStatus m_Status;
        private IEntity m_ParentEntity;
        private List<IEntity> m_ChildEntities;

        /// <summary>
        /// 创建实现信息实例
        /// </summary>
        public EntityInfo()
        {
            m_Entity = null;
            m_Status = EntityStatus.Unknown;
            m_ParentEntity = null;
            m_ChildEntities = new List<IEntity>();
        }

        /// <summary>
        /// 获取实体对象
        /// </summary>
        public IEntity Entity
        {
            get
            {
                return m_Entity;
            }
        }

        /// <summary>
        /// 获取实体状态
        /// </summary>
        public EntityStatus Status
        {
            get
            {
                return m_Status;
            }
            set
            {
                m_Status = value;
            }
        }

        /// <summary>
        /// 获取实体父对象
        /// </summary>
        public IEntity ParentEntity
        {
            get
            {
                return m_ParentEntity;
            }
            set
            {
                m_ParentEntity = value;
            }
        }

        /// <summary>
        /// 根据实体创建实体信息
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public static EntityInfo Create(IEntity entity)
        {
            if (entity == null)
            {
                Debug.LogError("Entity is invalid.");
            }

            EntityInfo entityInfo = ReferencePool.Acquire<EntityInfo>();
            entityInfo.m_Entity = entity;
            entityInfo.m_Status = EntityStatus.WillInit;
            return entityInfo;
        }

        /// <summary>
        /// 清理对象
        /// </summary>
        public void Clear()
        {
            m_Entity = null;
            m_Status = EntityStatus.Unknown;
            m_ParentEntity = null;
            m_ChildEntities.Clear();
        }

        /// <summary>
        /// 获取所有子实体
        /// </summary>
        /// <returns></returns>
        public IEntity[] GetChildEntities()
        {
            return m_ChildEntities.ToArray();
        }

        /// <summary>
        /// 获取所有子实体
        /// </summary>
        /// <param name="results"></param>
        public void GetChildEntities(List<IEntity> results)
        {
            if (results == null)
            {
                Debug.LogError("Results is invalid.");
            }

            results.Clear();
            foreach (IEntity childEntity in m_ChildEntities)
            {
                results.Add(childEntity);
            }
        }

        /// <summary>
        /// 添加子实体
        /// </summary>
        /// <param name="childEntity"></param>
        public void AddChildEntity(IEntity childEntity)
        {
            if (m_ChildEntities.Contains(childEntity))
            {
                Debug.LogError("Can not add child entity which is already exist.");
            }

            m_ChildEntities.Add(childEntity);
        }

        /// <summary>
        /// 移除子实体
        /// </summary>
        /// <param name="childEntity"></param>
        public void RemoveChildEntity(IEntity childEntity)
        {
            if (!m_ChildEntities.Remove(childEntity))
            {
                Debug.LogError("Can not remove child entity which is not exist.");
            }
        }
    } 
}