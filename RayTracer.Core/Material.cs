namespace RayTracer.Core;

public class Material(Color kDiffuse, float kSpecular, float kShininess)
{
    protected readonly Color KDiffuse = kDiffuse;

    public readonly float KSpecular = kSpecular;
    public readonly float KShininess = kShininess;

    public virtual Color GetDiffuseColor(float u, float v)
    {
        return KDiffuse;
    }
}