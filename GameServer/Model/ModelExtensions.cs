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
            var v = new Vector3Int();
            return v.SetFromProto(self);
        }

        public static ref Vector3Int SetFromProto(ref this Vector3Int self, NVector3 other)
        {
            self.x = other.X;
            self.y = other.Y;
            self.z = other.Z;
            return ref self;
        }


        public static void SetFromProto(this Entity self, NEntity other)
        {
            // self.EntityId = other.Id;

            self.Position = other.Position.GetNative();
            self.Direction = other.Direction.GetNative();
        }

        public static NEntity GetProto(this Entity self)
        {
            var entity = new NEntity()
            {
                Id = self.EntityId,
                Position = self.Position.GetProto(),
                Direction = self.Direction.GetProto(),
            };
            return entity;
        }
    }
}