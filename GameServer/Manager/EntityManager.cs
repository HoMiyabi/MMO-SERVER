using System.Collections.Concurrent;
using GameServer.Model;
using System.Collections.Generic;
using Kirara;
using Serilog;

namespace GameServer.Manager
{
    /// <summary>
    /// Entity管理器（角色，怪物，NPC，陷阱）
    /// </summary>
    public class EntityManager : Singleton<EntityManager>
    {
        private int index = 1;
        private ConcurrentDictionary<int, Entity> idToEntity = new();

        // public Entity CreateEntity()
        // {
        //     lock (this)
        //     {
        //         Entity entity = new(index, Vector3Int.zero, Vector3Int.zero);
        //         idToEntity.Add(index, entity);
        //         index++;
        //         return entity;
        //     }
        // }

        public void AddEntity(int spaceId, Entity entity)
        {
            if (!idToEntity.TryAdd(entity.EntityId, entity))
            {
                Log.Warning($"不能添加 entityId={entity.EntityId}");
            }
        }

        public int NextEntityId
        {
            get
            {
                lock (this)
                {
                    int id = index;
                    index++;
                    return id;
                }
            }
        }
    }
}
