using Kirara;
using Proto;

namespace GameServer.Model
{
    public class Monster : Entity
    {
        public int level;
        public int spaceId;
        public string name;
        public int tid;

        public Space space;

        public NMonster NMonster => new()
        {
            NEntity = NEntity,
            Name = name,
            Level = level,
            Tid = tid
        };

        public Monster(Float3 position, Float3 direction, int tid, int level, string name) : base(position, direction, EntityType.Monster, tid)
        {
            this.level = level;
            this.name = name;
            this.tid = tid;
        }
    }
}
