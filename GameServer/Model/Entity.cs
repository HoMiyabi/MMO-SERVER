namespace GameServer.Model
{
    // 在MMO世界进行同步的实体
    public class Entity
    {
        private int entityId;
        private Vector3Int position; // 位置
        private Vector3Int direction; // 方向

        private int spaceId;

        public int SpaceId
        {
            get => spaceId;
            set => spaceId = value;
        }

        public int EntityId => entityId;

        public Vector3Int Position
        {
            get => position;
            set => position = value;
        }

        public Vector3Int Direction
        {
            get => direction;
            set => direction = value;
        }

        public Entity(int entityId, Vector3Int position, Vector3Int direction)
        {
            this.entityId = entityId;
            this.position = position;
            this.direction = direction;
        }
    }
}
