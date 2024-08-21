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
        }

        private void OnGameEnterRequest(Connection sender, Proto.GameEnterRequest message)
        {
            Log.Information("进入游戏");

            int entityId = EntityManager.Instance.NextEntityId;
            Vector3Int position = new(500, 1, 500);
            Character character = new(entityId, position, Vector3Int.zero);

            space.CharacterJoin(sender, character);
        }
    }
}
