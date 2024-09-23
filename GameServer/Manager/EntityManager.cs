using GameServer.Model;
using System.Collections.Generic;
using Kirara;

namespace GameServer.Manager
{
    /// <summary>
    /// Entity管理器
    /// </summary>
    public class EntityManager : Singleton<EntityManager>
    {
        private int index = 1;
        private Dictionary<int, Entity> idToEntity = new();

        public Entity CreateEntity()
        {
            lock (this)
            {
                Entity entity = new(index, Vector3Int.zero, Vector3Int.zero);
                idToEntity.Add(index, entity);
                index++;
                return entity;
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
