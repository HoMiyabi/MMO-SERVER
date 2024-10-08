﻿using System;
using GameServer.Manager;
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
        public float speed;

        public EntityType entityType;
        public UnitDefine unitDefine;

        public DateTime lastUpdateTime;

        public NEntity NEntity => new()
        {
            EntityId = entityId,
            Position = position.Proto(),
            Direction = direction.Proto(),
            Speed = speed,
        };

        public void Update(NEntity nEntity)
        {
            entityId = nEntity.EntityId;
            position = nEntity.Position.Float3();
            direction = nEntity.Direction.Float3();
            // speed = nEntity.Speed;

            lastUpdateTime = DateTime.UtcNow;
        }

        public Entity(Float3 position, Float3 direction, EntityType entityType, int tid)
        {
            this.position = position;
            this.direction = direction;
            lastUpdateTime = DateTime.UtcNow;

            this.entityType = entityType;
            unitDefine = DefineManager.Instance.TIDToUnitDefine[tid];
        }
    }
}
