using Kirara;
using Proto;

namespace GameServer.Model
{
    internal static class ModelExtensions
    {
        public static NInt3 GetProto(this Int3 self)
        {
            return new NInt3()
            {
                X = self.x,
                Y = self.y,
                Z = self.z,
            };
        }

        public static Int3 GetNative(this NInt3 self)
        {
            return new Int3(self.X, self.Y, self.Z);
        }
    }
}