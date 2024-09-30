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
            new Float3(dbCharacter.X, dbCharacter.Y, dbCharacter.Z),
            Float3.Zero)
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
            speed = 10;
        }
    }
}
