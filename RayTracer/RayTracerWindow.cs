using Hexa.NET.ImGui;
using RayTracer.Core;

namespace RayTracer;

public class RayTracerWindow(int width, int height) : PixelBufferWindow(width, height)
{
    protected bool Realtime = false;
    protected bool SinglePass = true;

    protected Vector3d LightPosition =  new Vector3d(0, 2, -1);
    protected Vector3d SpherePosition = new  Vector3d(0, 0, 1);

    protected float Specular = 1.0f;
    protected float Shininess = 100.0f;
    
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

        unsafe
        {
            fixed (Vector3d* lightPosPtr = &LightPosition)
            {
                ImGui.SliderFloat3("Light position", (float*)lightPosPtr, -10, 10);
            }
            
            fixed (Vector3d* spherePosPtr = &SpherePosition)
            {
                ImGui.SliderFloat3("Sphere position", (float*)spherePosPtr, -10, 10);
            }
            
            ImGui.SliderFloat("Specular", ref Specular, 0.0f, 5.0f);
            ImGui.SliderFloat("Shininess", ref Shininess, 0.0f, 200.0f);
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
        var sphere = new Sphere(SpherePosition, 1f, new Material(new Color(0, 255, 255, 255), Specular, Shininess));
        var light = new Light(LightPosition);
        
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
                    var color = sphere.Shading(intersectionPoint, light, ray.Direction);
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