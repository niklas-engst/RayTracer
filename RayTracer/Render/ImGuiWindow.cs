/*
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.Backends.GLFW;
using Hexa.NET.ImGui.Backends.OpenGL3;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Numerics;

namespace RayTracer.Render;

public class ImGuiWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
    : GameWindow(gameWindowSettings, nativeWindowSettings)
{
    protected ImGuiContextPtr ImGuiContext;

    protected override unsafe void OnLoad()
    {
        base.OnLoad();
        
        ImGuiContext = ImGui.CreateContext();
        ImGui.SetCurrentContext(ImGuiContext);
        ImGuiImplGLFW.SetCurrentContext(ImGuiContext);
        ImGuiImplOpenGL3.SetCurrentContext(ImGuiContext);

        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
        io.IniFilename = null;

        // GameWindow is GLFW-based internally. NativePtr is the GLFWwindow*.
        ImGuiImplGLFW.SetCurrentContext(ImGuiContext);
        var initGlfw = ImGuiImplGLFW.InitForOpenGL((GLFWwindow*)WindowPtr, true);

        // Choose GLSL matching your requested OpenGL context.
        // #version 410 works for the OpenGL 4.1 context requested in Program.cs.
        ImGuiImplOpenGL3.SetCurrentContext(ImGuiContext);
        var initOpenGl = ImGuiImplOpenGL3.Init("#version 410");
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, e.Width, e.Height);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        ImGui.SetCurrentContext(ImGuiContext);
        ImGuiImplGLFW.SetCurrentContext(ImGuiContext);
        ImGuiImplOpenGL3.SetCurrentContext(ImGuiContext);
        
        base.OnRenderFrame(args);
        
        ImGuiImplOpenGL3.NewFrame();
        ImGuiImplGLFW.NewFrame();

        // GLFW can report a zero logical size under some Wayland compositors.
        // OpenTK already tracks the usable window and framebuffer sizes.
        var io = ImGui.GetIO();
        io.DisplaySize = new Vector2(Size.X, Size.Y);
        io.DisplayFramebufferScale = new Vector2(
            FramebufferSize.X / (float)Math.Max(Size.X, 1),
            FramebufferSize.Y / (float)Math.Max(Size.Y, 1));

        ImGui.NewFrame();
        
        RenderScene(args);
        RenderInterface(args);

        ImGui.Render();
        ImGuiImplOpenGL3.RenderDrawData(ImGui.GetDrawData());

        SwapBuffers();
    }
    
    protected override void OnUnload()
    {
        ImGui.SetCurrentContext(ImGuiContext);
        ImGuiImplGLFW.SetCurrentContext(ImGuiContext);
        ImGuiImplOpenGL3.SetCurrentContext(ImGuiContext);

        ImGuiImplOpenGL3.Shutdown();
        ImGuiImplGLFW.Shutdown();
        ImGui.DestroyContext();

        base.OnUnload();
    }

    protected virtual void RenderInterface(FrameEventArgs args)
    {
        ImGui.ShowDemoWindow();
    }
    
    protected virtual void RenderScene(FrameEventArgs args) { }
}
*/