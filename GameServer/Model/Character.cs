using GameServer.Database;
using GameServer.Manager;
using Kirara;
using Proto;

namespace GameServer.Model
{
    // 角色
    public class Character : Entity
    {
        public int id;
        public int tid;
        public string name;
        public int hp;
        public int mp;
        public int level;
        public int exp;
        public int spaceId;
        public long gold;
        public int playerId;
        
        public Connection conn;

        public Space space { get; set; }

        public NCharacter NCharacter => new()
        {
            NEntity = NEntity,
            Id = id,
            Tid = tid,
            Name = name,
            Hp = hp,
            Mp = mp,
            Level = level,
            Exp = exp,
            SpaceId = spaceId,
            Gold = gold,
            EntityType = entityType,
        };

        public DbCharacter DbCharacter => new()
        {
            Id = id,
            JobId = tid,
            Name = name,
            Hp = hp,
            Mp = mp,
            Level = level,
            Exp = exp,
            SpaceId = spaceId,
            X = position.x,
            Y = position.y,
            Z = position.z,
            Gold = gold,
            PlayerId = playerId,
        };

        public Character(DbCharacter dbCharacter) : base(
            new Float3(dbCharacter.X, dbCharacter.Y, dbCharacter.Z),
            Float3.Zero, EntityType.Character, dbCharacter.JobId)
        {
            id = dbCharacter.Id;
            tid = dbCharacter.JobId;
            name = dbCharacter.Name;
            hp = dbCharacter.Hp;
            mp = dbCharacter.Mp;
            level = dbCharacter.Level;
            exp = dbCharacter.Exp;
            spaceId = dbCharacter.SpaceId;
            gold = dbCharacter.Gold;
            playerId = dbCharacter.PlayerId;
            
            var ud = DefineManager.Instance.TIDToUnitDefine[dbCharacter.JobId];
            speed = ud.Speed;
        }
    }
}
