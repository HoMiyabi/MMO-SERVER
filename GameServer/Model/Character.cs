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
            nCharacter = new NCharacter
            {
                NEntity = NEntity,
                Id = dbCharacter.Id,
                JobId = dbCharacter.JobId,
                Name = dbCharacter.Name,
                Hp = dbCharacter.Hp,
                Mp = dbCharacter.Mp,
                Level = dbCharacter.Level,
                Exp = dbCharacter.Exp,
                SpaceId = dbCharacter.SpaceId,
                Gold = dbCharacter.Gold,
            };
        }
    }
}
