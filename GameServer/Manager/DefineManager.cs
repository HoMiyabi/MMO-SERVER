using System.Collections.Generic;
using System.IO;
using Kirara;
using Newtonsoft.Json;

namespace GameServer.Manager
{
    public class DefineManager : Singleton<DefineManager>
    {
        public Dictionary<int, SpaceDefine> SIDToSpaceDefine;
        public Dictionary<int, UnitDefine> TIDToUnitDefine;

        public void Init()
        {
            SIDToSpaceDefine = Load<SpaceDefine>("Define/SpaceDefine.json");
            TIDToUnitDefine = Load<UnitDefine>("Define/UnitDefine.json");
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