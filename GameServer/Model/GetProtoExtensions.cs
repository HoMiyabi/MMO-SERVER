using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    internal static class GetProtoExtensions
    {
        public static Proto.NVector3 GetProto(this Vector3Int o)
        {
            return new()
            {
                X = o.x,
                Y = o.y,
                Z = o.z,
            };
        }

        public static Proto.NEntity GetProto(this Entity o)
        {
            return new()
            {
                Id = o.EntityId,
                Position = o.Position.GetProto(),
                Direction = o.Direction.GetProto(),
            };
        }
    }
}
