using GameServer.Manager;
using GameServer.Model;
using Kirara;

namespace GameServer.Network;

/// <summary>
/// 地图服务
/// </summary>
public class SpaceService : Summer.Singleton<SpaceService>
{
    public void Start()
    {
        SpaceManager.Instance.Init();

        // 位置同步请求
        MessageRouter.Instance.Subscribe<Proto.SpaceEntitySyncRequest>(OnSpaceEntitySyncRequest);
    }

    private void OnSpaceEntitySyncRequest(Connection conn, Proto.SpaceEntitySyncRequest message)
    {
        // 获取当前场景
        var space = conn.Get<Space>();

        if (space == null)
        {
            return;
        }

        space.UpdateEntity(message.EntitySync);
    }
}