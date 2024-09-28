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

            if (space == null)
            {
                return;
            }

            space.UpdateEntity(message.EntitySync);
        }
    }
}