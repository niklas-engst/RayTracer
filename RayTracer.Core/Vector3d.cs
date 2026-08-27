namespace RayTracer.Core;

public struct Vector3d(float x, float y, float z)
{
    public float X = x;
    public float Y = y;
    public float Z = z;

    public static Vector3d operator +(Vector3d v1, Vector3d v2)
    {
        return new Vector3d(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
    }

    public static Vector3d operator -(Vector3d v1, Vector3d v2)
    {
        return new Vector3d(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
    }

    public static Vector3d operator *(Vector3d v, float scalar)
    {
        return new Vector3d(v.X * scalar, v.Y * scalar, v.Z * scalar);
    }

    public static Vector3d operator *(float scalar, Vector3d v)
    {
        return new Vector3d(v.X * scalar, v.Y * scalar, v.Z * scalar);
    }

    public static Vector3d operator *(Vector3d v1, Vector3d v2)
    {
        return new Vector3d(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
    }
    
    public readonly float Dot(Vector3d other)
    {
        return X * other.X
               + Y * other.Y
               + Z * other.Z;
    }

    public readonly float Length()
    {
        return MathF.Sqrt(X * X + Y * Y + Z * Z);
    }

    public readonly Vector3d Normalized()
    {
        var length = Length();
        if (length == 0)
            throw new InvalidOperationException("Cannot normalize a zero-length vector.");

        return new Vector3d(X / length, Y / length, Z / length);
    }

    public readonly Vector3d Reflect(Vector3d normal)
    {
        return 2 * Dot(normal) * normal - this;
    }

    public override string ToString()
    {
        return $"Vector3d({X}, {Y}, {Z})";
    }
}