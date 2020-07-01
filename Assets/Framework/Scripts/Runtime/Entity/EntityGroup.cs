using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Framework.Runtime
{
    /// <summary>
    /// 实体组
    /// </summary>
    public class EntityGroup : IEntityGroup
    {
        string IEntityGroup.Name => throw new System.NotImplementedException();

        int IEntityGroup.EntityCount => throw new System.NotImplementedException();

        float IEntityGroup.InstanceAutoReleaseInterval { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        int IEntityGroup.InstanceCapacity { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        float IEntityGroup.InstanceExpireTime { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        int IEntityGroup.InstancePriority { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        IEntityGroupHelper IEntityGroup.Helper => throw new System.NotImplementedException();

        IEntity[] IEntityGroup.GetAllEntities()
        {
            throw new System.NotImplementedException();
        }

        void IEntityGroup.GetAllEntities(List<IEntity> results)
        {
            throw new System.NotImplementedException();
        }

        IEntity[] IEntityGroup.GetEntities(string entityAssetName)
        {
            throw new System.NotImplementedException();
        }

        void IEntityGroup.GetEntities(string entityAssetName, List<IEntity> results)
        {
            throw new System.NotImplementedException();
        }

        IEntity IEntityGroup.GetEntity(int entityId)
        {
            throw new System.NotImplementedException();
        }

        IEntity IEntityGroup.GetEntity(string entityAssetName)
        {
            throw new System.NotImplementedException();
        }

        bool IEntityGroup.HasEntity(int entityId)
        {
            throw new System.NotImplementedException();
        }

        bool IEntityGroup.HasEntity(string entityAssetName)
        {
            throw new System.NotImplementedException();
        }

        void IEntityGroup.SetEntityInstanceLocked(object entityInstance, bool locked)
        {
            throw new System.NotImplementedException();
        }

        void IEntityGroup.SetEntityInstancePriority(object entityInstance, int priority)
        {
            throw new System.NotImplementedException();
        }
    }
}