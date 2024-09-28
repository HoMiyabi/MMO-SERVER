using Kirara;
using Proto;

namespace GameServer.Model
{
    internal static class ModelExtensions
    {
        public static NFloat3 GetProto(this Float3 self)
        {
            return new NFloat3()
            {
                X = self.x,
                Y = self.y,
                Z = self.z,
            };
        }

        public static Float3 GetNative(this NFloat3 self)
        {
            return new Float3(self.X, self.Y, self.Z);
        }
    }
}