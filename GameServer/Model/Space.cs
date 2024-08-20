using Summer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    // 空间 场景
    public class Space
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Music { get; set; }

        private Dictionary<int, Character> idToCharacter = new();

        /// <summary>
        /// 角色进入场景
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="character"></param>
        public void CharacterJoin(Connection conn, Character character)
        {
            idToCharacter.Add(character.Id, character);
        }
    }
}