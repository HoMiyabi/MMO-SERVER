using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    public class Actor : Entity
    {
        public string Name { get; set; }

        public int Level { get; set; }

        public Actor(int id, Vector3Int position, Vector3Int direction) : base(id, position, direction)
        {

        }
    }
}
