using Kirara;
using Proto;

namespace GameServer.Model
{
    // 在MMO世界进行同步的实体
    public class Entity
    {
        private NEntity nEntity;

        public NEntity NEntity
        {
            get => nEntity;
            set
            {
                nEntity.EntityId = value.EntityId;
                nEntity.Position = value.Position;
                nEntity.Direction = value.Direction;
                nEntity.Speed = value.Speed;
            }
        }

        public Entity(Float3 position, Float3 direction)
        {
            nEntity = new NEntity()
            {
                Position = position.GetProto(),
                Direction = direction.GetProto(),
            };
        }
    }
}
