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

    // 不太合理 space应该是conn或player的属性
    public Space GetSpace(Connection conn)
    {
        foreach (Space space in idToSpace.Values)
        {
            if (space.HasConnection(conn))
            {
                return space;
            }
        }
        return null;
    }

    private void OnSpaceEntitySyncRequest(Connection conn, Proto.SpaceEntitySyncRequest message)
    {
        // 找到 conn 所在的地图
        Space space = GetSpace(conn);
        space.UpdateEntity(message.EntitySync);
    }
}