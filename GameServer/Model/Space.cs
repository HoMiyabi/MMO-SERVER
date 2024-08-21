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
            Log.Information($"角色进入场景 {character.Id}");

            idToCharacter.Add(character.Id, character);
            character.conn = conn;

            if (!connectionToCharacter.ContainsKey(conn))
            {
                connectionToCharacter.Add(conn, character);
            }

            // 广播给场景的其他玩家
            Proto.SpaceCharactersEnterResponse response = new()
            {
                SpaceId = Id,
            };
            response.EntityList.Add(character.GetData());

            foreach (var (_, ch) in idToCharacter)
            {
                // 发送角色进入场景的消息
                ch.conn.Send(response);
            }
        }
    }
}