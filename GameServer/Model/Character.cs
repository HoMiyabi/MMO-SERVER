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

        public Character(int entityId, Vector3Int position, Vector3Int direction) : base(entityId, position, direction)
        {

        }

        public static explicit operator Character(DbCharacter r)
        {
            // 申请EntityId
            int entityId = EntityManager.Instance.NextEntityId;

            var character = new Character(entityId, new Vector3Int(r.X, r.Y, r.Z), Vector3Int.zero)
            {
                Id = r.Id,
                Name = r.Name,
                Level = r.Level,
                nCharacter = new()
                {
                    Id = r.Id,
                    TypeId = r.JobId,
                    EntityId = 0,
                    Name = r.Name,
                    Level = r.Level,
                    Exp = r.Exp,
                    SpaceId = r.SpaceId,
                    Gold = r.Gold,
                    Entity = null,
                    Hp = r.Hp,
                    Mp = r.Mp,
                },
            };
            return character;
        }
    }
}
