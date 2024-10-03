using System;
using GameServer.Manager;
using GameServer.Model;
using Kirara;
using Proto;
using Serilog;

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
            var character = conn.Get<Character>();
            var space = character?.space;

            var nEntity = message.EntitySync.NEntity;
            var serverEntity = EntityManager.Instance.GetEntity(nEntity.EntityId);
            float distance = Float3.Distance(serverEntity.position, nEntity.Position.Float3());
            float dt = (float)(DateTime.UtcNow - serverEntity.lastUpdateTime).TotalSeconds;

            dt = Math.Min(dt, 1f);

            float limit = serverEntity.speed * dt * 1.5f;

            if (float.IsNaN(distance) || distance > limit)
            {
                Log.Information($"角色移动过快 {character?.id.NameValue()} {distance.NameValue()} {limit.NameValue()}");
                var response = new SpaceEntitySyncResponse
                {
                    EntitySync = new NEntitySync
                    {
                        NEntity = serverEntity.NEntity,
                        Force = true,
                    },
                };
                conn.Send(response);
                return;
            }

            space?.UpdateEntity(message.EntitySync);
        }
    }
}