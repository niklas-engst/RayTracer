namespace RayTracer.Core;

public struct Color(byte r, byte g, byte b, byte a)
{
    public byte R = r;
    public byte G = g;
    public byte B = b;
    public byte A = a;

    public static Color operator +(Color color, Color otherColor)
    {
        return new Color
        (
            (byte)Math.Clamp(color.R + otherColor.R, 0, 255),
            (byte)Math.Clamp(color.G + otherColor.G, 0, 255),
            (byte)Math.Clamp(color.B + otherColor.B, 0, 255),
            (byte)Math.Clamp(color.A + otherColor.A, 0, 255)
        );
    }
    
    public static Color operator *(Color color, float scalar)
    {
        return new Color
        (
            (byte)Math.Clamp(color.R * scalar, 0, 255),
            (byte)Math.Clamp(color.G * scalar, 0, 255),
            (byte)Math.Clamp(color.B * scalar, 0, 255),
            (byte)Math.Clamp(color.A * scalar, 0, 255)
        );
    }
    
    public uint ToUInt32()
    {
        return (uint)(A << 24 | R << 16 | G << 8 | B);
    }
    
    public static Color FromFloat(float r, float g, float b, float a = 1f)
    {
        return new Color(
            (byte)Math.Clamp((int)(r * 255f), 0, 255),
            (byte)Math.Clamp((int)(g * 255f), 0, 255),
            (byte)Math.Clamp((int)(b * 255f), 0, 255),
            (byte)Math.Clamp((int)(a * 255f), 0, 255)
        );
    }
}