using Kirara;
using Proto;

namespace GameServer.Model
{
    // 在MMO世界进行同步的实体
    public class Entity
    {
        public Int3 position; // 位置
        public Int3 direction; // 方向
        private NEntity nEntity;

        public int EntityId => nEntity.EntityId;

        public NEntity NEntity
        {
            get
            {
                nEntity.Position = position.GetProto();
                nEntity.Direction = direction.GetProto();
                return nEntity;
            }
            set
            {
                nEntity = value;
                position = value.Position.GetNative();
                direction = value.Direction.GetNative();
            }
        }

        public Entity(Int3 position, Int3 direction)
        {
            nEntity = new NEntity();
            this.position = position;
            this.direction = direction;
        }
    }
}
