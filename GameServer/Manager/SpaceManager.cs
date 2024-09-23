using System.Collections.Generic;
using GameServer.Model;
using Kirara;
using Serilog;

namespace GameServer.Manager
{
    public class SpaceManager : Singleton<SpaceManager>
    {
        private Dictionary<int, Space> idToSpace;

        public void Init()
        {
            idToSpace = new Dictionary<int, Space>();
            foreach (var (_, spaceDefine) in DefineManager.Instance.spaceDefineDict)
            {
                idToSpace.Add(spaceDefine.SID, new Space(spaceDefine));
                Log.Information($"初始化地图：{spaceDefine.Name}");
            }
        }

        public Space GetSpace(int id)
        {
            return idToSpace[id];
        }
    }
}