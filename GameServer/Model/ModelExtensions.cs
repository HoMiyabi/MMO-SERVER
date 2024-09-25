using Proto;

namespace GameServer.Model
{
    internal static class ModelExtensions
    {
        public static NVector3 GetProto(this Vector3Int self)
        {
            return new NVector3()
            {
                X = self.x,
                Y = self.y,
                Z = self.z,
            };
        }

        public static Vector3Int GetNative(this NVector3 self)
        {
            return new Vector3Int()
            {
                x = self.X,
                y = self.Y,
                z = self.Z,
            };
        }
    }
}