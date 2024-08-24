using GameServer.Model;
using Summer;
using Summer.Network;

namespace GameServer.Network;

/// <summary>
/// 地图服务
/// </summary>
public class SpaceService : Singleton<SpaceService>
{
    private Dictionary<int, Space> idToSpace = new();

    public void Start()
    {
        // 位置同步请求
        MessageRouter.Instance.Subscribe<Proto.SpaceEntitySyncRequest>(OnSpaceEntitySyncRequest);

        // 新手村场景对象
        Space space = new();
        space.Name = "新手村";
        space.Id = 6; // 新手村Id
        idToSpace.Add(space.Id, space);
    }

    public Space GetSpace(int spaceId)
        => idToSpace[spaceId];

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