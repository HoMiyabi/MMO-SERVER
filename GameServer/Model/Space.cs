using Serilog;
using Summer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    // 空间 场景
    public class Space
    {
        public int Id { get; set; }
        public string Name { get; set; }

        private Dictionary<int, Character> idToCharacter = new();

        private Dictionary<Connection, Character> connectionToCharacter = new();

        /// <summary>
        /// 角色进入场景
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="character"></param>
        public void CharacterJoin(Connection conn, Character character)
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
            Proto.SpaceCharactersEnterResponse response = new()
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
        /// 广播更新Entity信息
        /// </summary>
        /// <param name="entitySync"></param>
        public void UpdateEntity(Proto.NEntitySync entitySync)
        {
            Log.Information("UpdateEntity " + entitySync);
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