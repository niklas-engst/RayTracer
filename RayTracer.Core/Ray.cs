namespace RayTracer.Core;

public readonly struct Ray(Vector3d origin, Vector3d direction)
{
    public readonly Vector3d Origin = origin;
    public readonly Vector3d Direction = direction;
    
    public Vector3d At(float t)
    {
        return Origin + t * Direction;
    }
}