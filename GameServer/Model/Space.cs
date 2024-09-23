using System.Collections.Generic;
using Serilog;
using Kirara;

namespace GameServer.Model
{
    // 空间 场景
    public class Space
    {
        public SpaceDefine SpaceDefine { get; set; }

        // 当前场景中全部的角色 <ChrId, ChrObj>
        private Dictionary<int, Character> idToCharacter = new();

        private Dictionary<Connection, Character> connectionToCharacter = new();

        // public Space() {}

        public Space(SpaceDefine spaceDefine)
        {
            SpaceDefine = spaceDefine;
        }

        /// <summary>
        /// 角色进入场景
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="character"></param>
        public void CharacterEnter(Connection conn, Character character)
        {
            Log.Information($"角色进入场景 SpaceId={SpaceDefine.SID} CharacterId={character.Id}");

            // 角色和场景存入连接
            conn.Set(character);
            character.space = this;

            idToCharacter.Add(character.Id, character);
            character.conn = conn;

            if (!connectionToCharacter.ContainsKey(conn))
            {
                connectionToCharacter.Add(conn, character);
            }

            // 把新进入的角色广播给场景的其他玩家
            var response = new Proto.SpaceCharactersEnterResponse()
            {
                SpaceId = SpaceDefine.SID,
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
            Log.Information($"角色离开场景 SpaceId={SpaceDefine.SID} CharacterId={character.Id}");
            idToCharacter.Remove(character.Id);

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