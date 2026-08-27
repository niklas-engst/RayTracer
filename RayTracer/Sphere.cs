namespace RayTracer;

public readonly struct Sphere(Vector3d center, float radius, Material material)
{
    public readonly Vector3d Center = center;
    public readonly float Radius = radius;

    public readonly Material Material = material;

    public readonly float Intersection(Ray ray)
    {
        var originMinusCenter = ray.Origin - Center;

        var b = ray.Direction.Dot(originMinusCenter);

        var c = originMinusCenter.Dot(originMinusCenter)
                  - Radius * Radius;

        var discriminant = b * b - c;

        if (discriminant < 0.0f)
            return float.PositiveInfinity;

        var root = MathF.Sqrt(discriminant);
        
        var t1 = -b - root;
        var t2 = -b + root;

        if (t1 > 0.0f)
            return t1;
        
        if (t2 > 0.0f)
            return t2;

        return float.PositiveInfinity;
    }
    
    public Vector3d NormalAt(Vector3d point)
    {
        return (point - Center).Normalized();
    }

    public Color DiffuseShading(Vector3d point, Light lightSource)
    {
        var lightDirection = (lightSource.Position - point).Normalized();
        var normal = NormalAt(point);
        
        var intensity = MathF.Max(0, normal.Dot(lightDirection));

        return Material.KDiffuse * intensity;
    }
}