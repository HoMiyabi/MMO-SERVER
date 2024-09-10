using GameServer.Database;
using GameServer.Manager;
using GameServer.Model;
using Proto;
using Serilog;
using Summer.Network;
using Summer;

namespace GameServer.Network;

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
    }

    private void OnUserLoginRequest(Connection conn, UserLoginRequest message)
    {
        var dbPlayer = Db.fsql.Select<DbPlayer>()
            .Where(it => it.Username == message.Username)
            .Where(it => it.Password == message.Password)
            .First();

        UserLoginResponse response = new();

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
        Log.Information("进入游戏");

        int entityId = EntityManager.Instance.NextEntityId;

        Random random = new();
        Vector3Int position = new(500 + random.Next(-5, 6), 0, 500 + random.Next(-5, 6));
        position *= 1000;

        Character character = new(entityId, position, Vector3Int.zero);

        // 通知玩家登录成功
        GameEnterResponse response = new()
        {
            Success = true,
            Entity = character.GetProto(),
        };
        Log.Debug($"response={response}");
        conn.Send(response);

        // 将新角色加入到地图
        var space = SpaceService.Instance.GetSpace(6); // 新手村id
        space.CharacterEnter(conn, character);
    }
}