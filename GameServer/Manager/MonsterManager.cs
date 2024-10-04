using System.Collections.Generic;
using GameServer.Model;
using Kirara;

namespace GameServer.Manager
{
    public class MonsterManager
    {
        private readonly Space space;


        private Dictionary<int, Monster> entityIdToMonster = new();

        public MonsterManager(Space space)
        {
            this.space = space;
        }

        public Monster CreateMonster(int tid, int level, Float3 position, Float3 direction)
        {
            var monster = new Monster(position, direction, tid, level)
            {
                spaceId = space.id,
            };
            EntityManager.Instance.AddEntity(space.id, monster);
            entityIdToMonster.Add(monster.entityId, monster);
            return monster;
        }
    }
}