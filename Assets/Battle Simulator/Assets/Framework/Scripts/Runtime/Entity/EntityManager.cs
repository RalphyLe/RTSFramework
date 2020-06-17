using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Framework.Runtime
{
    public class EntityManager : ManagerBase, IEntityManager
    {
        private readonly Dictionary<int, EntityInfo> m_EntityInfos;
        private readonly Dictionary<string, EntityGroup> m_EntityGroups;
        private readonly Dictionary<int, int> m_EntitiesBeingLoaded;
        private readonly Queue<EntityInfo> m_RecycleQueue;

        /// <summary>
        /// 初始化实体管理器的新实例。
        /// </summary>
        public EntityManager()
        {
            //m_EntityInfos = new Dictionary<int, EntityInfo>();
            //m_EntityGroups = new Dictionary<string, EntityGroup>();
            //m_EntitiesBeingLoaded = new Dictionary<int, int>();
            //m_EntitiesToReleaseOnLoad = new HashSet<int>();
            //m_RecycleQueue = new Queue<EntityInfo>();
            //m_LoadAssetCallbacks = new LoadAssetCallbacks(LoadAssetSuccessCallback, LoadAssetFailureCallback, LoadAssetUpdateCallback, LoadAssetDependencyAssetCallback);
            //m_ObjectPoolManager = null;
            //m_ResourceManager = null;
            //m_EntityHelper = null;
            //m_Serial = 0;
            //m_IsShutdown = false;
            //m_ShowEntitySuccessEventHandler = null;
            //m_ShowEntityFailureEventHandler = null;
            //m_ShowEntityUpdateEventHandler = null;
            //m_ShowEntityDependencyAssetEventHandler = null;
            //m_HideEntityCompleteEventHandler = null;
        }

        /// <summary>
        /// 获取实体数量。
        /// </summary>
        public int EntityCount
        {
            get
            {
                return m_EntityInfos.Count;
            }
        }

        /// <summary>
        /// 获取实体组数量。
        /// </summary>
        public int EntityGroupCount
        {
            get
            {
                return m_EntityGroups.Count;
            }
        }

        /// <summary>
        /// 实体管理器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            while (m_RecycleQueue.Count > 0)
            {
                EntityInfo entityInfo = m_RecycleQueue.Dequeue();
                IEntity entity = entityInfo.Entity;
                EntityGroup entityGroup = (EntityGroup)entity.EntityGroup;
                if (entityGroup == null)
                {
                    throw new GameFrameworkException("Entity group is invalid.");
                }

                entityInfo.Status = EntityStatus.WillRecycle;
                entity.OnRecycle();
                entityInfo.Status = EntityStatus.Recycled;
                entityGroup.UnspawnEntity(entity);
                ReferencePool.Release(entityInfo);
            }

            foreach (KeyValuePair<string, EntityGroup> entityGroup in m_EntityGroups)
            {
                entityGroup.Value.Update(elapseSeconds, realElapseSeconds);
            }
        }
    }
}