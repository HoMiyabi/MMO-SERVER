using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    // 在MMO世界进行同步的实体
    public class Entity
    {
        private int id;
        private Vector3Int position; // 位置
        private Vector3Int direction; // 方向

        public int Id => id;

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

        public Entity(int id, Vector3Int position, Vector3Int direction)
        {
            this.id = id;
            this.position = position;
            this.direction = direction;
        }
    }
}
