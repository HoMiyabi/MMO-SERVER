using GameServer.Model;
using Kirara;
using Serilog;

namespace GameServer.Manager;

public class SpaceManager : Singleton<SpaceManager>
{
    private Dictionary<int, Space> idToSpace;

    public void Init()
    {
        idToSpace = new Dictionary<int, Space>();
        foreach (var (key, spaceDefine) in DefineManager.Instance.spaceDefineDict)
        {
            idToSpace.Add(key, new Space(spaceDefine));
            Log.Information($"初始化地图：{spaceDefine.Name}");
        }
    }

    public Space GetSpace(int id)
    {
        return idToSpace[id];
    }
}