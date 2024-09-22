using Serilog;
using Kirara;

namespace GameServer.Model
{
    // 空间 场景
    public class Space
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public SpaceDefine SpaceDefine { get; set; }

        private Dictionary<int, Character> idToCharacter = new();

        private Dictionary<Connection, Character> connectionToCharacter = new();

        public Space() {}

        public Space(SpaceDefine spaceDefine)
        {
            SpaceDefine = spaceDefine;
            Id = spaceDefine.SID;
            Name = spaceDefine.Name;
        }

        /// <summary>
        /// 角色进入场景
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="character"></param>
        public void CharacterEnter(Connection conn, Character character)
        {
            Log.Information($"角色进入场景 EntityId={character.EntityId}");

            // 角色和场景存入连接
            conn.Set(character);
            conn.Set(this);

            character.SpaceId = Id;

            idToCharacter.Add(character.EntityId, character);
            character.conn = conn;

            if (!connectionToCharacter.ContainsKey(conn))
            {
                connectionToCharacter.Add(conn, character);
            }

            // 把新进入的角色广播给场景的其他玩家
            var response = new Proto.SpaceCharactersEnterResponse()
            {
                SpaceId = Id,
            };
            response.EntityList.Add(character.GetProto());

            // 发送角色进入场景的消息给其他人
            foreach (var (_, ch) in idToCharacter)
            {
                if (ch.conn != conn)
                {
                    ch.conn.Send(response);
                }
            }

            // 新上线的角色需要获取其他角色
            response.EntityList.Clear();
            foreach (var (_, ch) in idToCharacter)
            {
                if (ch.conn != conn)
                {
                    response.EntityList.Add(ch.GetProto());
                }
            }
            conn.Send(response);
        }

        /// <summary>
        /// 角色离开地图
        /// 客户端离线、切换地图
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="character"></param>
        public void CharacterLeave(Connection conn, Character character)
        {
            Log.Information($"角色离开场景 EntityId={character.EntityId}");
            conn.Set<Space>(null);
            idToCharacter.Remove(character.EntityId);

            var response = new Proto.SpaceCharacterLeaveResponse()
            {
                EntityId = character.EntityId
            };

            foreach (var (_, ch) in idToCharacter)
            {
                ch.conn.Send(response);
            }
        }

        /// <summary>
        /// 广播更新Entity信息
        /// </summary>
        /// <param name="entitySync"></param>
        public void UpdateEntity(Proto.NEntitySync entitySync)
        {
            // Log.Information("UpdateEntity " + entitySync);
            foreach (var (_, ch) in idToCharacter)
            {
                if (ch.EntityId == entitySync.Entity.Id)
                {
                    ch.SetFromProto(entitySync.Entity);
                }
                else
                {
                    var response = new Proto.SpaceEntitySyncResponse()
                    {
                        EntitySync = entitySync,
                    };
                    ch.conn.Send(response);
                }
            }
        }
    }
}