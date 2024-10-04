﻿using Kirara;
using Proto;

namespace GameServer.Model
{
    public class Monster : Entity
    {
        public int level;
        public int spaceId;


        public Monster(Float3 position, Float3 direction, int TID, int level) : base(position, direction, EntityType.Monster, TID)
        {
            this.level = level;
        }
    }
}
