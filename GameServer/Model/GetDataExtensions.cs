using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    internal static class GetDataExtensions
    {
        public static Proto.NVector3 GetData(this Vector3Int o)
        {
            return new()
            {
                X = o.x,
                Y = o.y,
                Z = o.z,
            };
        }

        public static Proto.NEntity GetData(this Entity o)
        {
            return new()
            {
                Id = o.Id,
                Position = o.Position.GetData(),
                Direction = o.Direction.GetData(),
            };
        }
    }
}
