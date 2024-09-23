using Proto;

namespace GameServer.Model
{
    public class Actor : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int Speed { get; set; }

        public Space space { get; set; }

        public NCharacter nCharacter { get; set; }

        public Actor(int entityId, Vector3Int position, Vector3Int direction) : base(entityId, position, direction)
        {

        }
    }
}
