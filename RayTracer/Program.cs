using OpenTK.Windowing.Desktop;
using RayTracer.Render;

namespace RayTracer;

internal class Program
{
    private static void Main()
    {
        using var window = new RayTracerWindow(GameWindowSettings.Default, new NativeWindowSettings()
        {
            ClientSize = (500, 500)
        });
        
        window.Run();
    }
}