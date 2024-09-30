using System;
using GameServer.Manager;
using GameServer.Model;
using Kirara;
using Proto;

namespace GameServer.Network
{
    /// <summary>
    /// 地图服务
    /// </summary>
    public class SpaceService : Singleton<SpaceService>
    {
        public void Start()
        {
            SpaceManager.Instance.Init();

            // 位置同步请求
            MessageRouter.Instance.Subscribe<SpaceEntitySyncRequest>(OnSpaceEntitySyncRequest);
        }

        private void OnSpaceEntitySyncRequest(Connection conn, SpaceEntitySyncRequest message)
        {
            // 获取当前场景
            var space = conn.Get<Character>()?.space;

            var nEntity = message.EntitySync.NEntity;
            var serverEntity = EntityManager.Instance.GetEntity(nEntity.EntityId);
            float distance = Float3.Distance(serverEntity.position, nEntity.Position.Float3());
            float dt = (float)(DateTime.UtcNow - serverEntity.lastUpdateTime).TotalSeconds;

            dt = Math.Min(dt, 1f);

            if (distance > serverEntity.speed * dt * 1.5f)
            {
                return;
            }

            space?.UpdateEntity(message.EntitySync);
        }
    }
}