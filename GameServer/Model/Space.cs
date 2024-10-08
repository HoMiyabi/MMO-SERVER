﻿using System.Collections.Generic;
using GameServer.Manager;
using Serilog;
using Kirara;
using Proto;

namespace GameServer.Model
{
    // 空间 场景
    public class Space
    {
        public readonly int spaceId;

        // public SpaceDefine SpaceDefine { get; set; }

        // 当前场景中全部的角色 <ChrId, ChrObj>
        private Dictionary<int, Character> idToCharacter = new();

        // 当前场景中的野怪 <entityId, monster>
        private Dictionary<int, Monster> entityIdToMonster = new();

        private Dictionary<Connection, Character> connectionToCharacter = new();

        public MonsterManager monsterManager;

        public Space(SpaceDefine spaceDefine)
        {
            // SpaceDefine = spaceDefine;
            spaceId = spaceDefine.SID;

            monsterManager = new MonsterManager(this);
        }

        /// <summary>
        /// 角色进入场景
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="character"></param>
        public void CharacterEnter(Connection conn, Character character)
        {
            Log.Information($"角色进入场景 {spaceId.NameValue()} {character.id.NameValue()}");

            // 角色和场景存入连接
            conn.Set(character);
            character.space = this;

            idToCharacter.Add(character.id, character);
            character.conn = conn;

            connectionToCharacter.TryAdd(conn, character);

            // 把新进入的角色广播给场景的其他玩家
            var response = new SpaceCharactersEnterResponse()
            {
                SpaceId = spaceId,
            };
            response.NCharacters.Add(character.NCharacter);

            // 发送角色进入场景的消息给其他人
            foreach (var (_, ch) in idToCharacter)
            {
                if (ch.conn != conn)
                {
                    ch.conn.Send(response);
                }
            }

            // 新上线的角色需要获取其他角色
            response.NCharacters.Clear();
            foreach (var (_, ch) in idToCharacter)
            {
                if (ch.conn != conn)
                {
                    response.NCharacters.Add(ch.NCharacter);
                }
            }
            conn.Send(response);

            // 同步野怪

            var monstersEnterSpaceResponse = new MonstersEnterSpaceResponse()
            {
                SpaceId = spaceId
            };
            foreach (var (_, monster) in entityIdToMonster)
            {
                monstersEnterSpaceResponse.NMonsters.Add(monster.NMonster);
            }
            conn.Send(monstersEnterSpaceResponse);
        }

        /// <summary>
        /// 角色离开地图
        /// 客户端离线、切换地图
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="character"></param>
        public void CharacterLeave(Connection conn, Character character)
        {
            Log.Information($"角色离开场景 {spaceId.NameValue()} {character.id.NameValue()}");
            idToCharacter.Remove(character.id);

            var response = new SpaceCharacterLeaveResponse()
            {
                EntityId = character.entityId,
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
        public void UpdateEntity(NEntitySync entitySync)
        {
            // Log.Information("UpdateEntity " + entitySync);
            foreach (var (_, character) in idToCharacter)
            {
                if (character.entityId == entitySync.NEntity.EntityId)
                {
                    character.Update(entitySync.NEntity);
                }
                else
                {
                    var response = new SpaceEntitySyncResponse()
                    {
                        EntitySync = entitySync,
                    };
                    character.conn.Send(response);
                }
            }
        }

        public void AddMonster(Monster monster)
        {
            monster.space = this;
            monster.spaceId = spaceId;

            entityIdToMonster.Add(monster.entityId, monster);

            var response = new MonstersEnterSpaceResponse
            {
                SpaceId = spaceId
            };
            response.NMonsters.Add(monster.NMonster);

            foreach (var (_, character) in idToCharacter)
            {
                character.conn.Send(response);
            }
        }
    }
}