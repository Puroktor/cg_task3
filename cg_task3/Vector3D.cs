using System;
using System.Drawing;

namespace cg_task3
{
    public class Vector3D
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3D(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3D(float x, float y, float z, float h)
        {
            X = x / h;
            Y = y / h;
            Z = z / h;
        }

        public PointF XY()
        {
            return new PointF(X, Y);
        }

        public float Size()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }
        public static Vector3D operator *(Vector3D first, float second)
        {
            return new Vector3D(first.X * second, first.Y * second, first.Z * second);
        }

        public static Vector3D operator -(Vector3D first, Vector3D second)
        {
            return new Vector3D(second.X - first.X, second.Y - first.Y, second.Z - first.Z);
        }

        public static float operator *(Vector3D first, Vector3D second)
        {
            return first.X * second.X + first.Y * second.Y + first.Z * second.Z;
        }

        public static Vector3D operator ^(Vector3D f, Vector3D s)
        {
            float x = f.Y * s.Z - s.Y * f.Z;
            float y = s.X * f.Z - f.X * s.Z;
            float z = f.X * s.Y - s.X * f.Y;
            return new Vector3D(x, y, z);
        }
    }
}
