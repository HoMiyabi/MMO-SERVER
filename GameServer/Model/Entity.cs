using System;
using Kirara;
using Proto;

namespace GameServer.Model
{
    // 在MMO世界进行同步的实体
    public class Entity
    {
        public int entityId;
        public Float3 position;
        public Float3 direction;
        public int speed;

        public DateTime lastUpdateTime;

        private NEntity nEntity;
        public NEntity NEntity
        {
            get
            {
                nEntity.EntityId = entityId;
                nEntity.Position = position.Proto();
                nEntity.Direction = position.Proto();
                nEntity.Speed = speed;
                return nEntity;
            }
        }

        public void Update(NEntity nEntity)
        {
            entityId = nEntity.EntityId;
            position = nEntity.Position.Float3();
            direction = nEntity.Direction.Float3();
            speed = nEntity.Speed;

            lastUpdateTime = DateTime.UtcNow;
        }

        public Entity(Float3 position, Float3 direction)
        {
            nEntity = new NEntity();
            this.position = position;
            this.direction = direction;
            lastUpdateTime = DateTime.UtcNow;
        }
    }
}
