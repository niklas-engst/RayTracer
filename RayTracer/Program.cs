using RayTracer.Render;

namespace RayTracer;

internal class Program
{
    private static void Main()
    {
        var window = new RayTracerWindow(500, 500);
        
        window.Run();
    }
}