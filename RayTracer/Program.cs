namespace RayTracer;

internal class Program
{
    private static void Main()
    {
        var window = new RayTracerWindow(1000, 1000);
        
        window.Run();
    }
}