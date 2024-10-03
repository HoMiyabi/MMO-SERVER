using System.Collections.Generic;
using System.IO;
using Kirara;
using Newtonsoft.Json;

namespace GameServer.Manager
{
    public class DefineManager : Singleton<DefineManager>
    {
        public Dictionary<int, SpaceDefine> spaceDefineDict;
        public Dictionary<int, UnitDefine> unitDefineDict;

        public void Init()
        {
            spaceDefineDict = Load<SpaceDefine>("Define/SpaceDefine.json");
            unitDefineDict = Load<UnitDefine>("Define/UnitDefine.json");
        }

        private static string LoadFile(string filePath)
        {
            string text = File.ReadAllText(filePath);
            return text;
        }

        private static Dictionary<int, T> Load<T>(string filePath)
        {
            string json = LoadFile(filePath);
            return JsonConvert.DeserializeObject<Dictionary<int, T>>(json);
        }
    }
}