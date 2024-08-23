using GameServer.Manager;
using GameServer.Model;
using Serilog;
using Summer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Network
{
    /// <summary>
    /// 玩家服务
    /// 注册，登录，创建角色，进入游戏
    /// </summary>
    public class UserService
    {
        Space space = new();
        public void Start()
        {
            MessageRouter.Instance.Subscribe<Proto.GameEnterRequest>(OnGameEnterRequest);
            space.Name = "新手村";
            space.Id = 6; // 新手村Id
        }

        private void OnGameEnterRequest(Connection conn, Proto.GameEnterRequest message)
        {
            Log.Information("进入游戏");

            int entityId = EntityManager.Instance.NextEntityId;

            Random random = new();
            Vector3Int position = new(500 + random.Next(-5, 6), 0, 500 + random.Next(-5, 6));
            Character character = new(entityId, position, Vector3Int.zero);

            // 通知玩家登录成功
            Proto.GameEnterResponse response = new()
            {
                Success = true,
                Entity = character.GetProto(),
            };
            conn.Send(response);

            // 将新角色加入到地图
            space.CharacterJoin(conn, character);
        }
    }
}
