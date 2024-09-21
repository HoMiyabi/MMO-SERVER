using System.Numerics;

namespace Common;

public struct Float3
{
    public float x;
    public float y;
    public float z;

    public Float3 xzy
    {
        get => new Float3(x, z, y);
        set
        {
            x = value.x;
            z = value.y;
            y = value.z;
        }
    }

    public Float3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static Float3 operator +(Float3 left, Float3 right)
    {
        return new Float3(left.x + right.x, left.y + right.y, left.z + right.z);
    }

    public static Float3 operator -(Float3 left, Float3 right)
    {
        return new Float3(left.x - right.x, left.y - right.y, left.z - right.z);
    }

    public static Float3 operator *(Float3 left, Float3 right)
    {
        return new Float3(left.x * right.x, left.y * right.y, left.z * right.z);
    }

    public static Float3 operator /(Float3 left, Float3 right)
    {
        return new Float3(left.x / right.x, left.y / right.y, left.z / right.z);
    }
}