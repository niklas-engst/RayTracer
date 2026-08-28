namespace RayTracer.Core;

public class CheckeredMaterial(Color kDiffuse, Color kDiffuse2, float kSpecular, float kShininess, int repeatU, int repeatV) : Material(kDiffuse, kSpecular, kShininess)
{
    public readonly Color KDiffuse2 = kDiffuse2;
    public readonly int RepeatU = repeatU;
    public readonly int RepeatV = repeatV;

    public override Color GetDiffuseColor(float u, float v)
    {
        var a = (int)MathF.Floor(u * RepeatU);
        var b = (int)MathF.Floor(v * RepeatV);
        
        return (a + b) % 2 == 0 ? KDiffuse : KDiffuse2;
    }
}