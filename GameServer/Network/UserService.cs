﻿using GameServer.Database;
using GameServer.Manager;
using Kirara;
using Proto;
using Serilog;

namespace GameServer.Network
{
    /// <summary>
    /// 玩家服务
    /// 注册，登录，创建角色，进入游戏
    /// </summary>
    public class UserService : Singleton<UserService>
    {

        public void Start()
        {
            MessageRouter.Instance.Subscribe<GameEnterRequest>(OnGameEnterRequest);
            MessageRouter.Instance.Subscribe<UserLoginRequest>(OnUserLoginRequest);
            MessageRouter.Instance.Subscribe<CharacterCreateRequest>(OnCharacterCreateRequest);
            MessageRouter.Instance.Subscribe<CharacterListRequest>(OnCharacterListRequest);
            MessageRouter.Instance.Subscribe<CharacterDeleteRequest>(OnCharacterDeleteRequest);
        }

        private void OnCharacterDeleteRequest(Connection conn, CharacterDeleteRequest message)
        {
            var player = conn.Get<DbPlayer>();
            Db.fsql.Delete<DbCharacter>()
                .Where(it => it.Id == message.CharacterId)
                .Where(it => it.PlayerId == player.Id).ExecuteAffrows();
            var response = new CharacterDeleteResponse()
            {
                Success = true,
                Message = "执行完成",
            };
            conn.Send(response);
        }

        private void OnCharacterListRequest(Connection conn, CharacterListRequest message)
        {
            var response = new CharacterListResponse();

            var player = conn.Get<DbPlayer>();
            var list = Db.fsql.Select<DbCharacter>().Where(it => it.PlayerId == player.Id).ToList();
            foreach (var item in list)
            {
                response.NCharacters.Add(new NCharacter()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Tid = item.JobId,
                    Exp = item.Exp,
                    Gold = item.Gold,
                    Level = item.Level,
                    SpaceId = item.SpaceId,
                });
            }
            conn.Send(response);
        }

        private void OnCharacterCreateRequest(Connection conn, CharacterCreateRequest message)
        {
            Log.Information("创建角色请求 " + message);

            var response = new CharacterCreateResponse();

            var player = conn.Get<DbPlayer>();
            if (player == null)
            {
                Log.Information("未登录，不能创建角色");
                response.Success = false;
                response.Message = "未登录，不能创建角色";
                conn.Send(response);
                return;
            }

            // 模型验证
            if (string.IsNullOrWhiteSpace(message.Name))
            {
                Log.Information("创建角色失败，角色名不应为空");
                response.Success = false;
                response.Message = "创建角色失败，角色名不应为空";
                conn.Send(response);
                return;
            }
            if (message.Name.Length > 7)
            {
                Log.Information("创建角色失败，名字长度最大为7");
                response.Success = false;
                response.Message = "创建角色失败，名字长度最大为7";
                conn.Send(response);
                return;
            }

            // 检验角色名是否存在
            if (Db.fsql.Select<DbCharacter>().Where(it => it.Name == message.Name).Any())
            {
                Log.Information("创建角色失败，名字已被占用");
                response.Success = false;
                response.Message = "创建角色失败，名字已被占用";
                conn.Send(response);
                return;
            }

            long count = Db.fsql.Select<DbCharacter>().Where(it => it.PlayerId == player.Id).Count();
            if (count >= 4)
            {
                Log.Information("角色数量最多4个");
                response.Success = false;
                response.Message = "角色数量最多4个";
                conn.Send(response);
                return;
            }

            var dbCharacter = new DbCharacter()
            {
                Name = message.Name,
                JobId = message.JobId,
                Hp = 100,
                Mp = 100,
                Level = 1,
                Exp = 0,
                SpaceId = 1,
                Gold = 0,
                PlayerId = player.Id
            };
            int aff = Db.fsql.Insert(dbCharacter).ExecuteAffrows();
            if (aff > 0)
            {
                response.Success = true;
                response.Message = "创建成功";
                conn.Send(response);
            }
        }

        private void OnUserLoginRequest(Connection conn, UserLoginRequest message)
        {
            var dbPlayer = Db.fsql.Select<DbPlayer>()
                .Where(it => it.Username == message.Username)
                .Where(it => it.Password == message.Password)
                .First();

            var response = new UserLoginResponse();

            if (dbPlayer != null)
            {
                response.Success = true;
                response.Message = "登录成功";
                conn.Set(dbPlayer); // 登录成功，在conn里记录用户信息
            }
            else
            {
                response.Success = false;
                response.Message = "用户名或密码不正确";
            }
            conn.Send(response);
        }

        private void OnGameEnterRequest(Connection conn, GameEnterRequest message)
        {
            Log.Information($"玩家进入游戏，CharacterId={message.CharacterId}");

            // 获取当前玩家
            var player = conn.Get<DbPlayer>();
            // 查询数据库的角色
            var dbCharacter = Db.fsql.Select<DbCharacter>()
                .Where(it => it.Id == message.CharacterId)
                .Where(it => it.PlayerId == player.Id)
                .First();

            // 把数据库角色变成游戏角色
            var character = CharacterManager.Instance.CreateCharacter(dbCharacter);

            // 通知玩家登录成功
            var response = new GameEnterResponse()
            {
                Success = true,
                NCharacter = character.NCharacter,
            };
            // Log.Debug($"response={response}");
            conn.Send(response);

            // 将新角色加入到地图
            var space = SpaceManager.Instance.GetSpace(dbCharacter.SpaceId); // 新手村id
            space.CharacterEnter(conn, character);
        }
    }
}