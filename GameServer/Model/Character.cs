using GameServer.Database;
using Kirara;
using Proto;

namespace GameServer.Model
{
    // 角色
    public class Character : Entity
    {
        public Connection conn;

        public int characterId => dbCharacter.Id;

        public Space space { get; set; }

        public NCharacter nCharacter { get; set; }

        public DbCharacter dbCharacter;

        public Character(DbCharacter dbCharacter) : base(
            new Int3(dbCharacter.X, dbCharacter.Y, dbCharacter.Z),
            Int3.zero)
        {
            this.dbCharacter = dbCharacter;
            nCharacter = new NCharacter()
            {
                Id = dbCharacter.Id,
                TypeId = dbCharacter.JobId,
                EntityId = 0,
                Name = dbCharacter.Name,
                Level = dbCharacter.Level,
                Exp = dbCharacter.Exp,
                SpaceId = dbCharacter.SpaceId,
                Gold = dbCharacter.Gold,
                Entity = NEntity,
                Hp = dbCharacter.Hp,
                Mp = dbCharacter.Mp,
            };
        }
    }
}
