namespace Kirara
{
    public class Int3
    {
        public int x;
        public int y;
        public int z;

        public static readonly Int3 zero = new Int3(0, 0, 0);

        public Int3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        
        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }

        public static Int3 operator +(Int3 l, Int3 r)
        {
            return new Int3(l.x + r.x, l.y + r.y, l.z + r.z);
        }

        public static Int3 operator -(Int3 l, Int3 r)
        {
            return new Int3(l.x - r.x, l.y - r.y, l.z - r.z);
        }

        public static Int3 operator *(Int3 l, Int3 r)
        {
            return new Int3(l.x * r.x, l.y * r.y, l.z * r.z);
        }

        public int Dot(Int3 v)
        {
            return x * v.x + y * v.y + z * v.z;
        }
    }
}