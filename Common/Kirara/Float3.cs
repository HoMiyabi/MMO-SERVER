﻿using System;

namespace Kirara
{
    public struct Float3
    {
        public float x;
        public float y;
        public float z;

        public Float3 xzy
        {
            get => new(x, z, y);
            set
            {
                x = value.x;
                z = value.y;
                y = value.z;
            }
        }

        public static readonly Float3 Zero = new(0f, 0f, 0f);
        public static readonly Float3 One = new(1f, 1f, 1f);

        public Float3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public void Set(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }

        public static Float3 operator +(Float3 l, Float3 r)
        {
            return new Float3(l.x + r.x, l.y + r.y, l.z + r.z);
        }

        public static Float3 operator -(Float3 l, Float3 r)
        {
            return new Float3(l.x - r.x, l.y - r.y, l.z - r.z);
        }

        public static Float3 operator *(Float3 l, Float3 r)
        {
            return new Float3(l.x * r.x, l.y * r.y, l.z * r.z);
        }

        public static Float3 operator *(Float3 l, float r)
        {
            return new Float3(l.x * r, l.y * r, l.z * r);
        }

        public static Float3 operator *(float l, Float3 r)
        {
            return new Float3(l * r.x, l * r.y, l * r.z);
        }

        public static Float3 operator /(Float3 l, Float3 r)
        {
            return new Float3(l.x / r.x, l.y / r.y, l.z / r.z);
        }

        public static Float3 operator /(Float3 l, float r)
        {
            return new Float3(l.x / r, l.y / r, l.z / r);
        }

        public float Dot(Float3 v)
        {
            return x * v.x + y * v.y + z * v.z;
        }

        public float Length()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }

        public static float Distance(Float3 l, Float3 r)
        {
            return (l - r).Length();
        }

        public static Float3 Max(Float3 l, Float3 r)
        {
            return new Float3(Math.Max(l.x, r.x), Math.Max(l.y, r.y), Math.Max(l.z, r.z));
        }

        public static Float3 Min(Float3 l, Float3 r)
        {
            return new Float3(Math.Min(l.x, r.x), Math.Min(l.y, r.y), Math.Min(l.z, r.z));
        }

        public static Float3 Lerp(Float3 a, Float3 b, float t)
        {
            return a + (b - a) * Math.Clamp(t, 0f, 1f);
        }

        public static Float3 LerpUnclamped(Float3 a, Float3 b, float t)
        {
            return a + (b - a) * t;
        }
    }
}