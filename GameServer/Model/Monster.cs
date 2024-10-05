using Kirara;
using Proto;

namespace GameServer.Model
{
    public class Monster : Entity
    {
        public int level;
        public int spaceId;
        public string name;

        public Space space;

        public NMonster NMonster => new()
        {
            NEntity = NEntity,
            Name = name,
            Level = level,
            Tid = tid
        };

        public Monster(Float3 position, Float3 direction, int TID, int level, string name) : base(position, direction, EntityType.Monster, TID)
        {
            this.level = level;
            this.name = name;
        }
    }
}
