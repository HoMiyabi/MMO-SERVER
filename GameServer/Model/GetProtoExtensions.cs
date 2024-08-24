namespace GameServer.Model;

internal static class GetProtoExtensions
{
    public static Proto.NVector3 GetProto(this Vector3Int self)
    {
        return new Proto.NVector3()
        {
            X = self.x,
            Y = self.y,
            Z = self.z,
        };
    }

    public static void SetFromProto(this Vector3Int self, Proto.NVector3 other)
    {
        self.x = other.X;
        self.y = other.Y;
        self.z = other.Z;
    }

    public static Proto.NEntity GetProto(this Entity self)
    {
        return new Proto.NEntity()
        {
            Id = self.EntityId,
            Position = self.Position.GetProto(),
            Direction = self.Direction.GetProto(),
        };
    }

    public static void SetFromProto(this Entity self, Proto.NEntity other)
    {
        // self.EntityId = other.Id;

        self.Position.SetFromProto(other.Position);
        self.Direction.SetFromProto(other.Direction);
    }
}