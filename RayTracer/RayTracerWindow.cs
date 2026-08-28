using Hexa.NET.ImGui;
using RayTracer.Core;

namespace RayTracer;

public class RayTracerWindow(int width, int height) : PixelBufferWindow(width, height)
{
    protected bool Realtime = false;
    protected bool SinglePass = true;

    protected Vector3d LightPosition =  new Vector3d(0, 2, -1);
    protected Vector3d SpherePosition = new Vector3d(0, 0, 1);
    protected Vector3d Sphere2Position = new Vector3d(1, 0, 3);

    protected float Specular = 1.0f;
    protected float Shininess = 100.0f;

    protected int InitialDepth = 2;
    
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
            
            fixed (Vector3d* sphere2PosPtr = &Sphere2Position)
            {
                ImGui.SliderFloat3("Sphere2 position", (float*)sphere2PosPtr, -10, 10);
            }
            
            ImGui.SliderFloat("Specular", ref Specular, 0.0f, 5.0f);
            ImGui.SliderFloat("Shininess", ref Shininess, 0.0f, 200.0f);
            
            ImGui.SliderInt("Initial depth", ref InitialDepth, 1, 5);
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
        var material = new CheckeredMaterial(new Color(0, 255, 0, 255), new Color(255, 0, 255, 255), Specular, Shininess, 16, 8);
        var light = new Light(LightPosition);
        
        var earthMaterial = new Material ( new Color (255, 219, 188, 255), 0.1f,100) ;

        List<Sphere> objects =
        [
            new(SpherePosition, 1f, material),
            new(Sphere2Position, 1.5f, new Material(new Color(255, 0, 0, 255), Specular, Shininess)),
            new(new Vector3d (0, -1001, 0), 1000, earthMaterial)
        ];
        
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

                var color = RecursiveRaytrace(ray, objects, light, InitialDepth);
                
                Pixels[windowY * RenderWidth + windowX] = color.ToUInt32();
            }
        }
    }
    
    protected Color RecursiveRaytrace(Ray ray, List<Sphere> objects, Light light, int depth)
    {
        if (depth <= 0)
        {
            return new Color(0, 0, 0, 255); // Base case: return black color
        }
        
        var closestIntersection = float.PositiveInfinity;
        Sphere? closestSphere = null;

        foreach (var sphere in objects)
        {
            var intersection = sphere.Intersection(ray);
            if (intersection < closestIntersection)
            {
                closestIntersection = intersection;
                closestSphere = sphere;
            }
        }

        if (closestSphere == null)
            return new Color(128, 128, 128, 128); // Background color
        
        var intersectionPoint = ray.At(closestIntersection);

        Color color = new(0, 0, 0, 255);
        if (!InShadow(intersectionPoint, objects, light))
        {
            color = closestSphere.Value.Shading(intersectionPoint, light, ray.Direction);
        }

        var normal = closestSphere.Value.NormalAt(intersectionPoint);
        var reflectionDirection = (ray.Direction - 2f * ray.Direction.Dot(normal) * normal).Normalized();
        
        var reflectedRay = new Ray(intersectionPoint + normal * 0.001f, reflectionDirection);

        var reflectedColor = RecursiveRaytrace(reflectedRay, objects, light, depth - 1);
        color += reflectedColor * closestSphere.Value.Material.KSpecular;
        
        return color;
    }
    
    protected bool InShadow(Vector3d point, List<Sphere> objects, Light light)
    {
        var lightDirection = (light.Position - point).Normalized();
        var shadowRay = new Ray(point, lightDirection);

        foreach (var sphere in objects)
        {
            var intersection = sphere.Intersection(shadowRay);
            if (intersection > 0.001 && intersection < (light.Position - point).Length())
            {
                return true;
            }
        }

        return false;
    }
}