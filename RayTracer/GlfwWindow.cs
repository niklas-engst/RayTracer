using System.Runtime.CompilerServices;
using Hexa.NET.GLFW;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.Backends.GLFW;
using Hexa.NET.ImGui.Backends.OpenGL3;
using Hexa.NET.OpenGL;
using GLFWmonitorPtr = Hexa.NET.GLFW.GLFWmonitorPtr;
using GLFWwindowPtr = Hexa.NET.GLFW.GLFWwindowPtr;

namespace RayTracer;

public class GlfwWindow(int width, int height)
{
    protected GL GL;
    
    public void Run()
    {
        GLFW.Init();
        
        var glslVersion = "#version 410";
        GLFW.WindowHint(GLFW.GLFW_CONTEXT_VERSION_MAJOR, 4);
        GLFW.WindowHint(GLFW.GLFW_CONTEXT_VERSION_MINOR, 1);
        GLFW.WindowHint(GLFW.GLFW_OPENGL_PROFILE, GLFW.GLFW_OPENGL_CORE_PROFILE);
        
        var mon = GLFW.GetPrimaryMonitor();
        var mainScale = ImGuiImplGLFW.GetContentScaleForMonitor(Unsafe.BitCast<GLFWmonitorPtr, Hexa.NET.ImGui.Backends.GLFW.GLFWmonitorPtr>(mon));
        var window = GLFW.CreateWindow((int)(width * mainScale), (int)(height * mainScale), "GLFW Example", null, null);
        if (window.IsNull)
        {
            Console.WriteLine("Failed to create GLFW window.");
            GLFW.Terminate();
            return;
        }
        
        GLFW.MakeContextCurrent(window);

        var guiContext = ImGui.CreateContext();
        ImGui.SetCurrentContext(guiContext);

        // Setup ImGui config.
        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;     // Enable Keyboard Controls
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;      // Enable Gamepad Controls
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;         // Enable Docking

        unsafe
        {
            io.IniFilename = null;
        }

        ImGui.StyleColorsDark();
        var style = ImGui.GetStyle();
        style.ScaleAllSizes(mainScale);
        style.FontScaleDpi = mainScale;
        io.ConfigDpiScaleFonts = true;
        io.ConfigDpiScaleViewports = true;

        if ((io.ConfigFlags & ImGuiConfigFlags.ViewportsEnable) != 0)
        {
            style.WindowRounding = 0.0f;
            style.Colors[(int)ImGuiCol.WindowBg].W = 1.0f;
        }

        ImGuiImplGLFW.SetCurrentContext(guiContext);

        if (!ImGuiImplGLFW.InitForOpenGL(Unsafe.BitCast<GLFWwindowPtr, Hexa.NET.ImGui.Backends.GLFW.GLFWwindowPtr>(window), true))
        {
            Console.WriteLine("Failed to init ImGui Impl GLFW");
            GLFW.Terminate();
            return;
        }

        ImGuiImplOpenGL3.SetCurrentContext(guiContext);
        if (!ImGuiImplOpenGL3.Init(glslVersion))
        {
            Console.WriteLine("Failed to init ImGui Impl OpenGL3");
            GLFW.Terminate();
            return;
        }

        GL = new GL(new BindingsContext(window));
        OnLoad();

        // Main loop
        while (GLFW.WindowShouldClose(window) == 0)
        {
            // Poll for and process events
            GLFW.PollEvents();

            if (GLFW.GetWindowAttrib(window, GLFW.GLFW_ICONIFIED) != 0)
            {
                ImGuiImplGLFW.Sleep(10);
                continue;
            }

            GLFW.MakeContextCurrent(window);
            GL.ClearColor(1, 0.8f, 0.75f, 1);
            GL.Clear(GLClearBufferMask.ColorBufferBit);

            ImGuiImplOpenGL3.NewFrame();
            ImGuiImplGLFW.NewFrame();
            ImGui.NewFrame();

            RenderInterface();

            ImGui.Render();

            GLFW.MakeContextCurrent(window);
            ImGuiImplOpenGL3.RenderDrawData(ImGui.GetDrawData());

            if ((io.ConfigFlags & ImGuiConfigFlags.ViewportsEnable) != 0)
            {
                ImGui.UpdatePlatformWindows();
                ImGui.RenderPlatformWindowsDefault();
            }

            GLFW.MakeContextCurrent(window);

            // Swap front and back buffers (double buffering)
            GLFW.SwapBuffers(window);
        }

        OnUnload();
        ImGuiImplOpenGL3.Shutdown();
        ImGuiImplOpenGL3.SetCurrentContext(null);
        ImGuiImplGLFW.Shutdown();
        ImGuiImplGLFW.SetCurrentContext(null);
        ImGui.DestroyContext();
        GL.Dispose();

        // Clean up and terminate GLFW
        GLFW.DestroyWindow(window);
        GLFW.Terminate();
    }

    protected virtual void RenderInterface()
    {
        ImGui.ShowDemoWindow();
    }

    protected virtual void OnLoad()
    {
    }

    protected virtual void OnUnload()
    {
    }
}

internal unsafe class BindingsContext(GLFWwindowPtr window) : HexaGen.Runtime.IGLContext
{
    public nint Handle => (nint)window.Handle;

    public bool IsCurrent => GLFW.GetCurrentContext() == window;

    public void Dispose()
    {
    }

    public nint GetProcAddress(string procName)
    {
        return (nint)GLFW.GetProcAddress(procName);
    }

    public bool IsExtensionSupported(string extensionName)
    {
        return GLFW.ExtensionSupported(extensionName) != 0;
    }

    public void MakeCurrent()
    {
        GLFW.MakeContextCurrent(window);
    }

    public void SwapBuffers()
    {
        GLFW.SwapBuffers(window);
    }

    public void SwapInterval(int interval)
    {
        GLFW.SwapInterval(interval);
    }

    public bool TryGetProcAddress(string procName, out nint procAddress)
    {
        procAddress = (nint)GLFW.GetProcAddress(procName);
        return procAddress != 0;
    }
}