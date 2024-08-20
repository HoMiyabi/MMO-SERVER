using GameServer.Model;
using Summer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
