using Hexa.NET.ImGui;
using RayTracer.Core;

namespace RayTracer;

public class RayTracerWindow(int width, int height) : PixelBufferWindow(width, height)
{
    protected bool Realtime = true;
    protected bool SinglePass = false;
    
    protected override void RenderInterface()
    {
        RenderPixels();

        if (!ImGui.Begin("CPU ray tracer", ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.End();
            return;
        }
        
        ImGui.Text($"Resolution: {RenderWidth} x {RenderHeight}");
        ImGui.Text($"Pixels: {Pixels.Length:N0}");
        ImGui.Text($"FPS: {ImGui.GetIO().Framerate:0.0}");
        
        ImGui.Spacing();
        
        ImGui.Checkbox("Realtime", ref Realtime);
        if (ImGui.Button("Single pass"))
        {
            SinglePass = true;
        }

        RenderPixelBuffer();
        
        ImGui.End();
    }

    protected override void RenderPixels()
    {
        if (!Realtime && !SinglePass)
        {
            return;
        }

        if (SinglePass)
        {
            SinglePass = false;
        }
        
        var camera = new Vector3d(0, 0, -1);
        var sphere = new Sphere(new Vector3d(0, 0, 1), 1f, new Material(new Color(0, 255, 255, 255), 1f, 1f));
        var light = new Light(new Vector3d(0, 2, -1));
        
        for (var windowY = 0; windowY < RenderHeight; windowY++)
        {
            for (var windowX = 0; windowX < RenderWidth; windowX++)
            {
                var normalizedX = (windowX + 0.5f) / RenderWidth;
                var normalizedY = (windowY + 0.5f) / RenderHeight;
                var x = 2 * normalizedX - 1;
                var y = 1 - 2 * normalizedY;
                
                var screenPoint = new Vector3d(x, y, 0);
                var rayDirection = (screenPoint - camera).Normalized();
                var ray = new Ray(camera, rayDirection);

                var intersection = sphere.Intersection(ray);

                if (!float.IsInfinity(intersection))
                {
                    var intersectionPoint = ray.At(intersection);
                    var color = sphere.DiffuseShading(intersectionPoint, light);
                    Pixels[windowY * RenderWidth + windowX] = color.ToUInt32();
                }
                else
                {
                    Pixels[windowY * RenderWidth + windowX] = new Color(128, 128, 128, 128).ToUInt32();
                }
            }
        }
    }
}