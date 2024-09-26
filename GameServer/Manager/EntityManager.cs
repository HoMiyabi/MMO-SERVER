using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameServer.Model;
using Kirara;
using Serilog;

namespace GameServer.Manager
{
    /// <summary>
    /// Entity管理器（角色，怪物，NPC，陷阱）
    /// </summary>
    public class EntityManager : Singleton<EntityManager>
    {
        private int id = 0;
        // 记录全部的Entity对象，<EntityId, Entity>
        private ConcurrentDictionary<int, Entity> entityIdToEntity = new();
        // 记录场景中的Entity列表，<SpaceId, EntityList>
        private ConcurrentDictionary<int, List<Entity>> spaceIdToEntities = new();

        public void AddEntity(int spaceId, Entity entity)
        {
            entity.NEntity.Id = NextEntityId;
            if (!entityIdToEntity.TryAdd(entity.EntityId, entity))
            {
                Log.Warning($"不能添加到entityIdToEntity {entity.EntityId.NameValue()}");
            }

            if (!spaceIdToEntities.TryAdd(spaceId, new List<Entity>() {entity}))
            {
                var list = spaceIdToEntities[spaceId];
                lock (list)
                {
                    list.Add(entity);
                }
            }
        }

        public void RemoveEntity(int spaceId, Entity entity)
        {
            if (!entityIdToEntity.TryRemove(entity.EntityId, out _))
            {
                Log.Warning($"不能删除 {entity.EntityId.NameValue()}");
            }

            if (spaceIdToEntities.TryGetValue(spaceId, out var list))
            {
                lock (list)
                {
                    list.RemoveAll(it => it.EntityId == entity.EntityId);
                }
            }
            else
            {
                Log.Warning($"场景找不到 {spaceId.NameValue()} {entity.EntityId.NameValue()}");
            }
        }

        public Entity GetEntity(int entityId)
        {
            return entityIdToEntity.GetValueOrDefault(entityId);
        }

        private int NextEntityId => Interlocked.Increment(ref id);
    }
}
