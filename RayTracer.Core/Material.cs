namespace RayTracer.Core;

public readonly struct Material(Color kDiffuse, float kSpecular, float kShininess)
{
    public readonly Color KDiffuse = kDiffuse;

    public readonly float KSpecular = kSpecular;
    public readonly float KShininess = kShininess;
}