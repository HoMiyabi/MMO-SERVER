using System.Collections.Generic;
using GameServer.Model;
using Kirara;
using Serilog;

namespace GameServer.Manager
{
    public class SpaceManager : Singleton<SpaceManager>
    {
        private Dictionary<int, Space> spaceIdToSpace;

        public void Init()
        {
            spaceIdToSpace = new Dictionary<int, Space>();
            foreach (var (_, spaceDefine) in DefineManager.Instance.SIDToSpaceDefine)
            {
                spaceIdToSpace.Add(spaceDefine.SID, new Space(spaceDefine));
                Log.Information($"初始化地图：{spaceDefine.Name}");
            }
        }

        public Space GetSpace(int id)
        {
            return spaceIdToSpace[id];
        }
    }
}