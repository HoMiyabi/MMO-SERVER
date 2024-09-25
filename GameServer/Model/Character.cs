using GameServer.Database;
using GameServer.Manager;
using Kirara;

namespace GameServer.Model
{
    // 角色
    public class Character : Actor
    {
        // 当前角色的客户端连接
        public Connection conn;

        public DbCharacter dbCharacter;

        public Character(DbCharacter dbCharacter) : base(
            new Vector3Int(dbCharacter.X, dbCharacter.Y, dbCharacter.Z),
            Vector3Int.zero)
        {
            this.dbCharacter = dbCharacter;
            Id = dbCharacter.Id;
            Name = dbCharacter.Name;
            Level = dbCharacter.Level;
            nCharacter = new()
            {
                Id = dbCharacter.Id,
                TypeId = dbCharacter.JobId,
                EntityId = 0,
                Name = dbCharacter.Name,
                Level = dbCharacter.Level,
                Exp = dbCharacter.Exp,
                SpaceId = dbCharacter.SpaceId,
                Gold = dbCharacter.Gold,
                Entity = null,
                Hp = dbCharacter.Hp,
                Mp = dbCharacter.Mp,
            };
        }
    }
}
