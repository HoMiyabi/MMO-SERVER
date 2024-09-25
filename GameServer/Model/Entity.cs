using Proto;

namespace GameServer.Model
{
    // 在MMO世界进行同步的实体
    public class Entity
    {
        private Vector3Int position; // 位置
        private Vector3Int direction; // 方向
        private NEntity nEntity; // 网络对象

        public int EntityId => nEntity.Id;

        public Vector3Int Position
        {
            get => position;
            set
            {
                position = value;
                nEntity.Position = value.GetProto();
            }
        }

        public Vector3Int Direction
        {
            get => direction;
            set
            {
                direction = value;
                nEntity.Direction = value.GetProto();
            }
        }

        public NEntity NEntity
        {
            get => nEntity;
            set
            {
                nEntity = value;
                position = value.Position.GetNative();
                direction = value.Direction.GetNative();
            }
        }

        public Entity(Vector3Int position, Vector3Int direction)
        {
            nEntity = new NEntity();
            Position = position;
            Direction = direction;
        }
    }
}
