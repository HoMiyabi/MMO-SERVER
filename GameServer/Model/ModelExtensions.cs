namespace GameServer.Model;

internal static class ModelExtensions
{
    public static Proto.NVector3 SetFromNative(this Proto.NVector3 self, Vector3Int other)
    {
        self.X = other.x;
        self.Y = other.y;
        self.Z = other.z;
        return self;
    }

    public static void SetFromProto(this Vector3Int self, Proto.NVector3 other)
    {
        self.x = other.X;
        self.y = other.Y;
        self.z = other.Z;
    }

    public static Proto.NVector3 GetProto(this Vector3Int self)
    {
        return new Proto.NVector3().SetFromNative(self);
    }

    public static void SetFromNative(this Proto.NEntity self, Entity other)
    {
        self.Id = other.EntityId;
        self.Position.SetFromNative(other.Position);
        self.Direction.SetFromNative(other.Direction);
    }

    public static void SetFromProto(this Entity self, Proto.NEntity other)
    {
        // self.EntityId = other.Id;

        self.Position.SetFromProto(other.Position);
        self.Direction.SetFromProto(other.Direction);
    }

    public static Proto.NEntity GetProto(this Entity self)
    {
        var entity = new Proto.NEntity()
        {
            Id = self.EntityId,
            Position = self.Direction.GetProto(),
            Direction = self.Direction.GetProto(),
        };
        return entity;
    }
}