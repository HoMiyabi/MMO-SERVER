using GameServer.Manager;
using GameServer.Model;
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
        MessageRouter.Instance.Subscribe<Proto.GameEnterRequest>(OnGameEnterRequest);
    }

    private void OnGameEnterRequest(Connection conn, Proto.GameEnterRequest message)
    {
        Log.Information("进入游戏");

        int entityId = EntityManager.Instance.NextEntityId;

        Random random = new();
        Vector3Int position = new(500 + random.Next(-5, 6), 0, 500 + random.Next(-5, 6));
        position *= 1000;

        Character character = new(entityId, position, Vector3Int.zero);

        // 通知玩家登录成功
        Proto.GameEnterResponse response = new()
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